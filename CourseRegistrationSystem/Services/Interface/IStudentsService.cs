using CourseRegistration_API.Payload.Response;

namespace CourseRegistration_API.Services.Interface
{
	public interface IStudentsService
	{
		Task<StudentInfoResponse> GetStudentInformationById(Guid id);
		Task<List<StudentInfoResponse>> GetAllStudentsInformation();
		Task<StudentFinancialInfoResponse> GetStudentFinancialInfo(Guid studentId);
		Task<StudentProgramResponse> GetStudentProgramAndCourses(Guid studentId);
		Task<StudentTranscriptResponse> GetStudentTranscript(Guid studentId);
		Task<StudentTuitionResponse> GetStudentTuition(Guid studentId);
		Task<List<StudentFinancialInfoResponse>> GetAllStudentScholarships();
		Task<List<StudentProgramResponse>> GetAllStudentProgramsAndCourses();
		Task<List<StudentTranscriptResponse>> GetAllStudentTranscripts();
		Task<List<StudentTuitionResponse>> GetAllStudentTuitions();
		Task<StudentInfoResponse> CreateStudent(StudentCreateRequest request);
		Task<StudentInfoResponse> UpdateStudent(Guid studentId, StudentUpdateRequest request);
		Task<StudentFinancialInfoResponse> AssignScholarshipToStudent(Guid studentId, ScholarshipAssignmentRequest request);
		Task<StudentFinancialInfoResponse> AssignFinancialAidToStudent(Guid studentId, FinancialAidAssignmentRequest request);
		Task<StudentInfoResponse> UpdateStudentProgram(Guid studentId, Guid programId);
		Task<StudentTuitionResponse> CreateStudentTuition(Guid studentId, StudentTuitionCreateRequest request);
		Task<StudentTuitionResponse> UpdateStudentTuition(Guid studentId, Guid tuitionId, StudentTuitionCreateRequest request);
	}
}
