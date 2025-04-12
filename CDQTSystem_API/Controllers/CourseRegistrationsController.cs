﻿using CDQTSystem_API.Messages;
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
				var studentId = Guid.Parse(User.FindFirst("UserId")?.Value);
				var offerings = await _registrationService.GetAvailableCourseOfferingsForStudent(studentId);
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
				var result = await _registrationService.RegisterCourse(request);
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
