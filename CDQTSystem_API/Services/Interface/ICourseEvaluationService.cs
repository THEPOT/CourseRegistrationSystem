using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
{
	public interface ICourseEvaluationService
	{
		Task<List<CourseOfferingForEvaluationResponse>> GetCourseOfferingsForEvaluation(Guid termId);
		Task<List<CourseEvaluationSummaryResponse>> GetCourseEvaluationSummaries(Guid termId);
		Task<bool> CreateCourseEvaluation(CourseEvaluationCreateRequest request);
	}
}
