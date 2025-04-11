using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
{
    public interface ICourseOfferingService
    {
        Task<List<CourseOfferingResponse>> CreateCourseOfferings(CourseOfferingCreateRequest request);
        Task<List<CourseOfferingResponse>> GetOfferingsBySemester(Guid semesterId);
        Task<CourseOfferingResponse> GetOfferingById(Guid offeringId);
        Task<CourseOfferingResponse> UpdateOffering(Guid offeringId, CourseOfferingUpdateRequest request);
        Task<bool> DeleteOffering(Guid offeringId);
    }
}
