using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDQTSystem_API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DepartmentController : ControllerBase
	{
		private readonly IDepartmentService _departmentService;
		public DepartmentController(IDepartmentService departmentService)
		{
			_departmentService = departmentService;
		}
		[HttpGet]
		[Authorize(Roles = "Staff")]
		public async Task<IActionResult> GetAllDepartments()
		{
			var departments = await _departmentService.GetAllDepartments();
			return Ok(departments);
		}
		[HttpPost]
		[Authorize(Roles = "Staff")]
		public async Task<IActionResult> CreateDepartment([FromBody] DepartmentCreateRequest request)
		{
			try
			{
				var result = await _departmentService.CreateDepartment(request);
				return CreatedAtAction(nameof(GetDepartment), new { id = result.Id }, result);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}
		[HttpGet("{departmentId:guid}")]
		[Authorize(Roles = "Staff")]
		public async Task<IActionResult> GetDepartment(Guid id)
		{
			var department = await _departmentService.GetDepartmentById(id);
			if (department == null)
				return NotFound();
			return Ok(department);
		}
		[HttpPut]
		[Authorize(Roles = "Staff")]
		public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] DepartmentUpdateRequest request)
		{
			try
			{
				var result = await _departmentService.UpdateDepartment(id, request);
				if (result == null)
					return NotFound();
				return Ok(result);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}
	} 

}
