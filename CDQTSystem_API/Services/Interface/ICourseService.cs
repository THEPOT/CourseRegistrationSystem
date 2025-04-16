using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_Domain.Paginate;

namespace CDQTSystem_API.Services.Interface
{
	public interface ICourseService
	{
		Task<IPaginate<CourseResponses>> GetAllCourses(string? search, int page, int pageSize);
		Task<CourseResponses> GetCourseByCode(string courseCode);
		Task<CourseResponses> GetCourseById(Guid courseId);
		Task<List<CourseResponses>> SearchCourses(string keyword);
		Task<CourseResponses> CreateCourse(CourseCreateRequest request);
		Task<CourseResponses> UpdateCourse(Guid courseId, CourseUpdateRequest request);
		Task<bool> DeleteCourse(Guid courseId);
		Task<CourseSyllabusResponse> GetLatestSyllabus(Guid courseId);
		Task<List<CourseSyllabusResponse>> GetAllSyllabusVersions(Guid courseId);
		Task<CourseSyllabusResponse> CreateSyllabus(Guid courseId, SyllabusCreateRequest request);
	}
}
