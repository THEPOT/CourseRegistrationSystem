// CourseRegistrationSystem/Services/Implements/MidtermEvaluationService.cs
using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_API.Services.Implements
{
	public class MidtermEvaluationService : BaseService<MidtermEvaluationService>, IMidtermEvaluationService
	{
		public MidtermEvaluationService(IUnitOfWork<DbContext> unitOfWork, ILogger<MidtermEvaluationService> logger,
						  IMapper mapper, IHttpContextAccessor httpContextAccessor)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public async Task<List<StudentForEvaluationResponse>> GetStudentsForEvaluation(Guid ClassSectionId)
		{
			// Get all registrations for the course offering
			var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: r => r.ClassSectionId == ClassSectionId,
					include: q => q
						.Include(r => r.Student)
							.ThenInclude(s => s.User)
						.Include(r => r.MidtermEvaluations)
				);

			// Map to response
			return registrations.Select(r => new StudentForEvaluationResponse
			{
				StudentId = r.StudentId,
				Mssv = r.Student.Mssv,
				FullName = r.Student.User.FullName,
				HasExistingEvaluation = r.MidtermEvaluations.Any(),
				ExistingStatus = r.MidtermEvaluations.FirstOrDefault()?.Status
			}).ToList();
		}

		public async Task<MidtermEvaluationSummaryResponse> GetMidtermEvaluationSummary(Guid ClassSectionId)
		{
			// Get the course offering with details
			var ClassSection = await _unitOfWork.GetRepository<ClassSection>()
				.SingleOrDefaultAsync(
					predicate: co => co.Id == ClassSectionId,
					include: q => q
						.Include(co => co.Course)
						.Include(co => co.Semester)
				);

			if (ClassSection == null)
				throw new BadHttpRequestException("Course offering not found");

			// Get all registrations with midterm evaluations
			var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: r => r.ClassSectionId == ClassSectionId,
					include: q => q
						.Include(r => r.Student)
							.ThenInclude(s => s.User)
						.Include(r => r.MidtermEvaluations)
				);

			var response = new MidtermEvaluationSummaryResponse
			{
				ClassSectionId = ClassSection.Id,
				CourseCode = ClassSection.Course.CourseCode,
				CourseName = ClassSection.Course.CourseName,
				TermName = ClassSection.Semester.SemesterName,
				StudentEvaluations = registrations
					.Where(r => r.MidtermEvaluations.Any())
					.Select(r => new StudentEvaluationDetail
					{
						StudentId = r.StudentId,
						Mssv = r.Student.Mssv,
						FullName = r.Student.User.FullName,
						Status = r.MidtermEvaluations.First().Status,
						Recommendation = r.MidtermEvaluations.First().Recommendation
					}).ToList()
			};

			return response;
		}

		public async Task<bool> CreateMidtermEvaluation(MidtermEvaluationCreateRequest request)
		{
			try
			{


				// Get the registration
				var registration = await _unitOfWork.GetRepository<CourseRegistration>()
					.SingleOrDefaultAsync(
						predicate: r => r.Id == request.CourseRegistrationId,
						include: q => q.Include(r => r.MidtermEvaluations)
					);

				if (registration == null)
					throw new BadHttpRequestException("Registration not found");

				// Check if evaluation already exists
				if (registration.MidtermEvaluations.Any())
					throw new BadHttpRequestException("Midterm evaluation already exists for this registration");

				// Create new evaluation
				var evaluation = new MidtermEvaluation
				{
					Id = Guid.NewGuid(),
					CourseRegistrationId = request.CourseRegistrationId,
					Status = request.Status,
					Recommendation = request.Recommendation
				};

				await _unitOfWork.GetRepository<MidtermEvaluation>().InsertAsync(evaluation);
				await _unitOfWork.CommitAsync();


				return true;
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Error creating midterm evaluation: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<bool> CreateBatchMidtermEvaluations(MidtermEvaluationBatchRequest request)
		{
			try
			{


				// Get all registrations for the course offering
				var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
					.GetListAsync(
						predicate: r => r.ClassSectionId == request.CourseOfferingId,
						include: q => q.Include(r => r.MidtermEvaluations)
					);

				foreach (var evalItem in request.Evaluations)
				{
					// Find matching registration
					var registration = registrations.FirstOrDefault(r => r.StudentId == evalItem.StudentId);

					if (registration == null)
						continue;

					// Check if evaluation already exists
					if (registration.MidtermEvaluations.Any())
					{
						// Update existing
						var existing = registration.MidtermEvaluations.First();
						existing.Status = evalItem.Status;
						existing.Recommendation = evalItem.Recommendation;
					}
					else
					{
						// Create new evaluation
						var evaluation = new MidtermEvaluation
						{
							Id = Guid.NewGuid(),
							CourseRegistrationId = registration.Id,
							Status = evalItem.Status,
							Recommendation = evalItem.Recommendation
						};

						await _unitOfWork.GetRepository<MidtermEvaluation>().InsertAsync(evaluation);
					}
				}

				await _unitOfWork.CommitAsync();


				return true;
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Error creating batch midterm evaluations: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<bool> UpdateMidtermEvaluation(Guid evaluationId, MidtermEvaluationCreateRequest request)
		{
			try
			{


				// Get the evaluation
				var evaluation = await _unitOfWork.GetRepository<MidtermEvaluation>()
					.SingleOrDefaultAsync(predicate: e => e.Id == evaluationId);

				if (evaluation == null)
					throw new BadHttpRequestException("Midterm evaluation not found");

				// Update properties
				evaluation.Status = request.Status;
				evaluation.Recommendation = request.Recommendation;

				await _unitOfWork.CommitAsync();

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating midterm evaluation: {Message}", ex.Message);
				throw;
			}
		}
	}
}