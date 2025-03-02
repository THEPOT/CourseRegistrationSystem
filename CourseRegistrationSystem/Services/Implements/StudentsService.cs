using AutoMapper;
using CourseRegistration_API.Helpers;
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
				ImageUrl = student.User.Image
			}).ToList();
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
							.ThenInclude(r => r.Grades) // Include grades collection
				);

			if (student == null)
				return null;

			var courseGrades = student.Registrations
				.Where(r => r.Grades.Any()) // Only include registrations with grades
				.Select(r => new CourseGrade
				{
					CourseCode = r.CourseOffering.Course.CourseCode,
					CourseName = r.CourseOffering.Course.CourseName,
					Credits = r.CourseOffering.Course.Credits,
					// Get the quality points from the first grade
					Grade = r.Grades.FirstOrDefault()?.QualityPoints,
					// Use the term name for the semester
					Semester = r.CourseOffering.Term.TermName
				}).ToList();

			// Calculate GPA and total credits
			var totalCredits = courseGrades.Sum(c => c.Credits);
			var gpa = courseGrades.Any()
				? courseGrades.Sum(c => c.Credits * c.Grade.GetValueOrDefault()) / totalCredits
				: 0;

			return new StudentTranscriptResponse
			{
				Mssv = student.Mssv,
				StudentName = student.User.FullName,
				ProgramName = student.Program.ProgramName,
				CourseGrades = courseGrades,
				GPA = Math.Round(gpa, 2),
				TotalCredits = totalCredits
			};
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
				Amount = st.TotalAmount, // Use actual amount from StudentTuition
				EffectiveDate = st.TuitionPolicy.EffectiveDate,
				ExpirationDate = st.TuitionPolicy.ExpirationDate,
				Semester = st.Term.TermName, // Use Term name instead of direct semester
				IsPaid = st.PaymentStatus == "Paid", // Convert payment status to boolean
				PaymentDate = st.PaymentDate.HasValue ? DateOnly.FromDateTime(st.PaymentDate.Value) : null
			}).ToList();

			var totalAmount = studentTuitions.Sum(t => t.TotalAmount);
			var paidAmount = studentTuitions.Where(t => t.PaymentStatus == "Paid").Sum(t => t.AmountPaid);

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

		public async Task<List<StudentFinancialInfoResponse>> GetAllStudentScholarships()
		{
			var students = await _unitOfWork.GetRepository<Student>()
				.GetListAsync(
					include: q => q
						.Include(s => s.StudentScholarships)
							.ThenInclude(ss => ss.Scholarship)
						.Include(s => s.StudentFinancialAids)
							.ThenInclude(sf => sf.FinancialAid)
				);

			return students.Select(student => new StudentFinancialInfoResponse
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
			}).ToList();
		}

		public async Task<List<StudentProgramResponse>> GetAllStudentProgramsAndCourses()
		{
			var students = await _unitOfWork.GetRepository<Student>()
				.GetListAsync(
					include: q => q
						.Include(s => s.Program)
							.ThenInclude(p => p.Faculty)
						.Include(s => s.Program)
							.ThenInclude(p => p.Courses)
				);

			return students.Select(student => new StudentProgramResponse
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
							.ThenInclude(r => r.Grades) // Include grades collection
				);

			return students.Select(student =>
			{
				var courseGrades = student.Registrations
					.Where(r => r.Grades.Any()) // Only include registrations with grades
					.Select(r => new CourseGrade
					{
						CourseCode = r.CourseOffering.Course.CourseCode,
						CourseName = r.CourseOffering.Course.CourseName,
						Credits = r.CourseOffering.Course.Credits,
						// Get the quality points from the first grade
						Grade = r.Grades.FirstOrDefault()?.QualityPoints,
						// Use the term name for the semester
						Semester = r.CourseOffering.Term.TermName
					}).ToList();

				var totalCredits = courseGrades.Sum(c => c.Credits);
				var gpa = courseGrades.Any()
					? courseGrades.Sum(c => c.Credits * c.Grade.GetValueOrDefault()) / totalCredits
					: 0;

				return new StudentTranscriptResponse
				{
					Mssv = student.Mssv,
					StudentName = student.User.FullName,
					ProgramName = student.Program.ProgramName,
					CourseGrades = courseGrades,
					GPA = Math.Round(gpa, 2),
					TotalCredits = totalCredits
				};
			}).ToList();
		}

		public async Task<List<StudentTuitionResponse>> GetAllStudentTuitions()
		{
			var students = await _unitOfWork.GetRepository<Student>()
				.GetListAsync(
					include: q => q
						.Include(s => s.User)
						.Include(s => s.Program)
				);

			var responses = new List<StudentTuitionResponse>();

			foreach (var student in students)
			{
				// Get the tuition records separately with Term information
				var studentTuitions = await _unitOfWork.GetRepository<StudentTuition>()
					.GetListAsync(
						predicate: st => st.StudentId == student.Id,
						include: q => q
							.Include(st => st.TuitionPolicy)
							.Include(st => st.Term)
					);

				var tuitions = studentTuitions.Select(st => new TuitionInfo
				{
					TuitionPolicyId = st.TuitionPolicyId,
					PolicyName = st.TuitionPolicy.PolicyName,
					Description = st.TuitionPolicy.Description,
					Amount = st.TotalAmount, // Use actual amount from StudentTuition
					EffectiveDate = st.TuitionPolicy.EffectiveDate,
					ExpirationDate = st.TuitionPolicy.ExpirationDate,
					Semester = st.Term.TermName, // Use Term name
					IsPaid = st.PaymentStatus == "Paid", // Convert payment status to boolean
					PaymentDate = st.PaymentDate.HasValue ? DateOnly.FromDateTime(st.PaymentDate.Value) : null
				}).ToList();

				var totalAmount = studentTuitions.Sum(t => t.TotalAmount);
				var paidAmount = studentTuitions.Where(t => t.PaymentStatus == "Paid").Sum(t => t.AmountPaid);

				responses.Add(new StudentTuitionResponse
				{
					StudentId = student.Id,
					Mssv = student.Mssv,
					StudentName = student.User.FullName,
					Tuitions = tuitions,
					TotalAmount = totalAmount,
					PaidAmount = paidAmount,
					RemainingAmount = totalAmount - paidAmount
				});
			}

			return responses;
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
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(
					predicate: s => s.Id == studentId,
					include: q => q.Include(s => s.User)
							   .Include(s => s.Program)
				);

			if (student == null)
				return null;

			// Update user info
			if (!string.IsNullOrEmpty(request.FullName))
				student.User.FullName = request.FullName;

			if (!string.IsNullOrEmpty(request.ImageUrl))
				student.User.Image = request.ImageUrl;

			// Update student info
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
				ImageUrl = student.User.Image
			};
		}



		public async Task<StudentFinancialInfoResponse> AssignScholarshipToStudent(Guid studentId, ScholarshipAssignmentRequest request)
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

				// Get term from semester name or handle appropriately
				var term = await _unitOfWork.GetRepository<Term>()
					.SingleOrDefaultAsync(predicate: t => t.TermName == request.Semester);

				if (term == null)
					throw new BadHttpRequestException("Term not found");

				var newTuition = new StudentTuition
				{
					Id = Guid.NewGuid(),
					StudentId = studentId,
					TermId = term.Id,  // Set TermId from the found term
					TuitionPolicyId = request.TuitionPolicyId,
					TotalAmount = tuitionPolicy.Amount,  // Set from policy amount
					AmountPaid = request.IsPaid ? tuitionPolicy.Amount : 0,  // Set based on IsPaid
					PaymentStatus = request.IsPaid ? "Paid" : "Unpaid",  // Convert boolean to status string
					PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),  // Set a due date
					PaymentDate = request.IsPaid ? DateTime.Now : null  // Set payment date if paid
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

		public async Task<StudentTuitionResponse> UpdateStudentTuition(Guid studentId, Guid tuitionId, StudentTuitionCreateRequest request)
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
				var term = await _unitOfWork.GetRepository<Term>()
					.SingleOrDefaultAsync(predicate: t => t.TermName == request.Semester);

				if (term == null)
					throw new BadHttpRequestException("Term not found");

				studentTuition.TermId = term.Id;

				// Handle payment status
				studentTuition.PaymentStatus = request.IsPaid ? "Paid" : "Unpaid";
				if (request.IsPaid)
				{
					studentTuition.AmountPaid = studentTuition.TotalAmount;
					studentTuition.PaymentDate = DateTime.Now;
				}
				else
				{
					// If marked as unpaid, update accordingly
					studentTuition.AmountPaid = 0;
					studentTuition.PaymentDate = null;
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


	}
}