// CourseRegistrationSystem/Services/Implements/MidtermEvaluationService.cs
using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CDQTSystem_Domain.Paginate;
namespace CDQTSystem_API.Services.Implements
{
	public class MidtermEvaluationService : BaseService<MidtermEvaluationService>, IMidtermEvaluationService
	{
		public MidtermEvaluationService(IUnitOfWork<DbContext> unitOfWork, ILogger<MidtermEvaluationService> logger,
						  IMapper mapper, IHttpContextAccessor httpContextAccessor)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}


		public async Task<MidtermEvaluationResponse> CreateMidtermEvaluation(CreateMidtermEvaluationRequest request)
		{
			var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
				throw new BadHttpRequestException("User is not authenticated");

			var professor = await _unitOfWork.GetRepository<Professor>()
				.SingleOrDefaultAsync(
					predicate: p => p.UserId == Guid.Parse(userId)
				);

			if (professor == null)
				throw new BadHttpRequestException("Only professors can create midterm evaluations");

			var professorId = professor.Id;

			var classSection = await _unitOfWork.GetRepository<ClassSection>()
				.SingleOrDefaultAsync(
					predicate: cs => cs.Id == request.ClassSectionId,
					include: q => q
						.Include(cs => cs.Course)
						.Include(cs => cs.Semester)
				);

			if (classSection == null)
				throw new BadHttpRequestException("Class section not found");

			// Check if professor teaches this class
			var professorTeachesClass = classSection.ProfessorId == professorId;

			// With this corrected code:
			var studentEnrolled = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: cr => cr.StudentId == request.StudentId &&
									 cr.ClassSectionId == request.ClassSectionId &&
									 cr.Status != "Dropped"
				);

			if (!studentEnrolled.Any())
				throw new BadHttpRequestException("Student is not enrolled in this class");
			if (!professorTeachesClass)
				throw new BadHttpRequestException("You are not authorized to evaluate students in this class");

			// Check if evaluation period is open
			var evaluationPeriod = await GetCurrentEvaluationPeriod();
			if (evaluationPeriod == null || !evaluationPeriod.IsCurrentlyOpen)
				throw new BadHttpRequestException("Midterm evaluation period is not currently open");


			// Check if evaluation already exists
			var existingEvaluation = await _unitOfWork.GetRepository<MidtermEvaluation>()
				.SingleOrDefaultAsync(
					predicate: me => me.StudentId == request.StudentId &&
									 me.ClassSectionId == request.ClassSectionId
				);

			if (existingEvaluation != null)
				throw new BadHttpRequestException("An evaluation already exists for this student in this class");

			var evaluation = new MidtermEvaluation
			{
				Id = Guid.NewGuid(),
				StudentId = request.StudentId,
				ProfessorId = professorId,
				CourseId = classSection.CourseId,
				ClassSectionId = request.ClassSectionId,
				SemesterId = classSection.SemesterId,
				Comments = request.Comments,
				Recommendation = request.Recommendation,
				EvaluationDate = DateTime.UtcNow.AddHours(7)
			};

			await _unitOfWork.GetRepository<MidtermEvaluation>().InsertAsync(evaluation);
			await _unitOfWork.CommitAsync();

			// Map to response
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(
					predicate: s => s.Id == request.StudentId
				);
			var user = await _unitOfWork.GetRepository<User>()
				.SingleOrDefaultAsync(
					predicate: u => u.Id == student.UserId
				);
			var professorUser = await _unitOfWork.GetRepository<User>()
				.SingleOrDefaultAsync(
					predicate: u => u.Id == professor.UserId
				);

			return new MidtermEvaluationResponse
			{
				Id = evaluation.Id,
				StudentId = evaluation.StudentId,
				StudentName = user?.FullName ?? string.Empty,
				StudentMssv = student?.User.UserCode ?? string.Empty,
				ProfessorId = evaluation.ProfessorId,
				ProfessorName = professorUser.FullName,
				CourseId = evaluation.CourseId,
				CourseCode = classSection.Course?.CourseCode ?? string.Empty,
				CourseName = classSection.Course?.CourseName ?? string.Empty,
				ClassSectionId = evaluation.ClassSectionId,
				SemesterId = evaluation.SemesterId,
				SemesterName = classSection.Semester?.SemesterName ?? string.Empty,
				Comments = evaluation.Comments,
				Recommendation = evaluation.Recommendation,
				EvaluationDate = evaluation.EvaluationDate
			};
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

		public Task<MidtermEvaluationResponse> UpdateMidtermEvaluation(Guid evaluationId, UpdateMidtermEvaluationRequest request)
		{
			throw new NotImplementedException();
		}

		public Task<List<MidtermEvaluationResponse>> GetMidtermEvaluationsByProfessor(Guid professorId, Guid semesterId)
		{
			throw new NotImplementedException();
		}

		public Task<List<MidtermEvaluationResponse>> GetStudentMidtermEvaluations(Guid studentId, Guid? semesterId = null)
		{
			throw new NotImplementedException();
		}

		public Task<MidtermEvaluationPeriodResponse> CreateOrUpdateEvaluationPeriod(MidtermEvaluationPeriodRequest request)
		{
			throw new NotImplementedException();
		}

		public Task<MidtermEvaluationPeriodResponse> GetCurrentEvaluationPeriod()
		{
			throw new NotImplementedException();
		}

		Task<List<MidtermEvaluationSummaryResponse>> IMidtermEvaluationService.GetMidtermEvaluationSummary(Guid semesterId)
		{
			throw new NotImplementedException();
		}

		public Task<byte[]> ExportMidtermEvaluations(Guid semesterId)
		{
			throw new NotImplementedException();
		}

		public Task<MidtermEvaluationResponse> GetMidtermEvaluation(Guid evaluationId)
		{
			throw new NotImplementedException();
		}
		public async Task<IPaginate<MidtermEvaluationResponse>> GetMidtermEvaluations(int page = 1, int size = 10)
		{
			var evaluations = await _unitOfWork.GetRepository<MidtermEvaluation>()
				.GetPagingListAsync(
				selector: e => new MidtermEvaluationResponse
				{
					Id = e.Id,
					StudentId = e.StudentId,
					ProfessorId = e.ProfessorId,
					CourseId = e.CourseId,
					ClassSectionId = e.ClassSectionId,
					SemesterId = e.SemesterId,
					Comments = e.Comments,
					Recommendation = e.Recommendation,
					EvaluationDate = e.EvaluationDate
				},
				predicate: null,
				page: page,
				size: size);
			return evaluations;
		}
	}
}