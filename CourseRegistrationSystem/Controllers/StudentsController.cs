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
	[Route("api/[controller]")]
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
		public async Task<ActionResult<List<StudentFinancialInfoResponse>>> GetAllStudentScholarships()
		{
			var scholarships = await _studentsService.GetAllStudentScholarships();
			return Ok(scholarships);
		}

		[HttpGet("programs")]
		[Authorize(Roles = "Student,Staff,Lecturer")]
		public async Task<ActionResult<List<StudentProgramResponse>>> GetAllStudentProgramsAndCourses()
		{
			var programs = await _studentsService.GetAllStudentProgramsAndCourses();
			return Ok(programs);
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
		public async Task<ActionResult<StudentTuitionResponse>> UpdateStudentTuition(Guid id, Guid tuitionId, [FromBody] StudentTuitionCreateRequest request)
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
	}
}