using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_Domain.Paginate;

namespace CDQTSystem_API.Services.Interface
{
	public interface IMidtermEvaluationService
	{
		Task<MidtermEvaluationResponse> CreateMidtermEvaluation(CreateMidtermEvaluationRequest request);
		Task<MidtermEvaluationResponse> UpdateMidtermEvaluation(Guid evaluationId, UpdateMidtermEvaluationRequest request);
		Task<List<MidtermEvaluationResponse>> GetMidtermEvaluationsByProfessor(Guid professorId, Guid semesterId);
		Task<List<MidtermEvaluationResponse>> GetStudentMidtermEvaluations(Guid userId, Guid? semesterId = null);
		Task<MidtermEvaluationPeriodResponse> CreateOrUpdateEvaluationPeriod(MidtermEvaluationPeriodRequest request);
		Task<MidtermEvaluationPeriodResponse> GetCurrentEvaluationPeriod();
		Task<List<MidtermEvaluationSummaryResponse>> GetMidtermEvaluationSummary(Guid semesterId);
		Task<byte[]> ExportMidtermEvaluations(Guid semesterId);
		Task<MidtermEvaluationResponse> GetMidtermEvaluation(Guid evaluationId);
		Task<IPaginate<MidtermEvaluationResponse>> GetMidtermEvaluations(int page, int size);
	}
}
