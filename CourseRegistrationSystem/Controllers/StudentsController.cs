using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;
using CourseRegistration_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseRegistration_API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class StudentsController : ControllerBase
	{
		private readonly IStudentsService _studentsService;

		public StudentsController(IStudentsService studentsService)
		{
			_studentsService = studentsService;
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Student,Staff")]
		public async Task<ActionResult<StudentInfoResponse>> GetStudentInformation(Guid id)
		{
			var studentInfo = await _studentsService.GetStudentInformationById(id);

			if (studentInfo == null)
				return NotFound();

			return Ok(studentInfo);
		}

		[HttpGet]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<List<StudentInfoResponse>>> GetAllStudentsInformation()
		{
			var studentsInfo = await _studentsService.GetAllStudentsInformation();
			return Ok(studentsInfo);
		}

		[HttpGet("{id}/financial-info")]
		[Authorize(Roles = "Student,Staff")]
		public async Task<ActionResult<StudentFinancialInfoResponse>> GetStudentFinancialInfo(Guid id)
		{
			var financialInfo = await _studentsService.GetStudentFinancialInfo(id);

			if (financialInfo == null)
				return NotFound();

			return Ok(financialInfo);
		}

		[HttpGet("{id}/program")]
		[Authorize(Roles = "Student,Staff")]
		public async Task<ActionResult<StudentProgramResponse>> GetStudentProgramAndCourses(Guid id)
		{
			var programInfo = await _studentsService.GetStudentProgramAndCourses(id);

			if (programInfo == null)
				return NotFound();

			return Ok(programInfo);
		}

		[HttpGet("{id}/transcript")]
		[Authorize(Roles = "Student,Staff,Lecturer")]
		public async Task<ActionResult<StudentTranscriptResponse>> GetStudentTranscript(Guid id)
		{
			var transcript = await _studentsService.GetStudentTranscript(id);

			if (transcript == null)
				return NotFound();

			return Ok(transcript);
		}

		[HttpGet("{id}/term/{termId}/gpa")]
		[Authorize(Roles = "Student,Staff,Lecturer")]
		public async Task<ActionResult<decimal>> GetStudentTermGPA(Guid id, Guid termId)
		{
			var gpa = await _studentsService.GetStudentTermGPA(id, termId);
			return Ok(gpa);
		}

		[HttpGet("{id}/failed-courses")]
		[Authorize(Roles = "Student,Staff,Lecturer")]
		public async Task<ActionResult<List<CourseGrade>>> GetStudentFailedCourses(Guid id)
		{
			var failedCourses = await _studentsService.GetStudentFailedCourses(id);
			return Ok(failedCourses);
		}

		[HttpGet("by-gpa")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<List<StudentTranscriptSummary>>> GetStudentsByGPA(
			[FromQuery] decimal minGPA = 0, [FromQuery] decimal? maxGPA = null)
		{
			var students = await _studentsService.GetStudentsByGPA(minGPA, maxGPA);
			return Ok(students);
		}

		[HttpGet("{id}/tuition")]
		[Authorize(Roles = "Student,Staff,Lecturer")]
		public async Task<ActionResult<StudentTuitionResponse>> GetStudentTuition(Guid id)
		{
			var tuition = await _studentsService.GetStudentTuition(id);

			if (tuition == null)
				return NotFound();

			return Ok(tuition);
		}

		[HttpGet("scholarships")]
		[Authorize(Roles = "Student,Staff,Lecturer")]
		public async Task<ActionResult<List<StudentScholarshipResponse>>> GetAllStudentScholarships()
		{
			var scholarships = await _studentsService.GetAllStudentScholarships();
			return Ok(scholarships);
		}

		[HttpGet("programs")]
		[Authorize(Roles = "Student,Staff,Lecturer")]
		public async Task<ActionResult<List<StudentProgramCourseResponse>>> GetAllStudentProgramsAndCourses()
		{
			var studentPrograms = await _studentsService.GetAllStudentProgramsAndCourses();
			return Ok(studentPrograms);
		}

		[HttpGet("transcripts")]
		[Authorize(Roles = "Student,Staff,Lecturer")]
		public async Task<ActionResult<List<StudentTranscriptResponse>>> GetAllStudentTranscripts()
		{
			var transcripts = await _studentsService.GetAllStudentTranscripts();
			return Ok(transcripts);
		}

		[HttpGet("tuitions")]
		[Authorize(Roles = "Student,Staff,Lecturer")]
		public async Task<ActionResult<List<StudentTuitionResponse>>> GetAllStudentTuitions()
		{
			var tuitions = await _studentsService.GetAllStudentTuitions();
			return Ok(tuitions);
		}

		[HttpPost]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<StudentInfoResponse>> CreateStudent([FromBody] StudentCreateRequest request)
		{
			try
			{
				var student = await _studentsService.CreateStudent(request);
				return CreatedAtAction(nameof(GetStudentInformation), new { id = student.Id }, student);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<StudentInfoResponse>> UpdateStudent(Guid id, [FromBody] StudentUpdateRequest request)
		{
			try
			{
				var student = await _studentsService.UpdateStudent(id, request);

				if (student == null)
					return NotFound();

				return Ok(student);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{id}/scholarships")]
		[Authorize(Roles = "Student,Staff")]
		public async Task<ActionResult<List<ScholarshipInfo>>> GetStudentScholarships(Guid id)
		{
			var scholarships = await _studentsService.GetStudentScholarshipById(id);

			if (scholarships == null)
				return NotFound();

			return Ok(scholarships);
		}


		[HttpPost("{id}/scholarships")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<StudentFinancialInfoResponse>> AssignScholarship(Guid id, [FromBody] ScholarshipAssignmentRequest request)
		{
			try
			{
				var result = await _studentsService.AssignScholarshipToStudent(id, request);

				if (result == null)
					return NotFound();

				return Ok(result);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("{id}/financial-aids")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<StudentFinancialInfoResponse>> AssignFinancialAid(Guid id, [FromBody] FinancialAidAssignmentRequest request)
		{
			try
			{
				var result = await _studentsService.AssignFinancialAidToStudent(id, request);

				if (result == null)
					return NotFound();

				return Ok(result);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}


		[HttpPut("{id}/program/{programId}")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<StudentInfoResponse>> UpdateStudentProgram(Guid id, Guid programId)
		{
			try
			{
				var student = await _studentsService.UpdateStudentProgram(id, programId);

				if (student == null)
					return NotFound();

				return Ok(student);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("{id}/tuitions")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<StudentTuitionResponse>> CreateStudentTuition(Guid id, [FromBody] StudentTuitionCreateRequest request)
		{
			try
			{
				var tuition = await _studentsService.CreateStudentTuition(id, request);

				if (tuition == null)
					return NotFound();

				return Ok(tuition);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}/tuitions/{tuitionId}")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<StudentTuitionResponse>> UpdateStudentTuition(Guid id, Guid tuitionId, [FromBody] StudentTuitionUpdateRequest request)
		{
			try
			{
				var tuition = await _studentsService.UpdateStudentTuition(id, tuitionId, request);

				if (tuition == null)
					return NotFound();

				return Ok(tuition);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPost("{id}/program-courses")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<StudentProgramResponse>> CreateStudentProgramCourses(
	Guid id,
	[FromBody] StudentProgramCoursesCreateRequest request)
		{
			try
			{
				var result = await _studentsService.CreateStudentProgramCourses(id, request);

				if (result == null)
					return NotFound();

				return Ok(result);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpGet("by-year/{year}")]
		[Authorize(Roles = "Staff,Admin")]
		public async Task<ActionResult<List<StudentInfoResponse>>> GetStudentsByEnrollmentYear(int year)
		{
			var students = await _studentsService.GetStudentsByEnrollmentYear(year);
			return Ok(students);
		}

		[HttpGet("by-program/{programId}")]
		[Authorize(Roles = "Staff,Admin")]
		public async Task<ActionResult<List<StudentInfoResponse>>> GetStudentsByProgram(Guid programId)
		{
			var students = await _studentsService.GetStudentsByProgram(programId);
			return Ok(students);
		}

		[HttpGet("by-scholarship/{scholarshipName}")]
		[Authorize(Roles = "Staff,Admin")]
		public async Task<ActionResult<List<StudentScholarshipResponse>>> GetStudentsByScholarship(string scholarshipName)
		{
			var students = await _studentsService.GetStudentsByScholarship(scholarshipName);
			return Ok(students);
		}

		[HttpGet("me")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<StudentInfoResponse>> GetMyInformation()
		{
			// Lấy ID của học sinh từ token
			var studentIdClaim = User.FindFirst("studentId");
			if (studentIdClaim == null || !Guid.TryParse(studentIdClaim.Value, out Guid studentId))
			{
				return BadRequest("Invalid student token");
			}

			var studentInfo = await _studentsService.GetStudentInformationById(studentId);

			if (studentInfo == null)
				return NotFound();

			return Ok(studentInfo);
		}

		[HttpGet("me/detailed")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<StudentDetailedInfoResponse>> GetMyDetailedInformation()
		{
			var studentIdClaim = User.FindFirst("studentId");
			if (studentIdClaim == null || !Guid.TryParse(studentIdClaim.Value, out Guid studentId))
			{
				return BadRequest("Invalid student token");
			}

			var detailedInfo = await _studentsService.GetStudentDetailedInformation(studentId);

			if (detailedInfo == null)
				return NotFound();

			return Ok(detailedInfo);
		}

	}
}
