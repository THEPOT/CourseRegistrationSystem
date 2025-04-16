using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_Domain.Paginate;

namespace CDQTSystem_API.Services.Interface
{
    public interface ISemesterService
    {
        Task<SemesterResponse> CreateSemester(SemesterCreateRequest request);
		Task<IPaginate<SemesterResponse>> GetAllSemesters(int page, int size, string? search);
        Task<SemesterResponse> GetSemesterById(Guid id);
        Task<bool> UpdateSemesterStatus(Guid id, string status);
    }
}