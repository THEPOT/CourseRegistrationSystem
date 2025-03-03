// CourseRegistrationSystem/Controllers/MidtermEvaluationsController.cs
using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;
using CourseRegistration_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseRegistration_API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MidtermEvaluationsController : ControllerBase
	{
		private readonly IMidtermEvaluationService _evaluationService;

		public MidtermEvaluationsController(IMidtermEvaluationService evaluationService)
		{
			_evaluationService = evaluationService;
		}

		[HttpGet("course-offering/{courseOfferingId}/students")]
		[Authorize(Roles = "Lecturer,Staff")]
		public async Task<ActionResult<List<StudentForEvaluationResponse>>> GetStudentsForEvaluation(Guid courseOfferingId)
		{
			var students = await _evaluationService.GetStudentsForEvaluation(courseOfferingId);
			return Ok(students);
		}

		[HttpGet("course-offering/{courseOfferingId}/summary")]
		[Authorize(Roles = "Student,Lecturer,Staff")]
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
		[Authorize(Roles = "Lecturer")]
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
		[Authorize(Roles = "Lecturer")]
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
		[Authorize(Roles = "Lecturer")]
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