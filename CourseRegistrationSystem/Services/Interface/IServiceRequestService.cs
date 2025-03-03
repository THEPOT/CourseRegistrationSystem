using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;

namespace CourseRegistration_API.Services.Interface
{
	public interface IServiceRequestService
	{
		Task<List<ServiceRequestResponse>> GetAllServiceRequests(string status = null);
		Task<List<ServiceRequestResponse>> GetStudentServiceRequests(Guid studentId, string status = null);
		Task<ServiceRequestResponse> GetServiceRequestById(Guid id);
		Task<ServiceRequestResponse> CreateServiceRequest(ServiceRequestCreateRequest request);
		Task<ServiceRequestResponse> UpdateServiceRequestStatus(Guid id, ServiceRequestStatusUpdateRequest request);
		Task<bool> DeleteServiceRequest(Guid id);
	}
}
