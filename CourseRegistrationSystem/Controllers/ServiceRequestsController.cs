// Controllers/ServiceRequestsController.cs
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
	public class ServiceRequestsController : ControllerBase
	{
		private readonly IServiceRequestService _serviceRequestService;

		public ServiceRequestsController(IServiceRequestService serviceRequestService)
		{
			_serviceRequestService = serviceRequestService;
		}

		[HttpGet]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<List<ServiceRequestResponse>>> GetAllServiceRequests([FromQuery] string status = null)
		{
			var requests = await _serviceRequestService.GetAllServiceRequests(status);
			return Ok(requests);
		}

		[HttpGet("student/{studentId}")]
		[Authorize(Roles = "Student,Staff,Lecturer")]
		public async Task<ActionResult<List<ServiceRequestResponse>>> GetStudentServiceRequests(
			Guid studentId, [FromQuery] string status = null)
		{
			var requests = await _serviceRequestService.GetStudentServiceRequests(studentId, status);
			return Ok(requests);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Student,Staff,Lecturer")]
		public async Task<ActionResult<ServiceRequestResponse>> GetServiceRequestById(Guid id)
		{
			var request = await _serviceRequestService.GetServiceRequestById(id);

			if (request == null)
				return NotFound();

			return Ok(request);
		}

		[HttpPost]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<ServiceRequestResponse>> CreateServiceRequest(
			[FromBody] ServiceRequestCreateRequest request)
		{
			try
			{
				var result = await _serviceRequestService.CreateServiceRequest(request);
				return CreatedAtAction(nameof(GetServiceRequestById), new { id = result.Id }, result);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}/status")]
		[Authorize(Roles = "Staff,Admin")]
		public async Task<ActionResult<ServiceRequestResponse>> UpdateServiceRequestStatus(
			Guid id, [FromBody] ServiceRequestStatusUpdateRequest request)
		{
			try
			{
				var result = await _serviceRequestService.UpdateServiceRequestStatus(id, request);

				if (result == null)
					return NotFound();

				return Ok(result);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Student,Staff,Admin")]
		public async Task<ActionResult> DeleteServiceRequest(Guid id)
		{
			try
			{
				var result = await _serviceRequestService.DeleteServiceRequest(id);

				if (!result)
					return NotFound();

				return NoContent();
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}