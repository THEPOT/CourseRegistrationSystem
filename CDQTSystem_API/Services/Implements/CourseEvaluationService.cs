using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
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
			throw new NotImplementedException();
		}

		public async Task<bool> CreateCourseEvaluation(CourseEvaluationCreateRequest request)
		{
			throw new NotImplementedException();
		}
		public async Task<bool> SubmitCourseEvaluation(SubmitCourseEvaluationRequest request)
		{
			throw new NotImplementedException();
		}

		public Task<StudentEvaluationStatusResponse> GetStudentEvaluationStatus(Guid studentId, Guid semesterId)
		{
			throw new NotImplementedException();
		}

		public Task<List<CourseEvaluationSummaryResponse>> GetProfessorEvaluations(Guid professorId, Guid semesterId)
		{
			throw new NotImplementedException();
		}

		public Task<CourseEvaluationPeriodResponse> CreateOrUpdateEvaluationPeriod(CourseEvaluationPeriodRequest request)
		{
			throw new NotImplementedException();
		}

		public Task<CourseEvaluationPeriodResponse> GetCurrentEvaluationPeriod()
		{
			throw new NotImplementedException();
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
	}
}