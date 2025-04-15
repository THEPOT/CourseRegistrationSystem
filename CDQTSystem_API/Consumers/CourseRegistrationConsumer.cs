using CDQTSystem_API.Messages;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_API.Consumers
{
    public class CourseRegistrationConsumer : IConsumer<CourseRegistrationMessage>
    {
        private readonly IUnitOfWork<DbContext> _unitOfWork;
        private readonly ILogger<CourseRegistrationConsumer> _logger;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(10, 10); // Allow 10 concurrent registrations

        public CourseRegistrationConsumer(
            IUnitOfWork<DbContext> unitOfWork,
            ILogger<CourseRegistrationConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CourseRegistrationMessage> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Processing registration request - RequestId: {RequestId}, StudentId: {StudentId}",
                message.RequestId,
                message.StudentId
            );

            try
            {
                // Check if this registration already exists in the database
                var existingRegistration = await _unitOfWork.GetRepository<CourseRegistration>()
                    .SingleOrDefaultAsync(
                        predicate: r => r.StudentId == message.StudentId && 
                                        r.ClassSectionId == message.CourseOfferingId &&
                                        r.Status != "Dropped"
                    );

                // If registration already exists, don't process again, just respond with success
                if (existingRegistration != null)
                {
                    _logger.LogInformation(
                        "Registration already exists - RequestId: {RequestId}, RegistrationId: {RegistrationId}",
                        message.RequestId,
                        existingRegistration.Id
                    );

                    await context.RespondAsync(new CourseRegistrationResult
                    {
                        RequestId = message.RequestId,
                        Success = true,
                        Status = existingRegistration.Status,
                        Message = $"Already {existingRegistration.Status.ToLower()} for the course"
                    });
                    
                    return;
                }

                // Try to acquire semaphore with timeout
                if (!await _semaphore.WaitAsync(TimeSpan.FromSeconds(30)))
                {
                    _logger.LogWarning(
                        "Registration request timed out waiting for semaphore - RequestId: {RequestId}",
                        message.RequestId
                    );
                    
                    await context.RespondAsync(new CourseRegistrationResult
                    {
                        RequestId = message.RequestId,
                        Success = false,
                        Message = "Registration request timed out due to high load"
                    });
                    
                    return;
                }

                try
                {
                    // Get latest class section data with lock
                    var classSection = await _unitOfWork.GetRepository<ClassSection>()
                        .SingleOrDefaultAsync(
                            predicate: cs => cs.Id == message.CourseOfferingId,
                            include: q => q
                                .Include(cs => cs.Course)
                                .Include(cs => cs.CourseRegistrations)
                        );

                    if (classSection == null)
                    {
                        throw new BadHttpRequestException("Class section not found");
                    }

                    // Check capacity with actual count
                    var currentRegistrations = classSection.CourseRegistrations
                        .Count(r => r.Status != "Dropped");

                    // If no available slots, return failure instead of waitlisting
                    if (currentRegistrations >= classSection.MaxCapacity)
                    {
                        _logger.LogInformation(
                            "Registration failed - No available slots - RequestId: {RequestId}",
                            message.RequestId
                        );

                        await context.RespondAsync(new CourseRegistrationResult
                        {
                            RequestId = message.RequestId,
                            Success = false,
                            Message = "No available slots for this course. Registration failed."
                        });
                        
                        return;
                    }

                    // Get current registration period
                    var currentPeriod = await _unitOfWork.GetRepository<RegistrationPeriod>()
                        .SingleOrDefaultAsync(
                            predicate: rp => rp.Status == "Open" &&
                                           rp.StartDate <= DateTime.UtcNow &&
                                           rp.EndDate >= DateTime.UtcNow
                        );

                    if (currentPeriod == null)
                    {
                        throw new BadHttpRequestException("No active registration period found");
                    }

                    var registration = new CourseRegistration
                    {
                        Id = Guid.NewGuid(),
                        StudentId = message.StudentId,
                        ClassSectionId = message.CourseOfferingId,
                        RegistrationDate = DateTime.UtcNow,
                        Status = "Registered",
                        TuitionStatus = "Pending",
                        RegistrationPeriodId = currentPeriod.Id
                    };

                    await _unitOfWork.GetRepository<CourseRegistration>().InsertAsync(registration);
                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation(
                        "Registration successful - RequestId: {RequestId}, Status: {Status}",
                        message.RequestId,
                        registration.Status
                    );

                    // Use RespondAsync instead of Publish to directly respond to the request
                    await context.RespondAsync(new CourseRegistrationResult
                    {
                        RequestId = message.RequestId,
                        Success = true,
                        Status = registration.Status,
                        Message = "Successfully registered for the course"
                    });
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing course registration - RequestId: {RequestId}",
                    message.RequestId
                );

                // Use RespondAsync for error responses as well
                await context.RespondAsync(new CourseRegistrationResult
                {
                    RequestId = message.RequestId,
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
