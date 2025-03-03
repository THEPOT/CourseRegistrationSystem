using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;
using CourseRegistration_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseRegistration_API.Controllers
{
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
		[Authorize(Roles = "Lecturer,Staff")]
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