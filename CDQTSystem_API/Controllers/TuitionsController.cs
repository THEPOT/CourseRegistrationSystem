using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace CDQTSystem_API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class TuitionsController : BaseController<TuitionsController>
	{
		private readonly ITuitionService _tuitionService;

		public TuitionsController(ILogger<TuitionsController> logger, ITuitionService tuitionService) : base(logger)
		{
			_tuitionService = tuitionService;
		}

		[HttpPost("periods")]
		[Authorize(Roles = "Financial")]
		public async Task<ActionResult<TuitionPeriodResponse>> CreateTuitionPeriod(
			[FromBody] TuitionPeriodCreateRequest request)
		{
			try
			{
				var result = await _tuitionService.CreateTuitionPeriod(request);
				return Ok(result);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("periods/{periodId}/status")]
		[Authorize(Roles = "Financial")]
		public async Task<ActionResult> UpdatePeriodStatus(
			Guid periodId, [FromBody] StatusUpdateRequest request)
		{
			try
			{
				var result = await _tuitionService.UpdateTuitionPeriodStatus(periodId, request.Status);
				return Ok(new { success = result });
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("payments")]
		[Authorize(Roles = "Financial")]
		public async Task<ActionResult> RecordPayment([FromBody] TuitionPaymentRequest request)
		{
			try
			{
				var result = await _tuitionService.RecordTuitionPayment(request);
				return Ok(new { success = result });
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("terms/{termId}/summary")]
		[Authorize(Roles = "Financial")]
		public async Task<ActionResult<List<TuitionSummaryResponse>>> GetTermSummary(Guid termId)
		{
			var summary = await _tuitionService.GetTuitionSummary(termId);
			return Ok(summary);
		}

		[HttpGet("students/{studentId}/status")]
		[Authorize(Roles = "Financial,Student")]
		public async Task<ActionResult<TuitionStatusResponse>> GetStudentStatus(
			Guid studentId, [FromQuery] Guid semesterId)
		{
			var status = await _tuitionService.GetStudentTuitionStatus(studentId, semesterId);
			if (status == null)
				return NotFound();
			return Ok(status);
		}

		[HttpGet("periods/current")]
		[Authorize(Roles = "Financial,Student,Admin")]
		public async Task<ActionResult<TuitionPeriodResponse>> GetCurrentPeriod()
		{
			var period = await _tuitionService.GetCurrentTuitionPeriod();
			if (period == null) return NotFound();
			return Ok(period);
		}

		[HttpGet("rates")]
		[Authorize(Roles = "Financial,Admin,Staff")]
		public async Task<ActionResult<List<TuitionRateResponse>>> GetTuitionRates()
		{
			var rates = await _tuitionService.GetTuitionRates();
			return Ok(rates);
		}

		[HttpPut("rates")]
		[Authorize(Roles = "Financial,Admin")]
		public async Task<ActionResult> UpdateTuitionRates([FromBody] List<TuitionRateRequest> rates)
		{
			var result = await _tuitionService.UpdateTuitionRates(rates);
			return Ok(new { success = result });
		}

		[HttpGet("program-rates")]
		[Authorize(Roles = "Financial,Admin,Staff")]
		public async Task<ActionResult<List<ProgramRateResponse>>> GetProgramRates()
		{
			var rates = await _tuitionService.GetProgramRates();
			return Ok(rates);
		}

		[HttpPut("program-rates")]
		[Authorize(Roles = "Financial,Admin")]
		public async Task<ActionResult> UpdateProgramRates([FromBody] List<ProgramRateRequest> rates)
		{
			var result = await _tuitionService.UpdateProgramRates(rates);
			return Ok(new { success = result });
		}

		[HttpGet("payments")]
		[Authorize(Roles = "Financial,Admin,Staff")]
		public async Task<ActionResult<List<TuitionPaymentResponse>>> GetTuitionPayments([FromQuery] Guid? studentId, [FromQuery] Guid? termId, [FromQuery] string status, [FromQuery] int page = 1, [FromQuery] int size = 20)
		{
			var filter = new TuitionPaymentFilterRequest
			{
				StudentId = studentId,
				TermId = termId,
				Status = status,
				Page = page,
				Size = size
			};
			var payments = await _tuitionService.GetTuitionPayments(filter);
			return Ok(payments);
		}

		[HttpGet("statistics")]
		[Authorize(Roles = "Financial,Admin,Staff")]
		public async Task<ActionResult<TuitionStatisticsResponse>> GetTuitionStatistics()
		{
			var stats = await _tuitionService.GetTuitionStatistics();
			return Ok(stats);
		}
	}
}
