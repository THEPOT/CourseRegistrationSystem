using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_API.Services.Implements
{
	public class GradesService : IGradesService
	{
		private readonly IUnitOfWork<DbContext> _unitOfWork;
		private readonly ILogger<GradesService> _logger;

		public GradesService(IUnitOfWork<DbContext> unitOfWork, ILogger<GradesService> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task<GradeResponse> CreateOrUpdateGrade(GradeEntryRequest request)
		{
			// Tìm CourseRegistration
			var registration = await _unitOfWork.GetRepository<CourseRegistration>()
				.SingleOrDefaultAsync(
				predicate: r => r.Id == request.CourseRegistrationId
				);

			if (registration == null)
				throw new BadHttpRequestException("Course registration not found");

			// Tìm hoặc tạo Grade
			var grade = await _unitOfWork.GetRepository<Grade>()
				.SingleOrDefaultAsync(predicate: g => g.CourseRegistrationId == request.CourseRegistrationId);

			if (grade == null)
			{
				grade = new Grade
				{
					Id = Guid.NewGuid(),
					CourseRegistrationId = request.CourseRegistrationId,
					QualityPoints = request.QualityPoints,
					GradeValue = request.GradeValue,
				};
				await _unitOfWork.GetRepository<Grade>().InsertAsync(grade);
			}
			else
			{
				grade.QualityPoints = request.QualityPoints;
				grade.GradeValue = request.GradeValue;
				_unitOfWork.GetRepository<Grade>().UpdateAsync(grade);
			}

			await _unitOfWork.CommitAsync();

			return new GradeResponse
			{
				Id = grade.Id,
				CourseRegistrationId = grade.CourseRegistrationId,
				QualityPoints = grade.QualityPoints,
				GradeValue = grade.GradeValue,
			};
		}
	}
}
