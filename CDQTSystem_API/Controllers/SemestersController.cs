using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemestersController(ISemesterService semesterService)
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
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<List<SemesterResponse>>> GetAllSemesters()
        {
            var semesters = await _semesterService.GetAllSemesters();
            return Ok(semesters);
        }
    }
}