using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Domain.Paginate;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CDQTSystem_API.Services.Implements
{
	public class CourseEvaluationService : BaseService<CourseEvaluationService>, ICourseEvaluationService
	{
		public CourseEvaluationService(IUnitOfWork<DbContext> unitOfWork, ILogger<CourseEvaluationService> logger,
						  IMapper mapper, IHttpContextAccessor httpContextAccessor)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public async Task<List<CourseOfferingForEvaluationResponse>> GetCourseOfferingsForEvaluation(Guid termId)
		{
			var courseOfferings = await _unitOfWork.GetRepository<ClassSection>()
				.GetListAsync(
					predicate: co => co.SemesterId == termId,
					include: q => q
						.Include(co => co.Course)
						.Include(co => co.Professor)
							.ThenInclude(l => l.User)
				);

			return courseOfferings.Select(co => new CourseOfferingForEvaluationResponse
			{
				CourseOfferingId = co.Id,
				CourseCode = co.Course.CourseCode,
				CourseName = co.Course.CourseName,
				ProfessorId = co.ProfessorId ?? Guid.Empty,
				ProfessorName = co.Professor.User.FullName
			}).ToList();
		}

		public async Task<List<CourseEvaluationSummaryResponse>> GetCourseEvaluationSummaries(Guid termId)
		{
			// Lấy tất cả các đánh giá theo học kỳ
			var evaluations = await _unitOfWork.GetRepository<CourseEvaluation>()
				.GetListAsync(
					predicate: e => e.SemesterId == termId,
					include: q => q.Include(e => e.Course).Include(e => e.Professor).ThenInclude(p => p.User)
				);

			var grouped = evaluations.GroupBy(e => e.CourseId);
			var result = new List<CourseEvaluationSummaryResponse>();
			foreach (var group in grouped)
			{
				var first = group.First();
				result.Add(new CourseEvaluationSummaryResponse
				{
					CourseId = first.CourseId,
					CourseCode = first.Course.CourseCode,
					CourseName = first.Course.CourseName,
					ProfessorId = first.ProfessorId,
					ProfessorName = first.Professor.User.FullName,
					TotalResponses = group.Count(),
					AverageTeachingQuality = 5, // TODO: Tính trung bình thực tế nếu có trường
					AverageCourseContent = 5,
					AverageAssessmentFairness = 5,
					AverageOverallSatisfaction = 5,
					CommonFeedback = group.Select(e => e.Comments).Where(c => !string.IsNullOrEmpty(c)).Take(5).ToList()
				});
			}
			return result;
		}

		public async Task<bool> CreateCourseEvaluation(CourseEvaluationCreateRequest request)
		{
			// Tạo mới một đánh giá môn học
			var classSection = await _unitOfWork.GetRepository<ClassSection>()
				.SingleOrDefaultAsync(predicate: cs => cs.Id == request.CourseOfferingId,
					include: q => q.Include(cs => cs.Course).Include(cs => cs.Professor));
			if (classSection == null || classSection.Professor == null)
				return false;

			var evaluation = new CourseEvaluation
			{
				Id = Guid.NewGuid(),
				StudentId = request.StudentId,
				ProfessorId = classSection.ProfessorId.Value,
				CourseId = classSection.CourseId,
				SemesterId = classSection.SemesterId,
				EvaluationDate = DateTime.UtcNow,
				OverallSatisfaction = request.OverallSatisfactionRating.ToString(),
				Comments = request.Feedback
			};
			await _unitOfWork.GetRepository<CourseEvaluation>().InsertAsync(evaluation);
			await _unitOfWork.CommitAsync();
			return true;
		}

		public async Task<bool> SubmitCourseEvaluation(SubmitCourseEvaluationRequest request)
		{
			// Tìm class section
			var classSection = await _unitOfWork.GetRepository<ClassSection>()
				.SingleOrDefaultAsync(predicate: cs => cs.Id == request.ClassSectionId,
					include: q => q.Include(cs => cs.Course).Include(cs => cs.Professor));
			if (classSection == null || classSection.Professor == null)
				return false;

			// TODO: Lấy StudentId từ context hoặc request nếu cần
			var evaluation = new CourseEvaluation
			{
				Id = Guid.NewGuid(),
				StudentId = Guid.Empty, // Cần truyền vào hoặc lấy từ context
				ProfessorId = classSection.ProfessorId.Value,
				CourseId = classSection.CourseId,
				SemesterId = classSection.SemesterId,
				EvaluationDate = DateTime.UtcNow,
				OverallSatisfaction = request.OverallSatisfactionRating.ToString(),
				Comments = request.Feedback
			};
			await _unitOfWork.GetRepository<CourseEvaluation>().InsertAsync(evaluation);
			await _unitOfWork.CommitAsync();
			return true;
		}

		public async Task<StudentEvaluationStatusResponse> GetStudentEvaluationStatus(Guid studentId, Guid semesterId)
		{
			// Lấy tất cả class section mà sinh viên đã đăng ký trong học kỳ
			var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync( predicate: r => r.StudentId == studentId && r.ClassSection.SemesterId == semesterId,
					include: q => q.Include(r => r.ClassSection).ThenInclude(cs => cs.Course).Include(r => r.ClassSection).ThenInclude(cs => cs.Professor).ThenInclude(p => p.User));

			// Lấy tất cả đánh giá đã nộp
			var evaluations = await _unitOfWork.GetRepository<CourseEvaluation>()
				.GetListAsync(predicate: e => e.StudentId == studentId && e.SemesterId == semesterId);

			var courses = registrations.Select(r => new CourseEvaluationStatus
			{
				ClassSectionId = r.ClassSection.Id,
				CourseId = r.ClassSection.CourseId,
				CourseCode = r.ClassSection.Course.CourseCode,
				CourseName = r.ClassSection.Course.CourseName,
				ProfessorId = r.ClassSection.ProfessorId ?? Guid.Empty,
				ProfessorName = r.ClassSection.Professor?.User.FullName ?? "",
				HasCompleted = evaluations.Any(e => e.CourseId == r.ClassSection.CourseId),
				CompletionDate = evaluations.FirstOrDefault(e => e.CourseId == r.ClassSection.CourseId)?.EvaluationDate
			}).ToList();

			return new StudentEvaluationStatusResponse
			{
				SemesterId = semesterId,
				SemesterName = registrations.FirstOrDefault()?.ClassSection.Semester.SemesterName ?? "",
				Courses = courses
			};
		}

		public async Task<List<CourseEvaluationSummaryResponse>> GetProfessorEvaluations(Guid professorId, Guid semesterId)
		{
			var evaluations = await _unitOfWork.GetRepository<CourseEvaluation>()
				.GetListAsync(predicate: e => e.ProfessorId == professorId && e.SemesterId == semesterId,
					include: q => q.Include(e => e.Course).Include(e => e.Professor).ThenInclude(p => p.User));

			var grouped = evaluations.GroupBy(e => e.CourseId);
			var result = new List<CourseEvaluationSummaryResponse>();
			foreach (var group in grouped)
			{
				var first = group.First();
				result.Add(new CourseEvaluationSummaryResponse
				{
					CourseId = first.CourseId,
					CourseCode = first.Course.CourseCode,
					CourseName = first.Course.CourseName,
					ProfessorId = first.ProfessorId,
					ProfessorName = first.Professor.User.FullName,
					TotalResponses = group.Count(),
					AverageTeachingQuality = 5, // TODO: Tính trung bình thực tế nếu có trường
					AverageCourseContent = 5,
					AverageAssessmentFairness = 5,
					AverageOverallSatisfaction = 5,
					CommonFeedback = group.Select(e => e.Comments).Where(c => !string.IsNullOrEmpty(c)).Take(5).ToList()
				});
			}
			return result;
		}

		public async Task<CourseEvaluationPeriodResponse> CreateOrUpdateEvaluationPeriod(CourseEvaluationPeriodRequest request)
		{
			var repo = _unitOfWork.GetRepository<CourseEvaluationPeriod>();
			var period = await repo.SingleOrDefaultAsync(
				predicate: p => p.SemesterId == request.SemesterId
			);

			if (period == null)
			{
				period = new CourseEvaluationPeriod
				{
					Id = Guid.NewGuid(),
					SemesterId = request.SemesterId,
					StartDate = request.StartDate,
					EndDate = request.EndDate,
					Status = request.IsActive ? "ACTIVE" : "INACTIVE"
				};
				await repo.InsertAsync(period);
			}
			else
			{
				period.StartDate = request.StartDate;
				period.EndDate = request.EndDate;
				period.Status = request.IsActive ? "ACTIVE" : "INACTIVE";
				repo.UpdateAsync(period);
			}

			await _unitOfWork.CommitAsync();

			// Lấy lại period kèm Semester
			period = await repo.SingleOrDefaultAsync(
				predicate: p => p.Id == period.Id,
				include: q => q.Include(p => p.Semester)
			);

			return new CourseEvaluationPeriodResponse
			{
				Id = period.Id,
				SemesterId = period.SemesterId,
				SemesterName = period.Semester?.SemesterName ?? string.Empty,
				StartDate = period.StartDate,
				EndDate = period.EndDate,
				IsActive = period.Status == "ACTIVE"
			};
		}

		public async Task<CourseEvaluationPeriodResponse> GetCurrentEvaluationPeriod()
		{
			var now = DateTime.UtcNow;
			var period = await _unitOfWork.GetRepository<CourseEvaluationPeriod>()
				.GetListAsync(
					predicate: p => p.StartDate <= now && p.EndDate >= now && p.Status == "ACTIVE",
					include: q => q.Include(p => p.Semester)
				);

			var current = period.FirstOrDefault();
			if (current == null) return null;

			return new CourseEvaluationPeriodResponse
			{
				Id = current.Id,
				SemesterId = current.SemesterId,
				SemesterName = current.Semester?.SemesterName ?? string.Empty,
				StartDate = current.StartDate,
				EndDate = current.EndDate,
				IsActive = current.Status == "ACTIVE",
				IsCurrentlyOpen = current.StartDate <= now && current.EndDate >= now
			};
		}

		public Task<List<ProfessorEvaluationSummaryResponse>> GetProfessorEvaluationSummaries(Guid semesterId)
		{
			throw new NotImplementedException();
		}

		public Task<bool> SendEvaluationResultsToProfessors(Guid semesterId)
		{
			throw new NotImplementedException();
		}

		public Task<byte[]> ExportCourseEvaluations(Guid semesterId)
		{
			throw new NotImplementedException();
		}

		public Task<byte[]> ExportProfessorEvaluations(Guid semesterId)
		{
			throw new NotImplementedException();
		}

		public async Task<IPaginate<CourseEvaluationPeriodResponse>> GetCourseOfferingsForEvaluation(Guid semesterId, int page, int size)
		{
			var courseOfferings = await _unitOfWork.GetRepository<CourseEvaluationPeriod>()
				.GetPagingListAsync(
					selector: q => new CourseEvaluationPeriodResponse
					{
						Id = q.Id,
						SemesterId = q.SemesterId,
						StartDate = q.StartDate,
						EndDate = q.EndDate,
						IsActive = q.Status == "ACTIVE"
					},
					predicate: co => co.SemesterId == semesterId,
					orderBy: q => q.OrderBy(co => co.Id),
					page: page,
					size: size
				);
			return courseOfferings;
		}
	}
}