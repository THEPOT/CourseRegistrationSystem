using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDQTSystem_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProfessorsController : ControllerBase
    {
        private readonly IProfessorService _professorService;

        public ProfessorsController(IProfessorService professorService)
        {
            _professorService = professorService;
        }

        [HttpGet]
        [Authorize(Roles = "Staff,Student")]
        public async Task<ActionResult<List<ProfessorResponse>>> GetAllProfessors()
        {
            try
            {
                var professors = await _professorService.GetAllProfessors();
                return Ok(professors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}