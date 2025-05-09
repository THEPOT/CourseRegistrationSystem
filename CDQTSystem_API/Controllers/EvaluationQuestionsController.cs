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
    public class EvaluationQuestionsController : ControllerBase
    {
        private readonly IEvaluationQuestionService _service;
        public EvaluationQuestionsController(IEvaluationQuestionService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<List<EvaluationQuestionResponse>>> GetAll()
        {
            var result = await _service.GetAllQuestions();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<EvaluationQuestionResponse>> GetById(Guid id)
        {
            var result = await _service.GetQuestionById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<EvaluationQuestionResponse>> Create([FromBody] EvaluationQuestionRequest request)
        {
            var result = await _service.CreateQuestion(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<EvaluationQuestionResponse>> Update(Guid id, [FromBody] EvaluationQuestionRequest request)
        {
            var result = await _service.UpdateQuestion(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteQuestion(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
} 