using AutoMapper;
using CourseRegistration_API.Helpers;
using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;
using CourseRegistration_API.Services.Interface;
using CourseRegistration_API.Utils;
using CourseRegistration_Domain.Entities;
using CourseRegistration_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseRegistration_API.Services.Implements
{
	public class StudentsService : BaseService<StudentsService>, IStudentsService
	{
		public StudentsService(IUnitOfWork<DbContext> unitOfWork, ILogger<StudentsService> logger,
							  IMapper mapper, IHttpContextAccessor httpContextAccessor)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public async Task<StudentInfoResponse> GetStudentInformationById(Guid id)
		{
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(
					predicate: s => s.Id == id,
					include: q => q.Include(s => s.User)
							  .Include(s => s.Program)
				);

			if (student == null)
				return null;

			return new StudentInfoResponse
			{
				Id = student.Id,
				Mssv = student.Mssv,
				FullName = student.User.FullName,
				Email = student.User.Email,
				ProgramName = student.Program.ProgramName,
				EnrollmentDate = student.EnrollmentDate,
				AdmissionDate = student.AdmissionDate,
				AdmissionStatus = student.AdmissionStatus,
				ImageUrl = student.User.Image
			};
		}

		public async Task<List<StudentInfoResponse>> GetAllStudentsInformation()
		{
			var students = await _unitOfWork.GetRepository<Student>()
				.GetListAsync(
					include: q => q.Include(s => s.User)
							  .Include(s => s.Program)
				);

			return students.Select(student => new StudentInfoResponse
			{
				Id = student.Id,
				Mssv = student.Mssv,
				FullName = student.User.FullName,
				Email = student.User.Email,
				ProgramName = student.Program.ProgramName,
				EnrollmentDate = student.EnrollmentDate,
				AdmissionDate = student.AdmissionDate,
				AdmissionStatus = student.AdmissionStatus,
				ImageUrl = student.User.Image
			}).ToList();
		}

		public async Task<List<ScholarshipInfo>> GetStudentScholarshipById(Guid studentId)
		{
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(
					predicate: s => s.Id == studentId,
					include: q => q
						.Include(s => s.StudentScholarships)
							.ThenInclude(ss => ss.Scholarship)
				);

			if (student == null)
				return null;

			var scholarships = student.StudentScholarships.Select(ss => new ScholarshipInfo
			{
				ScholarshipId = ss.ScholarshipId,
				ScholarshipName = ss.Scholarship.ScholarshipName,
				Description = ss.Scholarship.Description,
				Amount = ss.Scholarship.Amount,
				EligibilityCriteria = ss.Scholarship.EligibilityCriteria,
				AwardDate = ss.AwardDate
			}).ToList();

			return scholarships;
		}

		public async Task<StudentFinancialInfoResponse> GetStudentFinancialInfo(Guid studentId)
		{
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(
					predicate: s => s.Id == studentId,
					include: q => q
						.Include(s => s.StudentScholarships)
							.ThenInclude(ss => ss.Scholarship)
						.Include(s => s.StudentFinancialAids)
							.ThenInclude(sf => sf.FinancialAid)
				);

			if (student == null)
				return null;

			var response = new StudentFinancialInfoResponse
			{
				Scholarships = student.StudentScholarships.Select(ss => new ScholarshipInfo
				{
					ScholarshipId = ss.ScholarshipId,
					ScholarshipName = ss.Scholarship.ScholarshipName,
					Description = ss.Scholarship.Description,
					Amount = ss.Scholarship.Amount,
					AwardDate = ss.AwardDate
				}).ToList(),

				FinancialAids = student.StudentFinancialAids.Select(sf => new FinancialAidInfo
				{
					FinancialAidId = sf.FinancialAidId,
					AidName = sf.FinancialAid.AidName,
					Description = sf.FinancialAid.Description,
					Amount = sf.FinancialAid.Amount,
					AwardDate = sf.AwardDate
				}).ToList()
			};

			return response;
		}

		public async Task<StudentProgramResponse> GetStudentProgramAndCourses(Guid studentId)
		{
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(
					predicate: s => s.Id == studentId,
					include: q => q
						.Include(s => s.Program)
							.ThenInclude(p => p.Faculty)
						.Include(s => s.Program)
							.ThenInclude(p => p.Courses)
				);

			if (student == null)
				return null;

			return new StudentProgramResponse
			{
				ProgramId = student.Program.Id,
				ProgramName = student.Program.ProgramName,
				RequiredCredits = student.Program.RequiredCredits,
				FacultyName = student.Program.Faculty.FacultyName,
				Courses = student.Program.Courses.Select(c => new CourseInfo
				{
					CourseId = c.Id,
					CourseCode = c.CourseCode,
					CourseName = c.CourseName,
					Credits = c.Credits,
					Description = c.Description
				}).ToList()
			};
		}

		public async Task<StudentTranscriptResponse> GetStudentTranscript(Guid studentId)
		{
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(
					predicate: s => s.Id == studentId,
					include: q => q
						.Include(s => s.User)
						.Include(s => s.Program)
						.Include(s => s.Registrations)
							.ThenInclude(r => r.CourseOffering)
							.ThenInclude(co => co.Course)
						.Include(s => s.Registrations)
							.ThenInclude(r => r.CourseOffering)
							.ThenInclude(co => co.Term)
						.Include(s => s.Registrations)
							.ThenInclude(r => r.Grades)
				);

			if (student == null)
				return null;

			var courseGrades = student.Registrations
				.Where(r => r.Grades.Any())
				.Select(r => new CourseGrade
				{
					CourseCode = r.CourseOffering.Course.CourseCode,
					CourseName = r.CourseOffering.Course.CourseName,
					Credits = r.CourseOffering.Course.Credits,
					Grade = r.Grades.FirstOrDefault()?.QualityPoints,
					GradeValue = r.Grades.FirstOrDefault()?.GradeValue,
					Semester = r.CourseOffering.Term.TermName,
					IsPassed = r.Grades.Any(g => g.QualityPoints >= 1.0m) // Consider 1.0 (D) as passing
				}).ToList();

			// Group courses by term for term-specific calculations
			var coursesByTerm = courseGrades
				.GroupBy(c => c.Semester)
				.Select(termGroup =>
				{
					var termCourses = termGroup.ToList();
					var termCredits = termCourses.Sum(c => c.Credits);
					var termGPA = termCredits > 0
						? termCourses.Sum(c => c.Credits * c.Grade.GetValueOrDefault()) / termCredits
						: 0;

					return new TermGrades
					{
						TermName = termGroup.Key,
						Courses = termCourses,
						TermGPA = Math.Round(termGPA, 2),
						TermCredits = termCredits
					};
				})
				.OrderBy(t => t.TermName) // Order by term name
				.ToList();

			// Calculate cumulative statistics
			var totalCredits = courseGrades.Sum(c => c.Credits);
			var totalCreditsPassed = courseGrades.Where(c => c.IsPassed).Sum(c => c.Credits);
			var cumulativeGPA = totalCredits > 0
				? courseGrades.Sum(c => c.Credits * c.Grade.GetValueOrDefault()) / totalCredits
				: 0;

			return new StudentTranscriptResponse
			{
				Mssv = student.Mssv,
				StudentName = student.User.FullName,
				ProgramName = student.Program.ProgramName,
				Terms = coursesByTerm,
				CourseGrades = courseGrades, // Keep for backward compatibility
				CumulativeGPA = Math.Round(cumulativeGPA, 2),
				TotalCredits = totalCredits,
				TotalCreditsPassed = totalCreditsPassed
			};
		}

		public async Task<decimal> GetStudentTermGPA(Guid studentId, Guid termId)
		{
			var termGrades = await _unitOfWork.GetRepository<Grade>()
				.GetListAsync(
					predicate: g => g.Registration.StudentId == studentId &&
									g.Registration.CourseOffering.TermId == termId,
					include: q => q
						.Include(g => g.Registration)
						.ThenInclude(r => r.CourseOffering)
						.ThenInclude(co => co.Course)
				);

			if (!termGrades.Any())
				return 0;

			var totalCredits = termGrades.Sum(g => g.Registration.CourseOffering.Course.Credits);
			if (totalCredits == 0)
				return 0;

			var weightedGPA = termGrades.Sum(g =>
				g.QualityPoints * g.Registration.CourseOffering.Course.Credits);

			return Math.Round(weightedGPA / totalCredits, 2);
		}

		public async Task<List<CourseGrade>> GetStudentFailedCourses(Guid studentId)
		{
			var failedRegistrations = await _unitOfWork.GetRepository<Registration>()
				.GetListAsync(
					predicate: r => r.StudentId == studentId &&
								   r.Grades.Any(g => g.QualityPoints < 1.0m),
					include: q => q
						.Include(r => r.CourseOffering)
						.ThenInclude(co => co.Course)
						.Include(r => r.CourseOffering)
						.ThenInclude(co => co.Term)
						.Include(r => r.Grades)
				);

			return failedRegistrations.Select(r => new CourseGrade
			{
				CourseCode = r.CourseOffering.Course.CourseCode,
				CourseName = r.CourseOffering.Course.CourseName,
				Credits = r.CourseOffering.Course.Credits,
				Grade = r.Grades.FirstOrDefault()?.QualityPoints,
				GradeValue = r.Grades.FirstOrDefault()?.GradeValue,
				Semester = r.CourseOffering.Term.TermName,
				IsPassed = false
			}).ToList();
		}

		public async Task<List<StudentTranscriptSummary>> GetStudentsByGPA(decimal minGPA, decimal? maxGPA = null)
		{
			// Get all students with their transcripts
			var students = await _unitOfWork.GetRepository<Student>()
				.GetListAsync(
					include: q => q
						.Include(s => s.User)
						.Include(s => s.Program)
						.Include(s => s.Registrations)
							.ThenInclude(r => r.Grades)
						.Include(s => s.Registrations)
							.ThenInclude(r => r.CourseOffering)
							.ThenInclude(co => co.Course)
				);

			var results = new List<StudentTranscriptSummary>();

			foreach (var student in students)
			{
				var gradePoints = 0m;
				var totalCredits = 0;

				foreach (var registration in student.Registrations.Where(r => r.Grades.Any()))
				{
					var credits = registration.CourseOffering.Course.Credits;
					var grade = registration.Grades.FirstOrDefault()?.QualityPoints ?? 0;

					gradePoints += credits * grade;
					totalCredits += credits;
				}

				var gpa = totalCredits > 0 ? gradePoints / totalCredits : 0;

				// Filter by GPA range
				if (gpa >= minGPA && (!maxGPA.HasValue || gpa <= maxGPA.Value))
				{
					results.Add(new StudentTranscriptSummary
					{
						StudentId = student.Id,
						Mssv = student.Mssv,
						StudentName = student.User.FullName,
						ProgramName = student.Program.ProgramName,
						GPA = Math.Round(gpa, 2),
						TotalCredits = totalCredits
					});
				}
			}

			return results.OrderByDescending(s => s.GPA).ToList();
		}

		public async Task<StudentTuitionResponse> GetStudentTuition(Guid studentId)
		{
			// First, get the student with User and Program
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(
					predicate: s => s.Id == studentId,
					include: q => q
						.Include(s => s.User)
						.Include(s => s.Program)
				);

			if (student == null)
				return null;

			// Then, get the tuition records separately with Term information
			var studentTuitions = await _unitOfWork.GetRepository<StudentTuition>()
				.GetListAsync(
					predicate: st => st.StudentId == studentId,
					include: q => q
						.Include(st => st.TuitionPolicy)
						.Include(st => st.Term)
				);

			var tuitions = studentTuitions.Select(st => new TuitionInfo
			{
				TuitionPolicyId = st.TuitionPolicyId,
				PolicyName = st.TuitionPolicy.PolicyName,
				Description = st.TuitionPolicy.Description,
				Amount = st.TotalAmount,
				EffectiveDate = st.TuitionPolicy.EffectiveDate,
				ExpirationDate = st.TuitionPolicy.ExpirationDate,
				Semester = st.Term.TermName,
				AmountPaid = st.AmountPaid,
				DiscountAmount = st.DiscountAmount ?? 0,
				PaymentStatus = st.PaymentStatus,
				PaymentDueDate = st.PaymentDueDate,
				IsPaid = st.PaymentStatus == "Paid",
				PaymentDate = st.PaymentDate.HasValue ? DateOnly.FromDateTime(st.PaymentDate.Value) : null
			}).ToList();

			var totalAmount = studentTuitions.Sum(t => t.TotalAmount);
			var paidAmount = studentTuitions.Sum(t => t.AmountPaid);

			return new StudentTuitionResponse
			{
				StudentId = student.Id,
				Mssv = student.Mssv,
				StudentName = student.User.FullName,
				Tuitions = tuitions,
				TotalAmount = totalAmount,
				PaidAmount = paidAmount,
				RemainingAmount = totalAmount - paidAmount
			};
		}

		public async Task<List<StudentScholarshipResponse>> GetAllStudentScholarships()
		{
			var studentScholarships = await _unitOfWork.GetRepository<StudentScholarship>()
				.GetListAsync(
					include: q => q
						.Include(ss => ss.Student)
							.ThenInclude(s => s.User)
						.Include(ss => ss.Scholarship)
				);

			return studentScholarships
				.Select(ss => new StudentScholarshipResponse
				{
					Mssv = ss.Student.Mssv,
					FullName = ss.Student.User.FullName,
					ScholarshipName = ss.Scholarship.ScholarshipName,
					Amount = ss.Scholarship.Amount,
					AwardDate = ss.AwardDate
				}).ToList();
		}

		public async Task<List<StudentProgramCourseResponse>> GetAllStudentProgramsAndCourses()
		{
			var students = await _unitOfWork.GetRepository<Student>()
				.GetListAsync(
					include: q => q
						.Include(s => s.User)
						.Include(s => s.Program)
							.ThenInclude(p => p.Faculty)
						.Include(s => s.Program)
							.ThenInclude(p => p.Courses)
				);

			return students.Select(student => new StudentProgramCourseResponse
			{
				Mssv = student.Mssv,
				StudentName = student.User.FullName,
				ProgramName = student.Program.ProgramName,
				Courses = student.Program.Courses.Select(c => new CourseInfo
				{
					CourseId = c.Id,
					CourseCode = c.CourseCode,
					CourseName = c.CourseName,
					Credits = c.Credits,
					Description = c.Description
				}).ToList()
			}).ToList();
		}

		public async Task<List<StudentTranscriptResponse>> GetAllStudentTranscripts()
		{
			var students = await _unitOfWork.GetRepository<Student>()
				.GetListAsync(
					include: q => q
						.Include(s => s.User)
						.Include(s => s.Program)
						.Include(s => s.Registrations)
							.ThenInclude(r => r.CourseOffering)
							.ThenInclude(co => co.Course)
						.Include(s => s.Registrations)
							.ThenInclude(r => r.CourseOffering)
							.ThenInclude(co => co.Term)
						.Include(s => s.Registrations)
							.ThenInclude(r => r.Grades)
				);

			return students.Select(student =>
			{
				var courseGrades = student.Registrations
					.Where(r => r.Grades.Any())
					.Select(r => new CourseGrade
					{
						CourseCode = r.CourseOffering.Course.CourseCode,
						CourseName = r.CourseOffering.Course.CourseName,
						Credits = r.CourseOffering.Course.Credits,
						Grade = r.Grades.FirstOrDefault()?.QualityPoints,
						GradeValue = r.Grades.FirstOrDefault()?.GradeValue,
						Semester = r.CourseOffering.Term.TermName,
						IsPassed = r.Grades.Any(g => g.QualityPoints >= 1.0m) // Consider 1.0 (D) as passing
					}).ToList();

				// Group courses by term for term-specific calculations
				var coursesByTerm = courseGrades
					.GroupBy(c => c.Semester)
					.Select(termGroup =>
					{
						var termCourses = termGroup.ToList();
						var termCredits = termCourses.Sum(c => c.Credits);
						var termGPA = termCredits > 0
							? termCourses.Sum(c => c.Credits * c.Grade.GetValueOrDefault()) / termCredits
							: 0;

						return new TermGrades
						{
							TermName = termGroup.Key,
							Courses = termCourses,
							TermGPA = Math.Round(termGPA, 2),
							TermCredits = termCredits
						};
					})
					.OrderBy(t => t.TermName)
					.ToList();

				// Calculate cumulative statistics
				var totalCredits = courseGrades.Sum(c => c.Credits);
				var totalCreditsPassed = courseGrades.Where(c => c.IsPassed).Sum(c => c.Credits);
				var cumulativeGPA = totalCredits > 0
					? courseGrades.Sum(c => c.Credits * c.Grade.GetValueOrDefault()) / totalCredits
					: 0;

				return new StudentTranscriptResponse
				{
					Mssv = student.Mssv,
					StudentName = student.User.FullName,
					ProgramName = student.Program.ProgramName,
					Terms = coursesByTerm,
					CourseGrades = courseGrades, // Keep for backward compatibility
					CumulativeGPA = Math.Round(cumulativeGPA, 2),
					TotalCredits = totalCredits,
					TotalCreditsPassed = totalCreditsPassed
				};
			}).ToList();
		}

		public async Task<List<StudentTuitionResponse>> GetAllStudentTuitions()
		{
			// Fetch all student tuitions with related data in a single query
			var studentTuitions = await _unitOfWork.GetRepository<StudentTuition>()
				.GetListAsync(
					include: q => q
						.Include(st => st.Student)
							.ThenInclude(s => s.User)
						.Include(st => st.Term)
						.Include(st => st.TuitionPolicy)
				);

			// Group by student to create one response per student
			var groupedTuitions = studentTuitions
				.GroupBy(st => st.StudentId)
				.Select(group =>
				{
					var student = group.First().Student;

					var tuitions = group.Select(st => new TuitionInfo
					{
						TuitionPolicyId = st.TuitionPolicyId,
						PolicyName = st.TuitionPolicy.PolicyName,
						Description = st.TuitionPolicy.Description,
						Amount = st.TotalAmount,
						EffectiveDate = st.TuitionPolicy.EffectiveDate,
						ExpirationDate = st.TuitionPolicy.ExpirationDate,
						Semester = st.Term.TermName,
						AmountPaid = st.AmountPaid,
						DiscountAmount = st.DiscountAmount ?? 0,
						PaymentStatus = st.PaymentStatus,
						PaymentDueDate = st.PaymentDueDate,
						IsPaid = st.PaymentStatus == "Paid",
						PaymentDate = st.PaymentDate.HasValue ? DateOnly.FromDateTime(st.PaymentDate.Value) : null
					}).ToList();

					var totalAmount = tuitions.Sum(t => t.Amount);
					var paidAmount = tuitions.Sum(t => t.AmountPaid);

					return new StudentTuitionResponse
					{
						StudentId = student.Id,
						Mssv = student.Mssv,
						StudentName = student.User.FullName,
						Tuitions = tuitions,
						TotalAmount = totalAmount,
						PaidAmount = paidAmount,
						RemainingAmount = totalAmount - paidAmount
					};
				}).ToList();

			return groupedTuitions;
		}

		public async Task<StudentInfoResponse> CreateStudent(StudentCreateRequest request)
		{
			try
			{

				var existingUser = await _unitOfWork.GetRepository<User>()
					.SingleOrDefaultAsync(predicate: u => u.Email == request.Email);

				if (existingUser != null)
					throw new BadHttpRequestException("Email already exists");

				var role = await _unitOfWork.GetRepository<Role>()
					.SingleOrDefaultAsync(predicate: r => r.RoleName.ToLower() == "student");

				if (role == null)
					throw new BadHttpRequestException("Student role not found");

				var program = await _unitOfWork.GetRepository<Program>()
					.SingleOrDefaultAsync(predicate: p => p.Id == request.ProgramId);

				if (program == null)
					throw new BadHttpRequestException("Program not found");

				var newUser = new User
				{
					Id = Guid.NewGuid(),
					Email = request.Email,
					Password = PasswordUtil.HashPassword(request.Password),
					FullName = request.FullName,
					RoleId = role.Id,
					Image = request.ImageUrl
				};

				await _unitOfWork.GetRepository<User>().InsertAsync(newUser);

				var newStudent = new Student
				{
					Id = Guid.NewGuid(),
					UserId = newUser.Id,
					ProgramId = request.ProgramId,
					Mssv = MSSVGeneration.GenerateStudentId(),
					EnrollmentDate = DateOnly.FromDateTime(DateTime.Now),
					AdmissionDate = request.AdmissionDate,
					AdmissionStatus = request.AdmissionStatus
				};

				await _unitOfWork.GetRepository<Student>().InsertAsync(newStudent);

				// Add scholarship if provided
				if (request.ScholarshipId.HasValue && request.ScholarshipAwardDate.HasValue)
				{
					// Validate the scholarship exists
					var scholarship = await _unitOfWork.GetRepository<Scholarship>()
						.SingleOrDefaultAsync(predicate: s => s.Id == request.ScholarshipId.Value);

					if (scholarship == null)
						throw new BadHttpRequestException("Scholarship not found");

					var studentScholarship = new StudentScholarship
					{
						StudentId = newStudent.Id,
						ScholarshipId = request.ScholarshipId.Value,
						AwardDate = request.ScholarshipAwardDate.Value
					};

					await _unitOfWork.GetRepository<StudentScholarship>().InsertAsync(studentScholarship);
				}

				// Commit transaction
				await _unitOfWork.CommitAsync();

				return new StudentInfoResponse
				{
					Id = newStudent.Id,
					Mssv = newStudent.Mssv,
					FullName = newUser.FullName,
					Email = newUser.Email,
					ProgramName = program.ProgramName,
					EnrollmentDate = newStudent.EnrollmentDate,
					AdmissionDate = newStudent.AdmissionDate,
					AdmissionStatus = newStudent.AdmissionStatus,
					ImageUrl = newUser.Image
				};
			}
			catch (Exception ex)
			{
				// Ensure rollback on any error
				_logger.LogError(ex, "Error creating student: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<StudentInfoResponse> UpdateStudent(Guid studentId, StudentUpdateRequest request)
		{
			try
			{
				var student = await _unitOfWork.GetRepository<Student>()
					.SingleOrDefaultAsync(
						predicate: s => s.Id == studentId,
						include: q => q.Include(s => s.User)
								   .Include(s => s.Program)
					);

				if (student == null)
					return null;

				// Update user info
				if (!string.IsNullOrEmpty(request.Email) && student.User.Email != request.Email)
				{
					// Check if email already exists
					var existingUser = await _unitOfWork.GetRepository<User>()
						.SingleOrDefaultAsync(predicate: u => u.Email == request.Email && u.Id != student.UserId);

					if (existingUser != null)
						throw new BadHttpRequestException("Email already exists");

					student.User.Email = request.Email;
				}

				if (!string.IsNullOrEmpty(request.FullName))
					student.User.FullName = request.FullName;

				if (!string.IsNullOrEmpty(request.ImageUrl))
					student.User.Image = request.ImageUrl;

				// Update student info
				if (!string.IsNullOrEmpty(request.Mssv) && student.Mssv != request.Mssv)
				{
					// Check if MSSV already exists
					var existingStudent = await _unitOfWork.GetRepository<Student>()
						.SingleOrDefaultAsync(predicate: s => s.Mssv == request.Mssv && s.Id != studentId);

					if (existingStudent != null)
						throw new BadHttpRequestException("Student ID already exists");

					student.Mssv = request.Mssv;
				}

				if (request.EnrollmentDate.HasValue)
					student.EnrollmentDate = request.EnrollmentDate.Value;

				if (request.ProgramId.HasValue && request.ProgramId != student.ProgramId)
				{
					var program = await _unitOfWork.GetRepository<Program>()
						.SingleOrDefaultAsync(predicate: p => p.Id == request.ProgramId);

					if (program == null)
						throw new BadHttpRequestException("Program not found");

					student.ProgramId = request.ProgramId.Value;
					student.Program = program;
				}

				if (request.AdmissionDate.HasValue)
					student.AdmissionDate = request.AdmissionDate;

				if (!string.IsNullOrEmpty(request.AdmissionStatus))
					student.AdmissionStatus = request.AdmissionStatus;

				await _unitOfWork.CommitAsync();

				return new StudentInfoResponse
				{
					Id = student.Id,
					Mssv = student.Mssv,
					FullName = student.User.FullName,
					Email = student.User.Email,
					ProgramName = student.Program.ProgramName,
					EnrollmentDate = student.EnrollmentDate,
					AdmissionDate = student.AdmissionDate,
					AdmissionStatus = student.AdmissionStatus,
					ImageUrl = student.User.Image
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating student: {Message}", ex.Message);
				throw;
			}
		}



		public async Task<StudentFinancialInfoResponse> AssignScholarshipToStudent(Guid studentId, ScholarshipAssignmentRequest request)
		{
			try
			{
				var student = await _unitOfWork.GetRepository<Student>()
					.SingleOrDefaultAsync(predicate: s => s.Id == studentId);

				if (student == null)
					return null;

				var scholarship = await _unitOfWork.GetRepository<Scholarship>()
					.SingleOrDefaultAsync(predicate: s => s.Id == request.ScholarshipId);

				if (scholarship == null)
					throw new BadHttpRequestException("Scholarship not found");

				// Check if this scholarship is already assigned
				var existingAssignment = await _unitOfWork.GetRepository<StudentScholarship>()
					.SingleOrDefaultAsync(predicate: ss => ss.StudentId == studentId && ss.ScholarshipId == request.ScholarshipId);

				if (existingAssignment != null)
					throw new BadHttpRequestException("Scholarship already assigned to this student");

				var newAssignment = new StudentScholarship
				{
					StudentId = studentId,
					ScholarshipId = request.ScholarshipId,
					AwardDate = request.AwardDate
				};

				await _unitOfWork.GetRepository<StudentScholarship>().InsertAsync(newAssignment);
				await _unitOfWork.CommitAsync();


				return await GetStudentFinancialInfo(studentId);
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Error assigning scholarship to student: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<StudentFinancialInfoResponse> AssignFinancialAidToStudent(Guid studentId, FinancialAidAssignmentRequest request)
		{
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(predicate: s => s.Id == studentId);

			if (student == null)
				return null;

			var financialAid = await _unitOfWork.GetRepository<FinancialAid>()
				.SingleOrDefaultAsync(predicate: fa => fa.Id == request.FinancialAidId);

			if (financialAid == null)
				throw new BadHttpRequestException("Financial aid not found");

			// Check if this financial aid is already assigned
			var existingAssignment = await _unitOfWork.GetRepository<StudentFinancialAid>()
				.SingleOrDefaultAsync(predicate: sfa => sfa.StudentId == studentId && sfa.FinancialAidId == request.FinancialAidId);

			if (existingAssignment != null)
				throw new BadHttpRequestException("Financial aid already assigned to this student");

			var newAssignment = new StudentFinancialAid
			{
				StudentId = studentId,
				FinancialAidId = request.FinancialAidId,
				AwardDate = request.AwardDate
			};

			await _unitOfWork.GetRepository<StudentFinancialAid>().InsertAsync(newAssignment);
			await _unitOfWork.CommitAsync();

			return await GetStudentFinancialInfo(studentId);
		}





		public async Task<StudentInfoResponse> UpdateStudentProgram(Guid studentId, Guid programId)
		{
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(
					predicate: s => s.Id == studentId,
					include: q => q.Include(s => s.User)
				);

			if (student == null)
				return null;

			var program = await _unitOfWork.GetRepository<Program>()
				.SingleOrDefaultAsync(predicate: p => p.Id == programId);

			if (program == null)
				throw new BadHttpRequestException("Program not found");

			student.ProgramId = programId;
			await _unitOfWork.CommitAsync();

			return new StudentInfoResponse
			{
				Id = student.Id,
				Mssv = student.Mssv,
				FullName = student.User.FullName,
				Email = student.User.Email,
				ProgramName = program.ProgramName,
				EnrollmentDate = student.EnrollmentDate,
				ImageUrl = student.User.Image
			};
		}

		public async Task<StudentTuitionResponse> CreateStudentTuition(Guid studentId, StudentTuitionCreateRequest request)
		{
			try
			{
				var student = await _unitOfWork.GetRepository<Student>()
					.SingleOrDefaultAsync(predicate: s => s.Id == studentId);

				if (student == null)
					return null;

				var tuitionPolicy = await _unitOfWork.GetRepository<TuitionPolicy>()
					.SingleOrDefaultAsync(predicate: tp => tp.Id == request.TuitionPolicyId);

				if (tuitionPolicy == null)
					throw new BadHttpRequestException("Tuition policy not found");

				// Get term from semester name
				var term = await _unitOfWork.GetRepository<Term>()
					.SingleOrDefaultAsync(predicate: t => t.TermName == request.Semester);

				if (term == null)
					throw new BadHttpRequestException("Term not found");

				// Calculate payment due date (30 days from now by default)
				var paymentDueDate = request.PaymentDueDate ?? DateOnly.FromDateTime(DateTime.Now.AddDays(30));

				var newTuition = new StudentTuition
				{
					Id = Guid.NewGuid(), // Generate a new ID
					StudentId = studentId,
					TermId = term.Id,
					TuitionPolicyId = request.TuitionPolicyId,
					TotalAmount = tuitionPolicy.Amount,
					AmountPaid = 0, // Initially unpaid
					PaymentStatus = "Unpaid", // Always start as unpaid as per the SQL
					PaymentDueDate = paymentDueDate,
					PaymentDate = null // No payment date initially
				};

				await _unitOfWork.GetRepository<StudentTuition>().InsertAsync(newTuition);
				await _unitOfWork.CommitAsync();


				return await GetStudentTuition(studentId);
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Error creating student tuition: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<StudentTuitionResponse> UpdateStudentTuition(Guid studentId, Guid tuitionId, StudentTuitionUpdateRequest request)
		{
			try
			{
				var studentTuition = await _unitOfWork.GetRepository<StudentTuition>()
					.SingleOrDefaultAsync(predicate: st => st.Id == tuitionId && st.StudentId == studentId);

				if (studentTuition == null)
					return null;

				// If tuition policy changed, update related values
				if (request.TuitionPolicyId != studentTuition.TuitionPolicyId)
				{
					var tuitionPolicy = await _unitOfWork.GetRepository<TuitionPolicy>()
						.SingleOrDefaultAsync(predicate: tp => tp.Id == request.TuitionPolicyId);

					if (tuitionPolicy == null)
						throw new BadHttpRequestException("Tuition policy not found");

					studentTuition.TuitionPolicyId = request.TuitionPolicyId;
					studentTuition.TotalAmount = tuitionPolicy.Amount;
				}

				// Get term from semester name
				if (!string.IsNullOrEmpty(request.Semester))
				{
					var term = await _unitOfWork.GetRepository<Term>()
						.SingleOrDefaultAsync(predicate: t => t.TermName == request.Semester);

					if (term == null)
						throw new BadHttpRequestException("Term not found");

					studentTuition.TermId = term.Id;
				}

				// Update payment due date if provided
				if (request.PaymentDueDate.HasValue)
				{
					studentTuition.PaymentDueDate = request.PaymentDueDate.Value;
				}

				// Handle payment status
				if (request.IsPaid)
				{
					// If fully paid
					studentTuition.AmountPaid = studentTuition.TotalAmount;
					studentTuition.PaymentStatus = "Paid";
					studentTuition.PaymentDate = request.PaymentDate.HasValue
						? DateTime.Parse(request.PaymentDate.Value.ToString())
						: DateTime.Now;
				}
				else if (request.AmountPaid.HasValue)
				{
					// Handle partial payment
					studentTuition.AmountPaid = request.AmountPaid.Value;

					if (request.AmountPaid.Value >= studentTuition.TotalAmount)
					{
						studentTuition.PaymentStatus = "Paid";
					}
					else if (request.AmountPaid.Value > 0)
					{
						studentTuition.PaymentStatus = "Partial";
					}
					else
					{
						studentTuition.PaymentStatus = "Unpaid";
						studentTuition.PaymentDate = null;
					}
				}

				// Update discount if provided
				if (request.DiscountAmount.HasValue)
				{
					studentTuition.DiscountAmount = request.DiscountAmount.Value;
				}

				// Update notes if provided
				if (!string.IsNullOrEmpty(request.Notes))
				{
					studentTuition.Notes = request.Notes;
				}

				await _unitOfWork.CommitAsync();


				return await GetStudentTuition(studentId);
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Error updating student tuition: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<StudentProgramResponse> CreateStudentProgramCourses(Guid studentId, StudentProgramCoursesCreateRequest request)
		{
			try
			{


				// Get student with full details
				var student = await _unitOfWork.GetRepository<Student>()
					.SingleOrDefaultAsync(
						predicate: s => s.Id == studentId,
						include: q => q.Include(s => s.User)
					);

				if (student == null)
					return null;

				// Get program with courses
				var program = await _unitOfWork.GetRepository<Program>()
					.SingleOrDefaultAsync(
						predicate: p => p.Id == request.ProgramId,
						include: q => q
							.Include(p => p.Faculty)
							.Include(p => p.Courses)
					);

				if (program == null)
					throw new BadHttpRequestException("Program not found");

				// Update student program
				student.ProgramId = program.Id;

				// Get term for enrollment
				var term = await _unitOfWork.GetRepository<Term>()
					.SingleOrDefaultAsync(predicate: t => t.TermName == request.EnrollmentSemester);

				if (term == null)
					throw new BadHttpRequestException("Term not found");

				// Get courses to enroll
				var coursesToEnroll = program.Courses
					.Where(c => !request.SelectedCourseIds.Any() || request.SelectedCourseIds.Contains(c.Id))
					.ToList();

				if (!coursesToEnroll.Any() && request.SelectedCourseIds.Any())
					throw new BadHttpRequestException("No valid courses selected for enrollment");

				// Get course offerings for the term
				var courseOfferings = await _unitOfWork.GetRepository<CourseOffering>()
					.GetListAsync(
						predicate: co => co.TermId == term.Id &&
										coursesToEnroll.Select(c => c.Id).Contains(co.CourseId)
					);

				// Create registrations for each course offering
				foreach (var offering in courseOfferings)
				{
					// Check if registration already exists
					var existingRegistration = await _unitOfWork.GetRepository<Registration>()
						.SingleOrDefaultAsync(
							predicate: r => r.StudentId == studentId &&
											r.CourseOfferingId == offering.Id
						);

					if (existingRegistration == null)
					{
						var registration = new Registration
						{
							Id = Guid.NewGuid(),
							StudentId = studentId,
							CourseOfferingId = offering.Id,
							RegistrationDate = DateTime.Now,
							Status = "Registered"
						};

						await _unitOfWork.GetRepository<Registration>().InsertAsync(registration);
					}
				}

				await _unitOfWork.CommitAsync();


				// Return updated program info
				return new StudentProgramResponse
				{
					ProgramId = program.Id,
					ProgramName = program.ProgramName,
					RequiredCredits = program.RequiredCredits,
					FacultyName = program.Faculty.FacultyName,
					Courses = coursesToEnroll.Select(c => new CourseInfo
					{
						CourseId = c.Id,
						CourseCode = c.CourseCode,
						CourseName = c.CourseName,
						Credits = c.Credits,
						Description = c.Description
					}).ToList()
				};
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Error creating student program and courses: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<StudentProgramResponse> UpdateStudentProgramCourses(Guid studentId, StudentProgramCoursesUpdateRequest request)
		{
			try
			{

				var student = await _unitOfWork.GetRepository<Student>()
					.SingleOrDefaultAsync(
						predicate: s => s.Id == studentId,
						include: q => q
							.Include(s => s.User)
							.Include(s => s.Registrations)
					);

				if (student == null)
					return null;

				// Get program with courses
				var program = await _unitOfWork.GetRepository<Program>()
					.SingleOrDefaultAsync(
						predicate: p => p.Id == request.ProgramId,
						include: q => q
							.Include(p => p.Faculty)
							.Include(p => p.Courses)
					);

				if (program == null)
					throw new BadHttpRequestException("Program not found");

				// Update student program
				student.ProgramId = program.Id;

				// Get term for enrollment
				var term = await _unitOfWork.GetRepository<Term>()
					.SingleOrDefaultAsync(predicate: t => t.TermName == request.EnrollmentSemester);

				if (term == null)
					throw new BadHttpRequestException("Term not found");

				// Get course offerings for the term
				var courseOfferings = await _unitOfWork.GetRepository<CourseOffering>()
					.GetListAsync(
						predicate: co => co.TermId == term.Id &&
										program.Courses.Select(c => c.Id).Contains(co.CourseId)
					);

				// If replacing all courses, drop existing registrations for this term
				if (request.ReplaceAllCourses)
				{
					var existingRegistrationsForTerm = await _unitOfWork.GetRepository<Registration>()
						.GetListAsync(
							predicate: r => r.StudentId == studentId &&
										   r.CourseOffering.TermId == term.Id
						);

					foreach (var registration in existingRegistrationsForTerm)
					{
						_unitOfWork.GetRepository<Registration>().DeleteAsync(registration);
					}
				}

				// Remove specific courses if specified
				if (request.CoursesToRemove.Any())
				{
					var registrationsToRemove = await _unitOfWork.GetRepository<Registration>()
						.GetListAsync(
							predicate: r => r.StudentId == studentId &&
										   r.CourseOffering.TermId == term.Id &&
										   request.CoursesToRemove.Contains(r.CourseOffering.CourseId)
						);

					foreach (var registration in registrationsToRemove)
					{
						_unitOfWork.GetRepository<Registration>().DeleteAsync(registration);
					}
				}

				// Add new courses
				var coursesToEnroll = program.Courses
					.Where(c => request.CoursesToAdd.Contains(c.Id))
					.ToList();

				if (!coursesToEnroll.Any() && request.CoursesToAdd.Any())
					throw new BadHttpRequestException("No valid courses selected for enrollment");

				// Create registrations for each course to add
				foreach (var courseId in request.CoursesToAdd)
				{
					var offering = courseOfferings.FirstOrDefault(co => co.CourseId == courseId);

					if (offering != null)
					{
						// Check if registration already exists
						var existingRegistration = await _unitOfWork.GetRepository<Registration>()
							.SingleOrDefaultAsync(
								predicate: r => r.StudentId == studentId &&
											  r.CourseOfferingId == offering.Id
							);

						if (existingRegistration == null)
						{
							var registration = new Registration
							{
								Id = Guid.NewGuid(),
								StudentId = studentId,
								CourseOfferingId = offering.Id,
								RegistrationDate = DateTime.Now,
								Status = "Registered"
							};

							await _unitOfWork.GetRepository<Registration>().InsertAsync(registration);
						}
					}
				}

				// Commit all changes
				await _unitOfWork.CommitAsync();

				// Return updated program info with all current courses
				var updatedStudent = await _unitOfWork.GetRepository<Student>()
					.SingleOrDefaultAsync(
						predicate: s => s.Id == studentId,
						include: q => q
							.Include(s => s.Program)
								.ThenInclude(p => p.Faculty)
							.Include(s => s.Program)
								.ThenInclude(p => p.Courses)
					);

				return new StudentProgramResponse
				{
					ProgramId = updatedStudent.Program.Id,
					ProgramName = updatedStudent.Program.ProgramName,
					RequiredCredits = updatedStudent.Program.RequiredCredits,
					FacultyName = updatedStudent.Program.Faculty.FacultyName,
					Courses = updatedStudent.Program.Courses.Select(c => new CourseInfo
					{
						CourseId = c.Id,
						CourseCode = c.CourseCode,
						CourseName = c.CourseName,
						Credits = c.Credits,
						Description = c.Description
					}).ToList()
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating student program and courses: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<bool> DeleteStudentProgramAndCourses(Guid studentId, string semester = null)
		{
			try
			{
				// Begin transaction

				var student = await _unitOfWork.GetRepository<Student>()
					.SingleOrDefaultAsync(
						predicate: s => s.Id == studentId,
						include: q => q.Include(s => s.Registrations)
					);

				if (student == null)
					return false;

				// Get registrations to delete (either for specific semester or all)
				var registrationsQuery = _unitOfWork.GetRepository<Registration>()
					.GetListAsync(predicate: r => r.StudentId == studentId);

				if (!string.IsNullOrEmpty(semester))
				{
					// Get term ID for the specified semester
					var term = await _unitOfWork.GetRepository<Term>()
						.SingleOrDefaultAsync(predicate: t => t.TermName == semester);

					if (term == null)
						throw new BadHttpRequestException($"Term '{semester}' not found");

					var registrations = await registrationsQuery;
					registrations = registrations.Where(r => r.CourseOffering.TermId == term.Id).ToList();
				}

				var registrationsToDelete = await _unitOfWork.GetRepository<Registration>()
					.GetListAsync(predicate: r => r.StudentId == studentId, include: q => q.Include(r => r.CourseOffering));

				// Delete all registrations
				foreach (var registration in registrationsToDelete)
				{
					_unitOfWork.GetRepository<Registration>().DeleteAsync(registration);
				}

				// Reset program assignment if all courses are being deleted and no semester specified
				if (string.IsNullOrEmpty(semester))
				{
					// Since we're removing all courses, we should also clear the program
					// However, this is optional based on business requirements
					student.ProgramId = Guid.Empty; // This assumes ProgramId can be reset to an empty Guid, adjust if needed
				}

				await _unitOfWork.CommitAsync();


				return true;
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Error deleting student program and courses: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<List<StudentInfoResponse>> GetStudentsByEnrollmentYear(int year)
		{
			var students = await _unitOfWork.GetRepository<Student>()
				.GetListAsync(
					predicate: s => s.EnrollmentDate.Year == year,
					include: q => q.Include(s => s.User)
							  .Include(s => s.Program)
				);

			return students.Select(student => new StudentInfoResponse
			{
				Id = student.Id,
				Mssv = student.Mssv,
				FullName = student.User.FullName,
				Email = student.User.Email,
				ProgramName = student.Program.ProgramName,
				EnrollmentDate = student.EnrollmentDate,
				AdmissionDate = student.AdmissionDate,
				AdmissionStatus = student.AdmissionStatus,
				ImageUrl = student.User.Image
			}).ToList();
		}

		public async Task<List<StudentInfoResponse>> GetStudentsByProgram(Guid programId)
		{
			var students = await _unitOfWork.GetRepository<Student>()
				.GetListAsync(
					predicate: s => s.ProgramId == programId,
					include: q => q.Include(s => s.User)
							  .Include(s => s.Program)
				);

			return students.Select(student => new StudentInfoResponse
			{
				Id = student.Id,
				Mssv = student.Mssv,
				FullName = student.User.FullName,
				Email = student.User.Email,
				ProgramName = student.Program.ProgramName,
				EnrollmentDate = student.EnrollmentDate,
				AdmissionDate = student.AdmissionDate,
				AdmissionStatus = student.AdmissionStatus,
				ImageUrl = student.User.Image
			}).ToList();
		}

		public async Task<List<StudentInfoResponse>> GetStudentsByScholarship(string scholarshipName)
		{
			var studentScholarships = await _unitOfWork.GetRepository<StudentScholarship>()
				.GetListAsync(
					predicate: ss => ss.Scholarship.ScholarshipName == scholarshipName,
					include: q => q
						.Include(ss => ss.Student)
							.ThenInclude(s => s.User)
						.Include(ss => ss.Scholarship)
				);

			return studentScholarships.Select(ss => new StudentInfoResponse
			{
				Id = ss.Student.Id,
				Mssv = ss.Student.Mssv,
				FullName = ss.Student.User.FullName,
				Email = ss.Student.User.Email,
				ProgramName = ss.Student.Program.ProgramName,
				EnrollmentDate = ss.Student.EnrollmentDate,
				AdmissionDate = ss.Student.AdmissionDate,
				AdmissionStatus = ss.Student.AdmissionStatus,
				ImageUrl = ss.Student.User.Image
			}).ToList();
		}


	}
}