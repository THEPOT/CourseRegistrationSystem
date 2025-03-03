using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;

namespace CourseRegistration_API.Services.Interface
{
	public interface ICourseService
	{
		Task<List<CourseBasicInfo>> GetAllCourses();
		Task<CourseDetailsResponse> GetCourseByCode(string courseCode);
		Task<CourseDetailsResponse> GetCourseById(Guid courseId);
		Task<List<CourseDetailsResponse>> SearchCourses(string keyword);
		Task<CourseDetailsResponse> CreateCourse(CourseCreateRequest request);
		Task<CourseDetailsResponse> UpdateCourse(Guid courseId, CourseUpdateRequest request);
		Task<bool> DeleteCourse(Guid courseId);
		Task<CourseSyllabusResponse> GetLatestSyllabus(Guid courseId);
		Task<List<CourseSyllabusResponse>> GetAllSyllabusVersions(Guid courseId);
		Task<CourseSyllabusResponse> CreateSyllabus(Guid courseId, SyllabusCreateRequest request);
	}
}
