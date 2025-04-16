using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

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
		Task<CourseEvaluationPeriodResponse> GetCurrentEvaluationPeriod();
		Task<List<CourseEvaluationSummaryResponse>> GetCourseEvaluationSummaries(Guid semesterId);
		Task<List<ProfessorEvaluationSummaryResponse>> GetProfessorEvaluationSummaries(Guid semesterId);
		Task<bool> SendEvaluationResultsToProfessors(Guid semesterId);
		Task<byte[]> ExportCourseEvaluations(Guid semesterId);
		Task<byte[]> ExportProfessorEvaluations(Guid semesterId);
	}
}
