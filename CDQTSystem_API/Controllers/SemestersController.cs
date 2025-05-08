using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SemestersController : BaseController<SemestersController>
	{
        private readonly ISemesterService _semesterService;

        public SemestersController(ILogger<SemestersController> logger, ISemesterService semesterService) : base(logger)
		{
            _semesterService = semesterService;
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<SemesterResponse>> CreateSemester(
            [FromBody] SemesterCreateRequest request)
        {
            try
            {
                var result = await _semesterService.CreateSemester(request);
                return CreatedAtAction(nameof(GetSemesterById), 
                    new { id = result.Id }, result);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<SemesterResponse>> GetSemesterById(Guid id)
        {
            var semester = await _semesterService.GetSemesterById(id);
            if (semester == null)
                return NotFound();
            return Ok(semester);
        }

        [HttpGet]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<ActionResult<List<SemesterResponse>>> GetAllSemesters([FromQuery] int page , [FromQuery] int size, [FromQuery] string? search)
        {
            var semesters = await _semesterService.GetAllSemesters(page, size, search);
            return Ok(semesters);
        }
    }
}