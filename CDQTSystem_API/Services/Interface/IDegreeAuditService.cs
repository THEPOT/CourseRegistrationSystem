using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
{
	public interface IDegreeAuditService
	{
		Task<DegreeAuditResponse> GetStudentDegreeAudit(Guid studentId);
		Task<DegreeAuditResponse> UpdateStudentDegreeAudit(Guid studentId, DegreeAuditUpdateRequest request);
		Task<byte[]> ExportDegreeAuditPdf(Guid studentId);
		Task<ProgramRequirementsResponse> GetProgramRequirements(Guid programId);
		Task<bool> UpdateProgramRequirements(Guid programId, ProgramRequirementsUpdateRequest request);
		Task<BatchProgressResponse> GetBatchProgress(BatchProgressFilterRequest filter);
		Task<StudentProgressResponse> GetStudentProgress(Guid userId);
		Task<byte[]> ExportAuditReport(AuditExportRequest filter);
	}
}
