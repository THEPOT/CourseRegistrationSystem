using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;

namespace CourseRegistration_API.Services.Interface
{
	public interface ICourseEvaluationService
	{
		Task<List<CourseOfferingForEvaluationResponse>> GetCourseOfferingsForEvaluation(Guid termId);
		Task<List<CourseEvaluationSummaryResponse>> GetCourseEvaluationSummaries(Guid termId);
		Task<bool> CreateCourseEvaluation(CourseEvaluationCreateRequest request);
	}
}
