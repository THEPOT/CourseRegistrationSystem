using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
{
	public interface IDegreeAuditService
	{
		Task<DegreeAuditResponse> GetStudentDegreeAudit(Guid studentId);
		Task<DegreeAuditResponse> UpdateStudentDegreeAudit(Guid studentId, DegreeAuditUpdateRequest request);
		Task<byte[]> ExportDegreeAuditPdf(Guid studentId);
	}
}
