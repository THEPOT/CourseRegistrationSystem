using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_Domain.Paginate;

namespace CDQTSystem_API.Services.Interface
{
	public interface IServiceRequestService
	{
		Task<IPaginate<ServiceRequestResponse>> GetAllServiceRequests(string status = null, int page = 1, int size = 10, string search = null);
		Task<List<ServiceRequestResponse>> GetStudentServiceRequests(Guid studentId, string status = null);
		Task<ServiceRequestResponse> GetServiceRequestById(Guid id);
		Task<ServiceRequestResponse> CreateServiceRequest(ServiceRequestCreateRequest request);
		Task<ServiceRequestResponse> UpdateServiceRequestStatus(Guid id, ServiceRequestStatusUpdateRequest request);
		Task<bool> DeleteServiceRequest(Guid id);
		Task<ServiceRequestStatisticsResponse> GetStatistics(); 
	}
}
