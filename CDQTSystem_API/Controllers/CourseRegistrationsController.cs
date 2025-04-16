using CDQTSystem_API.Messages;
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
	public class CourseRegistrationsController : BaseController<CourseRegistrationsController>
	{
		private readonly ICourseRegistrationService _registrationService;

		public CourseRegistrationsController(ILogger<CourseRegistrationsController> logger, ICourseRegistrationService registrationService) : base(logger)
		{
			_registrationService = registrationService;
		}

		[HttpGet("available-courses")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<List<AvailableCourseResponse>>> GetAvailableCourseOfferings()
		{
			try 
			{
				var userId = Guid.Parse(User.FindFirst("UserId")?.Value);
				var offerings = await _registrationService.GetAvailableCourseOfferingsForStudent(userId);
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

		[HttpPost("register")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<CourseRegistrationResult>> RegisterCourse(
			[FromBody] CourseRegistrationRequest request)
		{
			try
			{
				var userId = Guid.Parse(User.FindFirst("UserId")?.Value);
				var result = await _registrationService.RegisterCourse(request, userId);
				return Ok(result);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request" });
			}
		}
	}
}
