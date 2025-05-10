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
using System.Text.Json;

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

		public async Task<ProgramRequirementsResponse> GetProgramRequirements(Guid programId)
		{
			var major = await _unitOfWork.GetRepository<Major>()
				.SingleOrDefaultAsync(
					selector: m => new ProgramRequirementsResponse
					{
						ProgramId = m.Id,
						MajorName = m.MajorName,
						RequiredCredits = m.RequiredCredits,
						Requirements = m.Courses.Select(c => new RequiredCoursesResponse
						{
							CourseId = c.Id,
							CourseCode = c.CourseCode,
							CourseName = c.CourseName,
							Credits = c.Credits
						}).ToList()
					},
					predicate: m => m.Id == programId,
					include: q => q.Include(m => m.Courses)
				);
			return major;
		}


		public async Task<bool> UpdateProgramRequirements(Guid programId, ProgramRequirementsUpdateRequest request)
		{
			// Update the major's required credits and courses
			var major = await _unitOfWork.GetRepository<Major>()
				.SingleOrDefaultAsync(
					predicate: m => m.Id == programId,
					include: q => q.Include(m => m.Courses)
				);
			if (major == null)
				return false;

			major.RequiredCredits = request.RequiredCredits;

			// Update required courses (replace all)
			major.Courses.Clear();
			if (request.Requirements != null && request.Requirements.Any())
			{
				var courseRepo = _unitOfWork.GetRepository<Course>();
				foreach (var courseDto in request.Requirements)
				{
					var course = await courseRepo.SingleOrDefaultAsync(predicate: c => c.Id == courseDto.CourseId);
					if (course != null)
						major.Courses.Add(course);
				}
			}

			await _unitOfWork.CommitAsync();
			return true;
		}

		public async Task<BatchProgressResponse> GetBatchProgress(BatchProgressFilterRequest filter)
		{
			// Find the semester by year and term
			var semester = await _unitOfWork.GetRepository<Semester>()
				.SingleOrDefaultAsync(predicate: s => s.AcademicYear == filter.Year.ToString() && s.SemesterName == filter.Term);
			if (semester == null)
				return new BatchProgressResponse { ProgressJson = "{}" };

			// Find the major by program code
			var major = await _unitOfWork.GetRepository<Major>()
				.SingleOrDefaultAsync(predicate: m => m.MajorName == filter.ProgramCode);
			if (major == null)
				return new BatchProgressResponse { ProgressJson = "{}" };

			// Get all students in the major
			var students = await _unitOfWork.GetRepository<Student>()
				.GetListAsync(predicate: s => s.MajorId == major.Id);

			// For each student, calculate completed credits and GPA for the semester
			var studentProgress = new List<object>();
			foreach (var student in students)
			{
				var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
					.GetListAsync(predicate: r => r.StudentId == student.Id && r.ClassSection.SemesterId == semester.Id,
						include: q => q.Include(r => r.ClassSection).ThenInclude(cs => cs.Course).Include(r => r.Grades));

				var completedCredits = registrations
					.Where(r => r.Grades.Any(g => g.QualityPoints >= 1.0m))
					.Sum(r => r.ClassSection.Course.Credits);

				var totalQualityPoints = registrations
					.SelectMany(r => r.Grades)
					.Sum(g => g.QualityPoints * (g.CourseRegistration.ClassSection.Course.Credits));
				var totalCredits = registrations.Sum(r => r.ClassSection.Course.Credits);
				var gpa = totalCredits > 0 ? (double)totalQualityPoints / totalCredits : 0.0;

				studentProgress.Add(new
				{
					StudentId = student.Id,
					StudentName = student.User.FullName,
					CompletedCredits = completedCredits,
					GPA = Math.Round(gpa, 2)
				});
			}

			var result = new
			{
				ProgramId = major.Id,
				ProgramName = major.MajorName,
				Semester = semester.SemesterName,
				Year = semester.AcademicYear,
				StudentProgress = studentProgress
			};

			return new BatchProgressResponse
			{
				ProgressJson = JsonSerializer.Serialize(result)
			};
		}

		public async Task<StudentProgressResponse> GetStudentProgress(Guid userId)
		{
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(
					predicate: s => s.UserId == userId,
					include: q => q.Include(s => s.Major).Include(s => s.User)
				);

			if (student == null || student.User == null || student.Major == null)
				return null;

			var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(predicate: r => r.StudentId == student.Id,
					include: q => q.Include(r => r.ClassSection).ThenInclude(cs => cs.Course).Include(r => r.Grades));

			var completedRegistrations = registrations.Where(r => r.Grades.Any(g => g.QualityPoints >= 1.0m)).ToList();
			var completedCredits = completedRegistrations.Sum(r => r.ClassSection.Course.Credits);
			var requiredCredits = student.Major.RequiredCredits;

			var totalQualityPoints = completedRegistrations
				.SelectMany(r => r.Grades)
				.Sum(g => g.QualityPoints * (g.CourseRegistration.ClassSection.Course.Credits));
			var totalCredits = completedRegistrations.Sum(r => r.ClassSection.Course.Credits);
			var gpa = totalCredits > 0 ? (double)totalQualityPoints / totalCredits : 0.0;

			var admissionYear = student.EnrollmentDate.Year;
			var expectedGraduationYear = admissionYear + 4;

			var requiredCourses = student.Major.Courses.ToList();
			var courseMap = registrations.ToDictionary(r => r.ClassSection.CourseId, r => r);

			var category = new CategoryDto
			{
				Name = "Required Courses",
				Completed = completedRegistrations.Sum(r => r.ClassSection.Course.Credits),
				Required = requiredCourses.Sum(c => c.Credits),
				Courses = requiredCourses.Select(c => {
					var reg = courseMap.ContainsKey(c.Id) ? courseMap[c.Id] : null;
					var grade = reg?.Grades.FirstOrDefault();
					string status = reg == null ? "not-registered" : (grade != null && grade.QualityPoints >= 1.0m ? "completed" : "in-progress");
					return new CourseDto
					{
						Code = c.CourseCode,
						Name = c.CourseName,
						Credits = c.Credits,
						Status = status,
						Grade = grade?.GradeValue
					};
				}).ToList()
			};

			return new StudentProgressResponse
			{
				Student = new StudentInfoDto
				{
					Name = student.User.FullName,
					Id = student.Id,
					Program = student.Major.MajorName,
					AdmissionYear = admissionYear,
					ExpectedGraduation = expectedGraduationYear,
					TotalCredits = completedCredits,
					RequiredCredits = requiredCredits,
					Gpa = Math.Round(gpa, 2)
				},
				Categories = new List<CategoryDto> { category }
			};
		}

		public async Task<byte[]> ExportAuditReport(AuditExportRequest filter)
		{
			// TODO: Implement logic to generate and export audit report as PDF or other format
			// For now, return a placeholder byte array
			return new byte[] { 1, 2, 3, 4, 5 };
		}
	}

}