using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDQTSystem_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProfessorsController : BaseController<ProfessorsController>
	{
        private readonly IProfessorService _professorService;

        public ProfessorsController(ILogger<ProfessorsController> logger, IProfessorService professorService) : base(logger)
		{
            _professorService = professorService;
        }

        [HttpGet]
        [Authorize(Roles = "Staff,Student")]
        public async Task<ActionResult<List<ProfessorResponse>>> GetAllProfessors([FromQuery] int page, [FromQuery] int size, [FromQuery] string? search)
        {
            try
            {
                var professors = await _professorService.GetAllProfessors(page, size, search);
                return Ok(professors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}