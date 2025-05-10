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
    public class MajorsController : ControllerBase
    {
        private readonly IMajorService _majorService;
        public MajorsController(IMajorService majorService)
        {
            _majorService = majorService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<List<MajorResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? search = null)
		{
			var majors = await _majorService.GetAllMajors(page, size, search);
			return Ok(majors);
		}

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<MajorResponse>> GetById(Guid id)
        {
            var major = await _majorService.GetMajorById(id);
            if (major == null) return NotFound();
            return Ok(major);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MajorResponse>> Create([FromBody] MajorCreateRequest request)
        {
            var major = await _majorService.CreateMajor(request);
            return CreatedAtAction(nameof(GetById), new { id = major.Id }, major);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MajorResponse>> Update(Guid id, [FromBody] MajorUpdateRequest request)
        {
            var major = await _majorService.UpdateMajor(id, request);
            if (major == null) return NotFound();
            return Ok(major);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _majorService.DeleteMajor(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
} 