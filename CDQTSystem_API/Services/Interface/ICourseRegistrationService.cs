using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_Domain.Paginate;

namespace CDQTSystem_API.Services.Interface
{
	public interface ICourseRegistrationService
	{
		Task<IPaginate<AvailableCourseResponse>> GetAvailableCourseOfferings();
		Task<IPaginate<AvailableCourseResponse>> GetAvailableCourseOfferingsForStudent(Guid userId);
		Task<bool> CheckPrerequisites(Guid studentId, Guid courseId);
		Task<bool> RegisterCourse(CourseRegistrationRequest request, Guid userId);
		Task<IPaginate<CourseOfferingResponse>> GetStudentRegistrations(Guid studentId, Guid termId);
		Task<IPaginate<CourseOfferingResponse>> GetProfessorOfferingsBySemester(Guid userId, Guid semesterId);
		Task<IPaginate<StudentInfoResponse>> GetStudentsInOffering(Guid offeringId);
	}
}
