using AutoMapper;
using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;
using CourseRegistration_API.Services.Interface;
using CourseRegistration_Domain.Entities;
using CourseRegistration_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseRegistration_API.Services.Implements
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
			var courseOfferings = await _unitOfWork.GetRepository<ClassSection>()
				.GetListAsync(
					predicate: co => co.SemesterId == termId, // Change TermId to SemesterId
					include: q => q
						.Include(co => co.Course)
						.Include(co => co.CourseEvaluations)
				);

			return courseOfferings
				.GroupBy(co => new { co.Course.CourseCode, co.Course.CourseName })
				.Select(group => new CourseEvaluationSummaryResponse
				{
					CourseCode = group.Key.CourseCode,
					CourseName = group.Key.CourseName,
					AverageRating = group.SelectMany(co => co.CourseEvaluations).Any()
						? group.SelectMany(co => co.CourseEvaluations).Average(ce => ce.Rating)
						: 0,
					TotalEvaluations = group.SelectMany(co => co.CourseEvaluations).Count(),
					Comments = group.SelectMany(co => co.CourseEvaluations)
						.Where(ce => !string.IsNullOrEmpty(ce.Comments))
						.Select(ce => ce.Comments)
						.ToList()
				}).ToList();
		}

		public async Task<bool> CreateCourseEvaluation(CourseEvaluationCreateRequest request)
		{
			try
			{
				var courseOffering = await _unitOfWork.GetRepository<ClassSection>()
					.SingleOrDefaultAsync(predicate: co => co.Id == request.CourseOfferingId);

				if (courseOffering == null)
					throw new BadHttpRequestException("Course offering not found");

				// Check if student has already evaluated this course
				var existingEvaluation = await _unitOfWork.GetRepository<CourseEvaluation>()
					.SingleOrDefaultAsync(predicate: ce =>
						ce.ClassSectionId == request.CourseOfferingId && // Change CourseOfferingId to ClassSectionId
						ce.StudentId == request.StudentId);

				if (existingEvaluation != null)
					throw new BadHttpRequestException("You have already evaluated this course");

				// Create new evaluation
				var evaluation = new CourseEvaluation
				{
					Id = Guid.NewGuid(),
					ClassSectionId = request.CourseOfferingId, // Change CourseOfferingId to ClassSectionId
					StudentId = request.StudentId,
					Rating = request.Rating,
					Comments = request.Comments,
					EvaluationDate = DateTime.Now
				};

				await _unitOfWork.GetRepository<CourseEvaluation>().InsertAsync(evaluation);
				await _unitOfWork.CommitAsync();


				return true;
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Error creating course evaluation: {Message}", ex.Message);
				throw;
			}
		}
	}
}