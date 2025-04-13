using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
{
	public interface IDepartmentService
	{
		Task<List<DepartmentResponse>> GetAllDepartments();
		Task<DepartmentResponse> GetDepartmentById(Guid departmentId);
		Task<DepartmentResponse> CreateDepartment(DepartmentCreateRequest request);
		Task<DepartmentResponse> UpdateDepartment(Guid departmentId, DepartmentUpdateRequest request);

	}
}
