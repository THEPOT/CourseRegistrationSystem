using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;
using CourseRegistration_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CourseRegistration_API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class DegreeAuditsController : ControllerBase
	{
		private readonly IDegreeAuditService _degreeAuditService;

		public DegreeAuditsController(IDegreeAuditService degreeAuditService)
		{
			_degreeAuditService = degreeAuditService;
		}

		[HttpGet("students/{studentId}")]
		[Authorize(Roles = "Student,Staff,Admin")]
		public async Task<ActionResult<DegreeAuditResponse>> GetStudentDegreeAudit(Guid studentId)
		{
			var audit = await _degreeAuditService.GetStudentDegreeAudit(studentId);

			if (audit == null)
				return NotFound();

			return Ok(audit);
		}

		[HttpPut("students/{studentId}")]
		[Authorize(Roles = "Staff,Admin")]
		public async Task<ActionResult<DegreeAuditResponse>> UpdateStudentDegreeAudit(
			Guid studentId, [FromBody] DegreeAuditUpdateRequest request)
		{
			try
			{
				var audit = await _degreeAuditService.UpdateStudentDegreeAudit(studentId, request);

				if (audit == null)
					return NotFound();

				return Ok(audit);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("students/{studentId}/export")]
		[Authorize(Roles = "Student,Staff,Admin")]
		public async Task<IActionResult> ExportDegreeAuditPdf(Guid studentId)
		{
			try
			{
				var pdfBytes = await _degreeAuditService.ExportDegreeAuditPdf(studentId);
				return File(pdfBytes, "application/pdf", $"degree-audit-{studentId}.pdf");
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}