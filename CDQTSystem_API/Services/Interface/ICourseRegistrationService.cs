using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
{
	public interface ICourseRegistrationService
	{
		Task<List<AvailableCourseResponse>> GetAvailableCourseOfferings();
		Task<List<AvailableCourseResponse>> GetAvailableCourseOfferingsForStudent(Guid userId);
		Task<bool> CheckPrerequisites(Guid studentId, Guid courseId);
		Task<bool> RegisterCourse(CourseRegistrationRequest request, Guid userId);
		Task<List<CourseOfferingResponse>> GetStudentRegistrations(Guid studentId, Guid termId);
	}
}
