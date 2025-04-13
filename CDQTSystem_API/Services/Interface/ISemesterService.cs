using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
{
    public interface ISemesterService
    {
        Task<SemesterResponse> CreateSemester(SemesterCreateRequest request);
        Task<List<SemesterResponse>> GetAllSemesters();
        Task<SemesterResponse> GetSemesterById(Guid id);
        Task<bool> UpdateSemesterStatus(Guid id, string status);
    }
}