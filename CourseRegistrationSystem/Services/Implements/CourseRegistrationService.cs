using AutoMapper;
using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;
using CourseRegistration_API.Services.Interface;
using CourseRegistration_Domain.Entities;
using CourseRegistration_Repository.Interfaces;
using CourseRegistration_API.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseRegistration_API.Services.Implements
{
	public class CourseRegistrationService : BaseService<CourseRegistrationService>, ICourseRegistrationService
	{
		public CourseRegistrationService(IUnitOfWork<DbContext> unitOfWork, ILogger<CourseRegistrationService> logger,
						  IMapper mapper, IHttpContextAccessor httpContextAccessor)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public async Task<List<CourseOfferingResponse>> GetTermCourseOfferings(Guid termId, Guid? studentId = null)
		{
			// Get all course offerings for the term
			var courseOfferings = await _unitOfWork.GetRepository<CourseOffering>()
				.GetListAsync(
					predicate: co => co.TermId == termId,
					include: q => q
						.Include(co => co.Course)
						.Include(co => co.Lecturer)
							.ThenInclude(l => l.User)
						.Include(co => co.Registrations)
				);

			// Create the response list
			var response = courseOfferings.Select(co => new CourseOfferingResponse
			{
				CourseOfferingId = co.Id,
				CourseId = co.CourseId,
				CourseCode = co.Course.CourseCode,
				CourseName = co.Course.CourseName,
				Credits = co.Course.Credits,
				LecturerId = co.LecturerId,
				LecturerName = co.Lecturer.User.FullName,
				Classroom = co.Classroom,
				Schedule = co.Schedule,
				Capacity = co.Capacity,
				RegisteredCount = co.Registrations.Count,
				PrerequisiteStatus = "Not Required", // Default value, will be updated if studentId is provided
				ConflictingCourses = new List<string>()
			}).ToList();

			// If a student ID is provided, check prerequisites and scheduling conflicts
			if (studentId.HasValue)
			{
				// Get student's passed courses
				var passedCourses = await GetStudentPassedCourses(studentId.Value);

				// Get student's current registrations for this term
				var currentRegistrations = await _unitOfWork.GetRepository<Registration>()
					.GetListAsync(
						predicate: r => r.StudentId == studentId.Value &&
									  r.CourseOffering.TermId == termId,
						include: q => q.Include(r => r.CourseOffering)
									  .ThenInclude(co => co.Course)
					);

				foreach (var offering in response)
				{
					// Check prerequisites
					offering.PrerequisiteStatus = await CheckPrerequisiteStatus(studentId.Value, offering.CourseId, passedCourses);

					// Check for schedule conflicts
					offering.ConflictingCourses = GetScheduleConflicts(offering, currentRegistrations);
				}
			}

			return response;
		}

		private async Task<List<Guid>> GetStudentPassedCourses(Guid studentId)
		{
			var registrations = await _unitOfWork.GetRepository<Registration>()
				.GetListAsync(
					predicate: r => r.StudentId == studentId,
					include: q => q
						.Include(r => r.Grades)
						.Include(r => r.CourseOffering)
				);

			return registrations
				.Where(r => r.Grades.Any(g => g.QualityPoints >= 2.0m)) // C grade or better
				.Select(r => r.CourseOffering.CourseId)
				.ToList();
		}

		private async Task<string> CheckPrerequisiteStatus(Guid studentId, Guid courseId, List<Guid> passedCourses = null)
		{
			if (passedCourses == null)
			{
				passedCourses = await GetStudentPassedCourses(studentId);
			}

			// Get course with its prerequisites using navigation property
			var course = await _unitOfWork.GetRepository<Course>()
				.SingleOrDefaultAsync(
					predicate: c => c.Id == courseId,
					include: q => q.Include(c => c.PrerequisiteCourses)
				);

			if (course == null)
				throw new BadHttpRequestException("Course not found");

			// If no prerequisites, return Not Required
			if (course.PrerequisiteCourses == null || !course.PrerequisiteCourses.Any())
				return "Not Required";

			// Check if all prerequisites are satisfied
			foreach (var prerequisite in course.PrerequisiteCourses)
			{
				if (!passedCourses.Contains(prerequisite.Id))
				{
					return "Missing";
				}
			}

			return "Satisfied";
		}

		private List<string> GetScheduleConflicts(CourseOfferingResponse offering, ICollection<Registration> currentRegistrations)
		{
			var conflicts = new List<string>();

			// Simple schedule conflict detection based on string comparison
			// In a real system, you would parse the schedule strings and check for actual time overlaps
			foreach (var registration in currentRegistrations)
			{
				if (registration.CourseOffering.Schedule == offering.Schedule &&
					registration.CourseOffering.Id != offering.CourseOfferingId)
				{
					conflicts.Add($"{registration.CourseOffering.Course.CourseCode}: {registration.CourseOffering.Schedule}");
				}
			}

			return conflicts;
		}

		public async Task<bool> CheckPrerequisites(Guid studentId, Guid courseId)
		{
			var status = await CheckPrerequisiteStatus(studentId, courseId);
			return status == "Satisfied" || status == "Not Required";
		}

		public async Task<List<CourseRegistrationSummaryResponse>> GetRegistrationSummaryByTerm(Guid termId)
		{
			var courseOfferings = await _unitOfWork.GetRepository<CourseOffering>()
				.GetListAsync(
					predicate: co => co.TermId == termId,
					include: q => q
						.Include(co => co.Course)
						.Include(co => co.Registrations)
				);

			return courseOfferings
				.GroupBy(co => new { co.Course.CourseCode, co.Course.CourseName })
				.Select(group =>
				{
					int totalCapacity = group.Sum(co => co.Capacity);
					int totalRegistered = group.Sum(co => co.Registrations.Count);

					return new CourseRegistrationSummaryResponse
					{
						CourseCode = group.Key.CourseCode,
						CourseName = group.Key.CourseName,
						RegisteredStudents = totalRegistered,
						Capacity = totalCapacity,
						FillPercentage = totalCapacity > 0
							? Math.Round((double)totalRegistered / totalCapacity * 100, 2)
							: 0
					};
				})
				.OrderByDescending(s => s.FillPercentage)
				.ToList();
		}

		public async Task<bool> RegisterCourse(CourseRegistrationRequest request)
		{
			try
			{

				// Check if the course offering exists
				var courseOffering = await _unitOfWork.GetRepository<CourseOffering>()
					.SingleOrDefaultAsync(
						predicate: co => co.Id == request.CourseOfferingId,
						include: q => q
							.Include(co => co.Course)
							.Include(co => co.Registrations)
					);

				if (courseOffering == null)
					throw new BadHttpRequestException("Course offering not found");

				// Check if already registered
				var existingRegistration = await _unitOfWork.GetRepository<Registration>()
					.SingleOrDefaultAsync(
						predicate: r => r.StudentId == request.StudentId &&
									 r.CourseOfferingId == request.CourseOfferingId
					);

				if (existingRegistration != null)
					throw new BadHttpRequestException("Student is already registered for this course");

				// Check capacity
				bool isWaitlisted = courseOffering.Registrations.Count >= courseOffering.Capacity;

				// Check prerequisites
				var prerequisiteStatus = await CheckPrerequisiteStatus(request.StudentId, courseOffering.CourseId);
				if (prerequisiteStatus == "Missing")
					throw new BadHttpRequestException("Prerequisites not satisfied for this course");

				// Check schedule conflicts
				var studentTermRegistrations = await _unitOfWork.GetRepository<Registration>()
					.GetListAsync(
						predicate: r => r.StudentId == request.StudentId &&
									  r.CourseOffering.TermId == courseOffering.TermId,
						include: q => q.Include(r => r.CourseOffering)
					);

				foreach (var termRegistration in studentTermRegistrations)
				{
					var scheduleHelper = new ScheduleHelper();
					if (scheduleHelper.HasScheduleConflict(termRegistration.CourseOffering.Schedule, courseOffering.Schedule))
					{
						throw new BadHttpRequestException($"Schedule conflict with {termRegistration.CourseOffering.Course.CourseCode}: {termRegistration.CourseOffering.Schedule}");
					}
				}

				// Check total credits for the term
				int currentTermCredits = studentTermRegistrations.Sum(r => r.CourseOffering.Course.Credits);
				int newCourseCredits = courseOffering.Course.Credits;

				if (currentTermCredits + newCourseCredits > 24) // Assuming max is 24 credits
					throw new BadHttpRequestException("Registering for this course would exceed maximum credit limit");

				// Create registration
				var registration = new Registration
				{
					Id = Guid.NewGuid(),
					StudentId = request.StudentId,
					CourseOfferingId = request.CourseOfferingId,
					RegistrationDate = DateTime.Now,
					Status = isWaitlisted ? "Waitlisted" : "Registered"
				};

				await _unitOfWork.GetRepository<Registration>().InsertAsync(registration);
				await _unitOfWork.CommitAsync();

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error registering course: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<List<bool>> RegisterCourses(BatchCourseRegistrationRequest request)
		{
			var results = new List<bool>();

			foreach (var courseOfferingId in request.CourseOfferingIds)
			{
				try
				{
					var singleRequest = new CourseRegistrationRequest
					{
						StudentId = request.StudentId,
						CourseOfferingId = courseOfferingId
					};

					bool success = await RegisterCourse(singleRequest);
					results.Add(success);
				}
				catch
				{
					results.Add(false);
				}
			}

			return results;
		}

		public async Task<bool> UpdateRegistration(Guid registrationId, CourseRegistrationUpdateRequest request)
		{
			try
			{

				var registration = await _unitOfWork.GetRepository<Registration>()
					.SingleOrDefaultAsync(predicate: r => r.Id == registrationId);

				if (registration == null)
					throw new BadHttpRequestException("Registration not found");

				// Validate status
				var validStatuses = new[] { "Registered", "Waitlisted", "Dropped" };
				if (!validStatuses.Contains(request.Status))
					throw new BadHttpRequestException($"Invalid status. Valid statuses: {string.Join(", ", validStatuses)}");

				registration.Status = request.Status;
				await _unitOfWork.CommitAsync();

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating registration: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<bool> CancelRegistration(Guid registrationId)
		{
			try
			{

				var registration = await _unitOfWork.GetRepository<Registration>()
					.SingleOrDefaultAsync(predicate: r => r.Id == registrationId);

				if (registration == null)
					throw new BadHttpRequestException("Registration not found");

				_unitOfWork.GetRepository<Registration>().DeleteAsync(registration);
				await _unitOfWork.CommitAsync();


				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error cancelling registration: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<List<CourseOfferingResponse>> GetStudentRegistrations(Guid studentId, Guid termId)
		{
			var registrations = await _unitOfWork.GetRepository<Registration>()
				.GetListAsync(
					predicate: r => r.StudentId == studentId &&
								  r.CourseOffering.TermId == termId,
					include: q => q
						.Include(r => r.CourseOffering)
							.ThenInclude(co => co.Course)
						.Include(r => r.CourseOffering)
							.ThenInclude(co => co.Lecturer)
								.ThenInclude(l => l.User)
						.Include(r => r.CourseOffering)
							.ThenInclude(co => co.Registrations)
				);

			return registrations.Select(r => new CourseOfferingResponse
			{
				CourseOfferingId = r.CourseOffering.Id,
				CourseId = r.CourseOffering.CourseId,
				CourseCode = r.CourseOffering.Course.CourseCode,
				CourseName = r.CourseOffering.Course.CourseName,
				Credits = r.CourseOffering.Course.Credits,
				LecturerId = r.CourseOffering.LecturerId,
				LecturerName = r.CourseOffering.Lecturer.User.FullName,
				Classroom = r.CourseOffering.Classroom,
				Schedule = r.CourseOffering.Schedule,
				Capacity = r.CourseOffering.Capacity,
				RegisteredCount = r.CourseOffering.Registrations.Count
			}).ToList();
		}

		public async Task<List<StudentInfoResponse>> GetCourseStudents(Guid courseOfferingId)
		{
			var registrations = await _unitOfWork.GetRepository<Registration>()
				.GetListAsync(
					predicate: r => r.CourseOfferingId == courseOfferingId && r.Status != "Dropped",
					include: q => q
						.Include(r => r.Student)
							.ThenInclude(s => s.User)
						.Include(r => r.Student)
							.ThenInclude(s => s.Program)
				);

			return registrations
				.OrderBy(r => r.Student.User.FullName)
				.Select(r => new StudentInfoResponse
				{
					Id = r.Student.Id,
					Mssv = r.Student.Mssv,
					FullName = r.Student.User.FullName,
					Email = r.Student.User.Email,
					ProgramName = r.Student.Program.ProgramName,
					EnrollmentDate = r.Student.EnrollmentDate,
					ImageUrl = r.Student.User.Image
				}).ToList();
		}
	}
}