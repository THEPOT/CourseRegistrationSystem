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
		private readonly ICourseEvaluationService _evaluationService;

		public CourseEvaluationsController(ICourseEvaluationService evaluationService)
		{
			_evaluationService = evaluationService;
		}

		[HttpGet("offerings/{termId}")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<List<CourseOfferingForEvaluationResponse>>> GetCourseOfferingsForEvaluation(Guid termId)
		{
			var offerings = await _evaluationService.GetCourseOfferingsForEvaluation(termId);
			return Ok(offerings);
		}

		[HttpGet("summaries/{termId}")]
		[Authorize(Roles = "Professor,Staff")]
		public async Task<ActionResult<List<CourseEvaluationSummaryResponse>>> GetCourseEvaluationSummaries(Guid termId)
		{
			var summaries = await _evaluationService.GetCourseEvaluationSummaries(termId);
			return Ok(summaries);
		}

		[HttpPost]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult> CreateCourseEvaluation([FromBody] CourseEvaluationCreateRequest request)
		{
			try
			{
				var result = await _evaluationService.CreateCourseEvaluation(request);
				return Ok(new { success = result });
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}