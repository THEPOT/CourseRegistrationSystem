// Controllers/ServiceRequestsController.cs
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
	public class ServiceRequestsController : BaseController<ServiceRequestsController>
	{
		private readonly IServiceRequestService _serviceRequestService;

		public ServiceRequestsController(ILogger<ServiceRequestsController> logger, IServiceRequestService serviceRequestService) : base(logger)
		{
			_serviceRequestService = serviceRequestService;
		}

		[HttpGet]
		[Authorize(Roles = "Admin,Staff,Student")]
		public async Task<ActionResult<List<ServiceRequestResponse>>> GetAllServiceRequests([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string status = null, [FromQuery] string search = null)
		{
			var requests = await _serviceRequestService.GetAllServiceRequests( status ,  page , size , search);
			return Ok(requests);
		}

		[HttpGet("student")]
		[Authorize(Roles = "Student,Staff,Professor")]
		public async Task<ActionResult<List<ServiceRequestResponse>>> GetStudentServiceRequests([FromQuery] int page = 1, [FromQuery] int size = 10)
		{
			var userId = Guid.Parse(User.FindFirst("UserId")?.Value);
			var requests = await _serviceRequestService.GetStudentServiceRequests(userId, page, size);
			return Ok(requests);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Student,Staff,Professor")]
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

		[HttpGet("statistics")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<ServiceRequestStatisticsResponse>> GetStatistics()
		{
			var stats = await _serviceRequestService.GetStatistics();
			return Ok(stats);
		}
	}
}