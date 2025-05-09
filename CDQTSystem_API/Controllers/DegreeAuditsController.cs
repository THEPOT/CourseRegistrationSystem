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
	public class DegreeAuditsController : BaseController<DegreeAuditsController>
	{
		private readonly IDegreeAuditService _degreeAuditService;

		public DegreeAuditsController(ILogger<DegreeAuditsController> logger, IDegreeAuditService degreeAuditService) : base(logger)
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

		[HttpGet("programs/{programId}/requirements")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<ProgramRequirementsResponse>> GetProgramRequirements(Guid programId)
		{
			var result = await _degreeAuditService.GetProgramRequirements(programId);
			return Ok(result);
		}

		[HttpPut("programs/{programId}/requirements")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult> UpdateProgramRequirements(Guid programId, [FromBody] ProgramRequirementsUpdateRequest request)
		{
			var result = await _degreeAuditService.UpdateProgramRequirements(programId, request);
			return Ok(result);
		}

		[HttpGet("batch-progress")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<BatchProgressResponse>> GetBatchProgress([FromQuery] BatchProgressFilterRequest filter)
		{
			var result = await _degreeAuditService.GetBatchProgress(filter);
			return Ok(result);
		}

		[HttpGet("students/{studentId}/progress")]
		[Authorize(Roles = "Student,Staff,Admin")]
		public async Task<ActionResult<StudentProgressResponse>> GetStudentProgress(Guid studentId)
		{
			var result = await _degreeAuditService.GetStudentProgress(studentId);
			return Ok(result);
		}

		[HttpGet("export")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<IActionResult> ExportAuditReport([FromQuery] AuditExportRequest filter)
		{
			var fileBytes = await _degreeAuditService.ExportAuditReport(filter);
			return File(fileBytes, "application/pdf", "audit-report.pdf");
		}
	}
}