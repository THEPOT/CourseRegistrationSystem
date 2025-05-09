using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CDQTSystem_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ScholarshipsController : ControllerBase
    {
        private readonly IScholarshipsService _service;
        public ScholarshipsController(IScholarshipsService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<PaginatedScholarshipResponse>> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _service.GetScholarships(page, size);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ScholarshipResponse>> Create([FromBody] ScholarshipRequest request)
        {
            var result = await _service.CreateScholarship(request);
            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ScholarshipResponse>> Update(Guid id, [FromBody] ScholarshipRequest request)
        {
            var result = await _service.UpdateScholarship(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteScholarship(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("{id}/recipients")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult> GetRecipients(Guid id)
        {
            var result = await _service.GetScholarshipRecipients(id);
            return Ok(result);
        }

        [HttpPost("review/{termId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ScholarshipReviewResponse>> Review(Guid termId)
        {
            var result = await _service.ReviewScholarships(termId);
            return Ok(result);
        }
    }
} 