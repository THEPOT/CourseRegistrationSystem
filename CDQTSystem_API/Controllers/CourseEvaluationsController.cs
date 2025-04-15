using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDQTSystem_API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class CourseEvaluationsController : ControllerBase
	{
		private readonly ICourseEvaluationService _courseEvaluationService;

		public CourseEvaluationsController(ICourseEvaluationService courseEvaluationService)
		{
			_courseEvaluationService = courseEvaluationService;
		}

		[HttpPost]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> SubmitEvaluation([FromBody] SubmitCourseEvaluationRequest request)
		{
			var result = await _courseEvaluationService.SubmitCourseEvaluation(request);
			return Ok(new { success = result });
		}

		[HttpGet("status/{semesterId:guid}")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> GetStudentStatus(Guid semesterId)
		{
			var studentId = Guid.Empty; // This would be retrieved from the current user
			var status = await _courseEvaluationService.GetStudentEvaluationStatus(studentId, semesterId);
			return Ok(status);
		}

		[HttpGet("professor/{semesterId:guid}")]
		[Authorize(Roles = "Professor")]
		public async Task<IActionResult> GetProfessorEvaluations(Guid semesterId)
		{
			var professorId = Guid.Empty; // This would be retrieved from the current user
			var evaluations = await _courseEvaluationService.GetProfessorEvaluations(professorId, semesterId);
			return Ok(evaluations);
		}

		[HttpPost("period")]
		[Authorize(Roles = "Staff")]
		public async Task<IActionResult> SetEvaluationPeriod([FromBody] CourseEvaluationPeriodRequest request)
		{
			var result = await _courseEvaluationService.CreateOrUpdateEvaluationPeriod(request);
			return Ok(result);
		}

		[HttpGet("period/current")]
		[Authorize]
		public async Task<IActionResult> GetCurrentPeriod()
		{
			var period = await _courseEvaluationService.GetCurrentEvaluationPeriod();
			return Ok(period);
		}

		[HttpGet("summary/courses/{semesterId:guid}")]
		[Authorize(Roles = "Staff,Professor")]
		public async Task<IActionResult> GetCourseSummaries(Guid semesterId)
		{
			var summaries = await _courseEvaluationService.GetCourseEvaluationSummaries(semesterId);
			return Ok(summaries);
		}

		[HttpGet("summary/professors/{semesterId:guid}")]
		[Authorize(Roles = "Staff,Professor")]
		public async Task<IActionResult> GetProfessorSummaries(Guid semesterId)
		{
			var summaries = await _courseEvaluationService.GetProfessorEvaluationSummaries(semesterId);
			return Ok(summaries);
		}

		[HttpPost("send-results/{semesterId:guid}")]
		[Authorize(Roles = "Staff")]
		public async Task<IActionResult> SendResultsToProfessors(Guid semesterId)
		{
			var result = await _courseEvaluationService.SendEvaluationResultsToProfessors(semesterId);
			return Ok(new { success = result });
		}

		[HttpGet("export/courses/{semesterId:guid}")]
		[Authorize(Roles = "Staff")]
		public async Task<IActionResult> ExportCourseEvaluations(Guid semesterId)
		{
			var fileData = await _courseEvaluationService.ExportCourseEvaluations(semesterId);
			return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"course-evaluations-{semesterId}.xlsx");
		}

		[HttpGet("export/professors/{semesterId:guid}")]
		[Authorize(Roles = "Staff")]
		public async Task<IActionResult> ExportProfessorEvaluations(Guid semesterId)
		{
			var fileData = await _courseEvaluationService.ExportProfessorEvaluations(semesterId);
			return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"professor-evaluations-{semesterId}.xlsx");
		}
	}
}