using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CDQTSystem_API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class GradesController : BaseController<GradesController>
	{
		private readonly IGradesService _gradesService;
		public GradesController(ILogger<GradesController> logger, IGradesService gradesService) : base(logger)
		{
			_gradesService = gradesService;
		}

		[HttpPost]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<GradeResponse>> CreateOrUpdateGrade([FromBody] GradeEntryRequest request)
		{
			var result = await _gradesService.CreateOrUpdateGrade(request);
			return Ok(result);
		}
	}
}