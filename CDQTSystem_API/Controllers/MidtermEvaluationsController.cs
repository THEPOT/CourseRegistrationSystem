// CourseRegistrationSystem/Controllers/MidtermEvaluationsController.cs
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDQTSystem_API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class MidtermEvaluationsController : ControllerBase
	{
		private readonly IMidtermEvaluationService _midtermEvaluationService;

		public MidtermEvaluationsController(IMidtermEvaluationService midtermEvaluationService)
		{
			_midtermEvaluationService = midtermEvaluationService;
		}

		[HttpPost]
		[Authorize(Roles = "Professor")]
		public async Task<IActionResult> CreateEvaluation([FromBody] CreateMidtermEvaluationRequest request)
		{
			var result = await _midtermEvaluationService.CreateMidtermEvaluation(request);
			return CreatedAtAction(nameof(GetEvaluation), new { id = result.Id }, result);
		}

		[HttpGet("{id:guid}")]
		[Authorize(Roles = "Professor,Student,Staff")]
		public async Task<IActionResult> GetEvaluation(Guid id)
		{
			var evaluation = await _midtermEvaluationService.GetMidtermEvaluation(id);
			if (evaluation == null)
				return NotFound();
			return Ok(evaluation);
		}

		[HttpPut("{id:guid}")]
		[Authorize(Roles = "Professor")]
		public async Task<IActionResult> UpdateEvaluation(Guid id, [FromBody] UpdateMidtermEvaluationRequest request)
		{
			var result = await _midtermEvaluationService.UpdateMidtermEvaluation(id, request);
			return Ok(result);
		}

		[HttpGet("professor/{semesterId:guid}")]
		[Authorize(Roles = "Professor")]
		public async Task<IActionResult> GetProfessorEvaluations(Guid semesterId)
		{
			var evaluations = await _midtermEvaluationService.GetMidtermEvaluationsByProfessor(Guid.Empty, semesterId);
			return Ok(evaluations);
		}

		[HttpGet("student/{studentId:guid}")]
		[Authorize(Roles = "Student,Staff")]
		public async Task<IActionResult> GetStudentEvaluations(Guid studentId, [FromQuery] Guid? semesterId = null)
		{
			var evaluations = await _midtermEvaluationService.GetStudentMidtermEvaluations(studentId, semesterId);
			return Ok(evaluations);
		}

		[HttpPost("period")]
		[Authorize(Roles = "Staff")]
		public async Task<IActionResult> SetEvaluationPeriod([FromBody] MidtermEvaluationPeriodRequest request)
		{
			var result = await _midtermEvaluationService.CreateOrUpdateEvaluationPeriod(request);
			return Ok(result);
		}

		[HttpGet("period/current")]
		[Authorize]
		public async Task<IActionResult> GetCurrentPeriod()
		{
			var period = await _midtermEvaluationService.GetCurrentEvaluationPeriod();
			return Ok(period);
		}

		[HttpGet("summary/{semesterId:guid}")]
		[Authorize(Roles = "Staff")]
		public async Task<IActionResult> GetSummary(Guid semesterId)
		{
			var summary = await _midtermEvaluationService.GetMidtermEvaluationSummary(semesterId);
			return Ok(summary);
		}

		[HttpGet("export/{semesterId:guid}")]
		[Authorize(Roles = "Staff")]
		public async Task<IActionResult> ExportEvaluations(Guid semesterId)
		{
			var fileData = await _midtermEvaluationService.ExportMidtermEvaluations(semesterId);
			return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"midterm-evaluations-{semesterId}.xlsx");
		}
	}
}