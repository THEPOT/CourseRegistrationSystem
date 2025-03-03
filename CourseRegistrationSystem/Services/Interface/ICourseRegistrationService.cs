using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;

namespace CourseRegistration_API.Services.Interface
{
	public interface ICourseRegistrationService
	{
		Task<List<CourseOfferingResponse>> GetTermCourseOfferings(Guid termId, Guid? studentId = null);
		Task<bool> CheckPrerequisites(Guid studentId, Guid courseId);
		Task<List<CourseRegistrationSummaryResponse>> GetRegistrationSummaryByTerm(Guid termId);
		Task<bool> RegisterCourse(CourseRegistrationRequest request);
		Task<List<bool>> RegisterCourses(BatchCourseRegistrationRequest request);
		Task<bool> UpdateRegistration(Guid registrationId, CourseRegistrationUpdateRequest request);
		Task<bool> CancelRegistration(Guid registrationId);
		Task<List<CourseOfferingResponse>> GetStudentRegistrations(Guid studentId, Guid termId);
		Task<List<StudentInfoResponse>> GetCourseStudents(Guid courseOfferingId);
	}
}
