using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDQTSystem_API.Controllers
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

		[HttpGet("students/{studentId}/prerequisites/{courseId}")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<bool>> CheckPrerequisites(Guid studentId, Guid courseId)
		{
			var result = await _registrationService.CheckPrerequisites(studentId, courseId);
			return Ok(result);
		}

		[HttpPost]
		[Authorize(Roles = "Student")]
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
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<List<bool>>> RegisterCourses([FromBody] BatchCourseRegistrationRequest request)
		{
			var results = await _registrationService.RegisterCourses(request);
			return Ok(results);
		}

		[HttpGet("students/{studentId}/terms/{termId}")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<List<CourseOfferingResponse>>> GetStudentRegistrations(Guid studentId, Guid termId)
		{
			var registrations = await _registrationService.GetStudentRegistrations(studentId, termId);
			return Ok(registrations);
		}
	}
}
