using CDQTSystem_API.Messages;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_API.Consumers
{
	public class CourseRegistrationConsumer : IConsumer<CourseRegistrationMessage>
	{
		private readonly IUnitOfWork<UniversityDbContext> _unitOfWork;
		private readonly ILogger<CourseRegistrationConsumer> _logger;
		private readonly SemaphoreSlim _semaphore;

		public CourseRegistrationConsumer(
			IUnitOfWork<UniversityDbContext> unitOfWork,
			ILogger<CourseRegistrationConsumer> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
			_semaphore = new SemaphoreSlim(1, 1); // Ensure sequential processing
		}

		public async Task Consume(ConsumeContext<CourseRegistrationMessage> context)
		{
			var message = context.Message;

			try
			{
				await _semaphore.WaitAsync();

				// Start transaction using DbContextTransaction
				await using var transaction = await _unitOfWork.Context.Database.BeginTransactionAsync();

				try
				{
					// Get latest class section data with lock
					var classSection = await _unitOfWork.GetRepository<ClassSection>()
						.SingleOrDefaultAsync(
							predicate: cs => cs.Id == message.CourseOfferingId,
							include: q => q
								.Include(cs => cs.Course) // Ensure Microsoft.EntityFrameworkCore is imported
								.Include(cs => cs.CourseRegistrations)
						);

					if (classSection == null)
					{
						throw new BadHttpRequestException("Class section not found");
					}

					// Check for existing registration
					var existingRegistration = await _unitOfWork.GetRepository<CourseRegistration>()
						.SingleOrDefaultAsync(
							predicate: r => r.StudentId == message.StudentId &&
										  r.ClassSectionId == message.CourseOfferingId
						);

					if (existingRegistration != null)
					{
						throw new BadHttpRequestException("Already registered for this course");
					}

					// Check capacity with actual count
					var currentRegistrations = await _unitOfWork.GetRepository<CourseRegistration>()
						.GetListAsync(
							predicate: r => r.ClassSectionId == message.CourseOfferingId &&
										  r.Status != "Cancelled"
						);

					bool isWaitlisted = currentRegistrations.Count >= classSection.Capacity;

					// Get current registration period
					var currentPeriod = await _unitOfWork.GetRepository<RegistrationPeriod>()
						.SingleOrDefaultAsync(
							predicate: rp => rp.Status == "Open" &&
										   rp.StartDate <= DateTime.UtcNow &&
										   rp.EndDate >= DateTime.UtcNow
						);

					if (currentPeriod == null)
					{
						throw new BadHttpRequestException("Registration period is closed");
					}

					// Create registration
					var registration = new CourseRegistration
					{
						Id = Guid.NewGuid(),
						StudentId = message.StudentId,
						ClassSectionId = message.CourseOfferingId,
						RegistrationDate = DateTime.UtcNow,
						Status = isWaitlisted ? "Waitlisted" : "Registered",
						RegistrationPeriodId = currentPeriod.Id,
						TuitionStatus = "Pending"
					};

					await _unitOfWork.GetRepository<CourseRegistration>().InsertAsync(registration);
					await _unitOfWork.CommitAsync();
					await transaction.CommitAsync();

					// Publish result
					await context.Publish(new CourseRegistrationResult
					{
						RequestId = message.RequestId,
						Success = true,
						Status = registration.Status,
						Message = $"Successfully {registration.Status.ToLower()} for the course"
					});
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					_logger.LogError(ex, "Error during transaction");
					throw;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing course registration");

				await context.Publish(new CourseRegistrationResult
				{
					RequestId = message.RequestId,
					Success = false,
					Message = ex.Message
				});
			}
			finally
			{
				_semaphore.Release();
			}
		}
	}
}
