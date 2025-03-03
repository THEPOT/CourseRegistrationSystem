using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;

namespace CourseRegistration_API.Services.Interface
{
	public interface IDegreeAuditService
	{
		Task<DegreeAuditResponse> GetStudentDegreeAudit(Guid studentId);
		Task<DegreeAuditResponse> UpdateStudentDegreeAudit(Guid studentId, DegreeAuditUpdateRequest request);
		Task<byte[]> ExportDegreeAuditPdf(Guid studentId);
	}
}
