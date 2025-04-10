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
	public class CourseRegistrationsController : ControllerBase
	{
		private readonly ICourseRegistrationService _registrationService;

		public CourseRegistrationsController(ICourseRegistrationService registrationService)
		{
			_registrationService = registrationService;
		}

		[HttpGet("students/{studentId}/prerequisites/{courseId}")]
		[Authorize(Roles = "Student,Staff,Admin")]
		public async Task<ActionResult<bool>> CheckPrerequisites(Guid studentId, Guid courseId)
		{
			var result = await _registrationService.CheckPrerequisites(studentId, courseId);
			return Ok(result);
		}

		[HttpGet("terms/{termId}/summary")]
		[Authorize(Roles = "Staff,Admin")]
		public async Task<ActionResult<List<CourseRegistrationSummaryResponse>>> GetRegistrationSummaryByTerm(Guid termId)
		{
			var summary = await _registrationService.GetRegistrationSummaryByTerm(termId);
			return Ok(summary);
		}

		[HttpPost]
		[Authorize(Roles = "Student,Staff,Admin")]
		public async Task<ActionResult> RegisterCourse([FromBody] CourseRegistrationRequest request)
		{
			try
			{
				var result = await _registrationService.RegisterCourse(request);
				return Ok(new { success = result });
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("batch")]
		[Authorize(Roles = "Student,Staff,Admin")]
		public async Task<ActionResult<List<bool>>> RegisterCourses([FromBody] BatchCourseRegistrationRequest request)
		{
			var results = await _registrationService.RegisterCourses(request);
			return Ok(results);
		}

		[HttpPut("{registrationId}")]
		[Authorize(Roles = "Staff,Admin")]
		public async Task<ActionResult> UpdateRegistration(
			Guid registrationId, [FromBody] CourseRegistrationUpdateRequest request)
		{
			try
			{
				var result = await _registrationService.UpdateRegistration(registrationId, request);
				return Ok(new { success = result });
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{registrationId}")]
		[Authorize(Roles = "Student,Staff,Admin")]
		public async Task<ActionResult> CancelRegistration(Guid registrationId)
		{
			try
			{
				var result = await _registrationService.CancelRegistration(registrationId);
				return Ok(new { success = result });
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("students/{studentId}/terms/{termId}")]
		[Authorize(Roles = "Student,Staff,Lecturer,Admin")]
		public async Task<ActionResult<List<CourseOfferingResponse>>> GetStudentRegistrations(Guid studentId, Guid termId)
		{
			var registrations = await _registrationService.GetStudentRegistrations(studentId, termId);
			return Ok(registrations);
		}

		[HttpGet("courses/{courseOfferingId}/students")]
		[Authorize(Roles = "Lecturer,Staff,Admin")]
		public async Task<ActionResult<List<StudentInfoResponse>>> GetCourseStudents(Guid courseOfferingId)
		{
			var students = await _registrationService.GetCourseStudents(courseOfferingId);
			return Ok(students);
		}

		[HttpGet("available-courses")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<List<AvailableCourseResponse>>> GetAvailableCourseOfferings()
		{
			try 
			{
				var offerings = await _registrationService.GetAvailableCourseOfferings();
				return Ok(offerings);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
