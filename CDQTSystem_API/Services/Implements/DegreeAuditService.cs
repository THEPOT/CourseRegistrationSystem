using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDQTSystem_API.Services.Implements
{
	public class DegreeAuditService : BaseService<DegreeAuditService>, IDegreeAuditService
	{
		public DegreeAuditService(IUnitOfWork<DbContext> unitOfWork, ILogger<DegreeAuditService> logger,
					  IMapper mapper, IHttpContextAccessor httpContextAccessor)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public async Task<DegreeAuditResponse> GetStudentDegreeAudit(Guid studentId)
		{
			// Get student with all necessary information
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(
					predicate: s => s.Id == studentId,
					include: q => q
						.Include(s => s.User)
						.Include(s => s.Major)
							.ThenInclude(p => p.Courses)
						.Include(s => s.CourseRegistrations)
							.ThenInclude(r => r.ClassSection)
								.ThenInclude(co => co.Course)
						.Include(s => s.CourseRegistrations)
							.ThenInclude(r => r.ClassSection)
								.ThenInclude(co => co.Semester)
						.Include(s => s.CourseRegistrations)
							.ThenInclude(r => r.Grades)
				);

			if (student == null)
				return null;

			// Get all Major required courses
			var requiredCourses = student.Major.Courses.ToList();
			var requiredCourseIds = requiredCourses.Select(c => c.Id).ToList();

			// Get completed courses (with passing grades)
			var completedRegistrations = student.CourseRegistrations
				.Where(r => r.Grades.Any(g => g.QualityPoints >= 1.0m)) // Passing grade
				.ToList();

			var completedCourseIds = completedRegistrations
				.Select(r => r.ClassSection.CourseId)
				.ToList();

			// Get courses in progress (registered but no grades or non-passing grades)
			var inProgressRegistrations = student.CourseRegistrations
				.Where(r => !r.Grades.Any() || r.Grades.Any(g => g.QualityPoints < 1.0m))
				.ToList();

			// Get remaining required courses
			var remainingCourseIds = requiredCourseIds
				.Except(completedCourseIds)
				.Except(inProgressRegistrations.Select(r => r.ClassSection.CourseId))
				.ToList();

			// Calculate completed credits
			var completedCredits = completedRegistrations.Sum(r => r.ClassSection.Course.Credits);

			// Calculate remaining credits
			var remainingCredits = student.Major.RequiredCredits - completedCredits;

			// Calculate GPA
			var totalQualityPoints = completedRegistrations
				.Sum(registration => 
					registration.Grades.Sum(grade => 
						grade.QualityPoints * registration.ClassSection.Course.Credits
					)
				);

			var totalCreditsForGPA = completedRegistrations.Sum(r => r.ClassSection.Course.Credits);

			var gpa = totalCreditsForGPA > 0
				? (double)(totalQualityPoints / totalCreditsForGPA)
				: 0.0;

			// Determine graduation eligibility (simple check - can be made more complex)
			var eligibleForGraduation = completedCredits >= student.Major.RequiredCredits &&
									   remainingCourseIds.Count == 0;

			// Retrieve additional requirements from a degreeAuditNotes table or other source
			// For now, we'll leave this empty
			string additionalRequirements = "";
			List<string> notes = new List<string>();

			// Build response
			var response = new DegreeAuditResponse
			{
				StudentId = student.Id,
				Mssv = student.User.UserCode,
				StudentName = student.User.FullName,
				MajorName = student.Major.MajorName,
				RequiredCredits = student.Major.RequiredCredits,
				CompletedCredits = completedCredits,
				RemainingCredits = remainingCredits,
				CompletionPercentage = student.Major.RequiredCredits > 0
					? Math.Round((double)completedCredits / student.Major.RequiredCredits * 100, 2)
					: 0,
				GPA = Math.Round(gpa, 2),
				EligibleForGraduation = eligibleForGraduation,
				AdditionalRequirements = additionalRequirements,
				Notes = notes
			};

			// Add required courses information
			response.RequiredCourses = requiredCourses.Select(c => new CourseAuditInfo
			{
				CourseId = c.Id,
				CourseCode = c.CourseCode,
				CourseName = c.CourseName,
				Credits = c.Credits,
				IsRequired = true
			}).ToList();

			// Add completed courses with grades
			response.CompletedCourses = completedRegistrations.Select(r => new CourseAuditInfo
			{
				CourseId = r.ClassSection.CourseId,
				CourseCode = r.ClassSection.Course.CourseCode,
				CourseName = r.ClassSection.Course.CourseName,
				Credits = r.ClassSection.Course.Credits,
				Grade = r.Grades.FirstOrDefault()?.GradeValue,
				QualityPoints = r.Grades.FirstOrDefault()?.QualityPoints,
				Semester = r.ClassSection.Semester.SemesterName,
				IsRequired = requiredCourseIds.Contains(r.ClassSection.CourseId)
			}).ToList();

			// Add in-progress courses
			response.InProgressCourses = inProgressRegistrations.Select(r => new CourseAuditInfo
			{
				CourseId = r.ClassSection.CourseId,
				CourseCode = r.ClassSection.Course.CourseCode,
				CourseName = r.ClassSection.Course.CourseName,
				Credits = r.ClassSection.Course.Credits,
				Semester = r.ClassSection.Semester.SemesterName,
				IsRequired = requiredCourseIds.Contains(r.ClassSection.CourseId)
			}).ToList();

			// Add remaining required courses
			response.RemainingCourses = requiredCourses
				.Where(c => remainingCourseIds.Contains(c.Id))
				.Select(c => new CourseAuditInfo
				{
					CourseId = c.Id,
					CourseCode = c.CourseCode,
					CourseName = c.CourseName,
					Credits = c.Credits,
					IsRequired = true
				}).ToList();

			return response;
		}

		public async Task<DegreeAuditResponse> UpdateStudentDegreeAudit(Guid studentId, DegreeAuditUpdateRequest request)
		{
			try
			{

				// Get student audit first
				var auditResponse = await GetStudentDegreeAudit(studentId);

				if (auditResponse == null)
					return null;

				// For now, we'll just add the updates to the audit response
				// In a real implementation, you would store these adjustments in a database table

				// Update notes
				if (request.NotesToAdd != null && request.NotesToAdd.Any())
				{
					if (auditResponse.Notes == null)
						auditResponse.Notes = new List<string>();

					auditResponse.Notes.AddRange(request.NotesToAdd);
				}

				// Update additional requirements
				if (!string.IsNullOrEmpty(request.AdditionalRequirements))
				{
					auditResponse.AdditionalRequirements = request.AdditionalRequirements;
				}

				// Override graduation eligibility
				auditResponse.EligibleForGraduation = request.EligibleForGraduation;

				// For course adjustments, in a real implementation you would:
				// 1. Create a table for overrides/adjustments
				// 2. Store the adjustments there
				// 3. Consider them when generating the audit

				await _unitOfWork.CommitAsync();

				return auditResponse;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating degree audit: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<byte[]> ExportDegreeAuditPdf(Guid studentId)
		{
			var audit = await GetStudentDegreeAudit(studentId);

			if (audit == null)
				throw new BadHttpRequestException("Student not found");

			// In a real implementation, you would:
			// 1. Use a PDF generation library like iTextSharp or PdfSharp
			// 2. Create a nicely formatted PDF with the audit information
			// 3. Return the PDF bytes

			// For now, we'll return a placeholder byte array
			return new byte[] { 1, 2, 3, 4, 5 };
		}
	}
}