using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

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
    }
}
