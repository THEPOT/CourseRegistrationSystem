using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RegistrationPeriodsController : ControllerBase
    {
        private readonly IRegistrationPeriodService _registrationPeriodService;

        public RegistrationPeriodsController(IRegistrationPeriodService registrationPeriodService)
        {
            _registrationPeriodService = registrationPeriodService;
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<RegistrationPeriodResponse>> CreateRegistrationPeriod(
            [FromBody] RegistrationPeriodCreateRequest request)
        {
            try
            {
                var result = await _registrationPeriodService.CreateRegistrationPeriod(request);
                return Ok(result);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{periodId}/status")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> UpdateStatus(
            Guid periodId, [FromBody] StatusUpdateRequest request)
        {
            try
            {
                var result = await _registrationPeriodService.UpdateRegistrationPeriodStatus(
                    periodId, request.Status);
                return Ok(new { success = result });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("terms/{termId}/summary")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<List<RegistrationSummaryResponse>>> GetTermSummary(
            Guid termId)
        {
            var summary = await _registrationPeriodService.GetRegistrationSummary(termId);
            return Ok(summary);
        }

        [HttpGet("{periodId}/statistics")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<RegistrationStatisticsResponse>> GetStatistics(
            Guid periodId, [FromQuery] Guid? programId, [FromQuery] Guid? courseId)
        {
            var stats = await _registrationPeriodService.GetRegistrationStatistics(
                periodId, programId, courseId);
            return Ok(stats);
        }

        [HttpGet("{periodId}/programs/statistics")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<List<ProgramRegistrationStatisticsResponse>>> 
            GetProgramStatistics(Guid periodId)
        {
            var stats = await _registrationPeriodService.GetProgramStatistics(periodId);
            return Ok(stats);
        }
    }
}
