using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_Domain.Paginate;

namespace CDQTSystem_API.Services.Interface
{
	public interface ICourseEvaluationService
	{
		// Student actions
		Task<bool> SubmitCourseEvaluation(SubmitCourseEvaluationRequest request);
		Task<StudentEvaluationStatusResponse> GetStudentEvaluationStatus(Guid studentId, Guid semesterId);

		// Professor actions
		Task<List<CourseEvaluationSummaryResponse>> GetProfessorEvaluations(Guid professorId, Guid semesterId);

		// Admin actions
		Task<CourseEvaluationPeriodResponse> CreateOrUpdateEvaluationPeriod(CourseEvaluationPeriodRequest request);
		Task<IPaginate<CourseEvaluationPeriodResponse>> GetCourseOfferingsForEvaluation(Guid semesterId, int page, int size);
		Task<CourseEvaluationPeriodResponse> GetCurrentEvaluationPeriod();
		Task<List<CourseEvaluationSummaryResponse>> GetCourseEvaluationSummaries(Guid semesterId);
		Task<List<ProfessorEvaluationSummaryResponse>> GetProfessorEvaluationSummaries(Guid semesterId);
		Task<bool> SendEvaluationResultsToProfessors(Guid semesterId);
		Task<byte[]> ExportCourseEvaluations(Guid semesterId);
		Task<byte[]> ExportProfessorEvaluations(Guid semesterId);
	}
}
