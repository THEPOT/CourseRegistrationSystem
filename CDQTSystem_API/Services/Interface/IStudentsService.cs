using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_Domain.Paginate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDQTSystem_API.Services.Interface
{
	public interface IStudentsService
	{
		Task<StudentInfoResponse> GetStudentInformationById(Guid id);
		Task<IPaginate<StudentInfoResponse>> GetAllStudentsInformation(string? search, int page, int size);
		Task<StudentFinancialInfoResponse> GetStudentFinancialInfo(Guid studentId);
		Task<List<ScholarshipInfo>> GetStudentScholarshipById(Guid studentId);
		Task<StudentProgramResponse> GetStudentProgramAndCourses(Guid studentId);
		Task<StudentTranscriptResponse> GetStudentTranscript(Guid studentId);
		Task<StudentDetailedTranscriptResponse> GetStudentDetailedTranscript(Guid studentId);
		Task<StudentTuitionResponse> GetStudentTuition(Guid studentId);
		Task<List<StudentScholarshipResponse>> GetAllStudentScholarships();
		Task<List<StudentProgramCourseResponse>> GetAllStudentProgramsAndCourses();
		Task<List<StudentTranscriptResponse>> GetAllStudentTranscripts();
		Task<List<StudentTuitionResponse>> GetAllStudentTuitions();
		Task<StudentInfoResponse> CreateStudent(StudentCreateRequest request);
		Task<StudentInfoResponse> UpdateStudent(Guid studentId, StudentUpdateRequest request);
		Task<StudentFinancialInfoResponse> AssignScholarshipToStudent(Guid studentId, ScholarshipAssignmentRequest request);
		Task<StudentFinancialInfoResponse> AssignFinancialAidToStudent(Guid studentId, FinancialAidAssignmentRequest request);
		Task<StudentInfoResponse> UpdateStudentProgram(Guid studentId, Guid programId);
		Task<StudentTuitionResponse> CreateStudentTuition(Guid studentId, StudentTuitionCreateRequest request);
		Task<StudentTuitionResponse> UpdateStudentTuition(Guid studentId, Guid tuitionId, StudentTuitionUpdateRequest request);
		Task<StudentProgramResponse> CreateStudentProgramCourses(Guid studentId, StudentProgramCoursesCreateRequest request);
		Task<StudentProgramResponse> UpdateStudentProgramCourses(Guid studentId, StudentProgramCoursesUpdateRequest request);
		Task<bool> DeleteStudentProgramAndCourses(Guid studentId, string semester = null);
		Task<decimal> GetStudentTermGPA(Guid studentId, Guid termId);
		Task<List<CourseGrade>> GetStudentFailedCourses(Guid studentId);
		Task<List<StudentTranscriptSummary>> GetStudentsByGPA(decimal minGPA, decimal? maxGPA = null);
		Task<List<StudentInfoResponse>> GetStudentsByEnrollmentYear(int year);
		Task<List<StudentInfoResponse>> GetStudentsByProgram(Guid programId);
		Task<List<StudentInfoResponse>> GetStudentsByScholarship(string scholarshipName);
		Task<StudentDetailedInfoResponse> GetStudentDetailedInformation(Guid studentId);
		Task<byte[]> ExportStudentTranscript(Guid studentId, string format);
		Task<byte[]> ExportAllTranscripts(string format);
		Task<List<StudentScheduleResponse>> GetStudentSchedule(Guid studentId, int? week = null, int? year = null);
	}
}
