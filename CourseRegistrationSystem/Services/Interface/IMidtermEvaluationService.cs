using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;

namespace CourseRegistration_API.Services.Interface
{
	public interface IMidtermEvaluationService
	{
		Task<List<StudentForEvaluationResponse>> GetStudentsForEvaluation(Guid courseOfferingId);
		Task<MidtermEvaluationSummaryResponse> GetMidtermEvaluationSummary(Guid courseOfferingId);
		Task<bool> CreateMidtermEvaluation(MidtermEvaluationCreateRequest request);
		Task<bool> CreateBatchMidtermEvaluations(MidtermEvaluationBatchRequest request);
		Task<bool> UpdateMidtermEvaluation(Guid evaluationId, MidtermEvaluationCreateRequest request);
	}
}
