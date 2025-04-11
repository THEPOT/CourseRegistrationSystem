using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
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
