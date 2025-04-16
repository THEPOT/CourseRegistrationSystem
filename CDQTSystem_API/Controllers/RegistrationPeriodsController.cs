using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using Microsoft.Extensions.Logging;

namespace CDQTSystem_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RegistrationPeriodsController : BaseController<RegistrationPeriodsController>
	{
        private readonly IRegistrationPeriodService _registrationPeriodService;

        public RegistrationPeriodsController(IRegistrationPeriodService registrationPeriodService, ILogger<RegistrationPeriodsController> logger) : base(logger)
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
                var userIdClaim = User.FindFirst("UserId");
                var userId = Guid.Parse(userIdClaim?.Value);
                

                var result = await _registrationPeriodService.CreateRegistrationPeriod(request, userId);
                return CreatedAtAction(
                    nameof(GetRegistrationPeriod), 
                    new { id = result.Id }, 
                    result);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new 
                { 
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<List<RegistrationPeriodResponse>>> GetAllRegistrationPeriods()
        {
            var periods = await _registrationPeriodService.GetAllRegistrationPeriods();
            return Ok(periods);
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

        [HttpGet("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<RegistrationPeriodResponse>> GetRegistrationPeriod(Guid id)
        {
            var period = await _registrationPeriodService.GetRegistrationPeriodById(id);
            if (period == null)
                return NotFound();
            
            return Ok(period);
        }

        [HttpGet("analytics")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<RegistrationAnalyticsResponse>> GetAnalytics(
            [FromQuery] Guid termId)
        {
            var analytics = await _registrationPeriodService.GetRegistrationAnalytics(termId);
            return Ok(analytics);
        }

        [HttpGet("terms/summaries")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<List<TermRegistrationSummary>>> GetTermSummaries()
        {
            var summaries = await _registrationPeriodService.GetTermRegistrationSummaries();
            return Ok(summaries);
        }

        [HttpGet("courses/summaries")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<List<CourseRegistrationSummary>>> GetCourseSummaries(
            [FromQuery] Guid termId)
        {
            var summaries = await _registrationPeriodService.GetCourseRegistrationSummaries(termId);
            return Ok(summaries);
        }

        [HttpGet("programs/summaries")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<List<ProgramEnrollmentSummary>>> GetProgramSummaries(
            [FromQuery] Guid termId)
        {
            var summaries = await _registrationPeriodService.GetProgramEnrollmentSummaries(termId);
            return Ok(summaries);
        }
    }
}
