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
		private readonly IMidtermEvaluationService _evaluationService;

		public MidtermEvaluationsController(IMidtermEvaluationService evaluationService)
		{
			_evaluationService = evaluationService;
		}

		[HttpGet("course-offering/{courseOfferingId}/students")]
		[Authorize(Roles = "Professor,Staff")]
		public async Task<ActionResult<List<StudentForEvaluationResponse>>> GetStudentsForEvaluation(Guid courseOfferingId)
		{
			var students = await _evaluationService.GetStudentsForEvaluation(courseOfferingId);
			return Ok(students);
		}

		[HttpGet("course-offering/{courseOfferingId}/summary")]
		[Authorize(Roles = "Student,Professor,Staff")]
		public async Task<ActionResult<MidtermEvaluationSummaryResponse>> GetMidtermEvaluationSummary(Guid courseOfferingId)
		{
			try
			{
				var summary = await _evaluationService.GetMidtermEvaluationSummary(courseOfferingId);
				return Ok(summary);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("single")]
		[Authorize(Roles = "Professor")]
		public async Task<ActionResult> CreateMidtermEvaluation([FromBody] MidtermEvaluationCreateRequest request)
		{
			try
			{
				var result = await _evaluationService.CreateMidtermEvaluation(request);
				return Ok(new { success = result });
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("batch")]
		[Authorize(Roles = "Professor")]
		public async Task<ActionResult> CreateBatchMidtermEvaluations([FromBody] MidtermEvaluationBatchRequest request)
		{
			try
			{
				var result = await _evaluationService.CreateBatchMidtermEvaluations(request);
				return Ok(new { success = result });
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{evaluationId}")]
		[Authorize(Roles = "Professor")]
		public async Task<ActionResult> UpdateMidtermEvaluation(Guid evaluationId, [FromBody] MidtermEvaluationCreateRequest request)
		{
			try
			{
				var result = await _evaluationService.UpdateMidtermEvaluation(evaluationId, request);
				return Ok(new { success = result });
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}