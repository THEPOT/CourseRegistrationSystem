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
			var classSections = await _unitOfWork.GetRepository<ClassSection>()
				.GetListAsync(
					predicate: co => co.SemesterId == termId,
					include: q => q
						.Include(co => co.Course)
						.Include(co => co.Professor)
							.ThenInclude(l => l.User)
						.Include(co => co.CourseRegistrations)
				);

			// Create the response list
			var response = classSections.Select(co => new CourseOfferingResponse
			{
				CourseOfferingId = co.Id,
				CourseId = co.CourseId,
				CourseCode = co.Course.CourseCode,
				CourseName = co.Course.CourseName,
				Credits = co.Course.Credits,
				LecturerId = co.ProfessorId,
				LecturerName = co.Professor.User.FullName,
				Classroom = co.Classroom?.RoomName ?? "Online",
				Schedule = GetScheduleString(co),
				Capacity = co.Capacity,
				RegisteredCount = co.CourseRegistrations?.Count ?? 0,
				PrerequisiteStatus = "Not Required", // Default value, will be updated if studentId is provided
				ConflictingCourses = new List<string>()
			}).ToList();

			// If a student ID is provided, check prerequisites and scheduling conflicts
			if (studentId.HasValue)
			{
				// Get student's passed courses
				var passedCourses = await GetStudentPassedCourses(studentId.Value);

				// Get student's current registrations for this term
				var currentRegistrations = await _unitOfWork.GetRepository<CourseRegistration>()
					.GetListAsync(
						predicate: r => r.StudentId == studentId.Value &&
									  r.ClassSection.SemesterId == termId,
						include: q => q.Include(r => r.ClassSection)
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

		private string GetScheduleString(ClassSection classSection)
		{
			if (classSection.ClassSectionSchedules == null || !classSection.ClassSectionSchedules.Any())
				return "Not scheduled";

			return string.Join(", ", classSection.ClassSectionSchedules.Select(s =>
				$"{s.DayOfWeek} {s.StartTime:hh\\:mm}-{s.EndTime:hh\\:mm}"));
		}

		private async Task<List<Guid>> GetStudentPassedCourses(Guid studentId)
		{
			var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: r => r.StudentId == studentId,
					include: q => q
						.Include(r => r.Grades)
						.Include(r => r.ClassSection)
				);

			return registrations
				.Where(r => r.Grades != null && r.Grades.Any(g => g.QualityPoints >= 2.0m)) // C grade or better
				.Select(r => r.ClassSection.CourseId)
				.ToList();
		}

		private async Task<string> CheckPrerequisiteStatus(Guid studentId, Guid courseId, List<Guid>? passedCourses = null)
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

		private List<string> GetScheduleConflicts(CourseOfferingResponse offering, ICollection<CourseRegistration> currentRegistrations)
		{
			var conflicts = new List<string>();

			// Simple schedule conflict detection based on string comparison
			// In a real system, you would parse the schedule strings and check for actual time overlaps
			foreach (var registration in currentRegistrations)
			{
				var regSchedule = GetScheduleString(registration.ClassSection);

				if (regSchedule == offering.Schedule &&
					registration.ClassSection.Id != offering.CourseOfferingId)
				{
					conflicts.Add($"{registration.ClassSection.Course.CourseCode}: {regSchedule}");
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
			var classSections = await _unitOfWork.GetRepository<ClassSection>()
				.GetListAsync(
					predicate: co => co.SemesterId == termId,
					include: q => q
						.Include(co => co.Course)
						.Include(co => co.CourseRegistrations)
				);

			return classSections
				.GroupBy(co => new { co.Course.CourseCode, co.Course.CourseName })
				.Select(group =>
				{
					int totalCapacity = group.Sum(co => co.Capacity);
					int totalRegistered = group.Sum(co => co.CourseRegistrations?.Count ?? 0);

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
				// Check if the class section exists
				var classSection = await _unitOfWork.GetRepository<ClassSection>()
					.SingleOrDefaultAsync(
						predicate: co => co.Id == request.CourseOfferingId,
						include: q => q
							.Include(co => co.Course)
							.Include(co => co.CourseRegistrations)
					);

				if (classSection == null)
					throw new BadHttpRequestException("Class section not found");

				// Check if already registered
				var existingRegistration = await _unitOfWork.GetRepository<CourseRegistration>()
					.SingleOrDefaultAsync(
						predicate: r => r.StudentId == request.StudentId &&
									 r.ClassSectionId == request.CourseOfferingId
					);

				if (existingRegistration != null)
					throw new BadHttpRequestException("Student is already registered for this course");

				// Check capacity
				bool isWaitlisted = classSection.CourseRegistrations.Count >= classSection.Capacity;

				// Check prerequisites
				var prerequisiteStatus = await CheckPrerequisiteStatus(request.StudentId, classSection.CourseId);
				if (prerequisiteStatus == "Missing")
					throw new BadHttpRequestException("Prerequisites not satisfied for this course");

				// Check schedule conflicts
				var studentTermRegistrations = await _unitOfWork.GetRepository<CourseRegistration>()
					.GetListAsync(
						predicate: r => r.StudentId == request.StudentId &&
									  r.ClassSection.SemesterId == classSection.SemesterId,
						include: q => q.Include(r => r.ClassSection)
									   .ThenInclude(cs => cs.Course)
					);

				foreach (var termRegistration in studentTermRegistrations)
				{
					var scheduleHelper = new ScheduleHelper();
					var existingSchedule = GetScheduleString(termRegistration.ClassSection);
					var newSchedule = GetScheduleString(classSection);

					if (scheduleHelper.HasScheduleConflict(existingSchedule, newSchedule))
					{
						throw new BadHttpRequestException($"Schedule conflict with {termRegistration.ClassSection.Course.CourseCode}: {existingSchedule}");
					}
				}

				// Check total credits for the term
				int currentTermCredits = studentTermRegistrations.Sum(r => r.ClassSection.Course.Credits);
				int newCourseCredits = classSection.Course.Credits;

				if (currentTermCredits + newCourseCredits > 24) // Assuming max is 24 credits
					throw new BadHttpRequestException("Registering for this course would exceed maximum credit limit");

				// Create registration
				var registration = new CourseRegistration
				{
					Id = Guid.NewGuid(),
					StudentId = request.StudentId,
					ClassSectionId = request.CourseOfferingId,
					RegistrationDate = DateTime.Now,
					Status = isWaitlisted ? "Waitlisted" : "Registered"
				};

				await _unitOfWork.GetRepository<CourseRegistration>().InsertAsync(registration);
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
				var registration = await _unitOfWork.GetRepository<CourseRegistration>()
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
				var registration = await _unitOfWork.GetRepository<CourseRegistration>()
					.SingleOrDefaultAsync(predicate: r => r.Id == registrationId);

				if (registration == null)
					throw new BadHttpRequestException("Registration not found");

				_unitOfWork.GetRepository<CourseRegistration>().DeleteAsync(registration);
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
			var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: r => r.StudentId == studentId &&
								  r.ClassSection.SemesterId == termId,
					include: q => q
						.Include(r => r.ClassSection)
							.ThenInclude(co => co.Course)
						.Include(r => r.ClassSection)
							.ThenInclude(co => co.Professor)
								.ThenInclude(l => l.User)
						.Include(r => r.ClassSection)
							.ThenInclude(co => co.CourseRegistrations)
				);

			return registrations.Select(r => new CourseOfferingResponse
			{
				CourseOfferingId = r.ClassSection.Id,
				CourseId = r.ClassSection.CourseId,
				CourseCode = r.ClassSection.Course.CourseCode,
				CourseName = r.ClassSection.Course.CourseName,
				Credits = r.ClassSection.Course.Credits,
				LecturerId = r.ClassSection.ProfessorId,
				LecturerName = r.ClassSection.Professor.User.FullName,
				Classroom = r.ClassSection.Classroom?.RoomName ?? "Online",
				Schedule = GetScheduleString(r.ClassSection),
				Capacity = r.ClassSection.Capacity,
				RegisteredCount = r.ClassSection.CourseRegistrations?.Count ?? 0
			}).ToList();
		}

		public async Task<List<StudentInfoResponse>> GetCourseStudents(Guid courseOfferingId)
		{
			var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: r => r.ClassSectionId == courseOfferingId && r.Status != "Dropped",
					include: q => q
						.Include(r => r.Student)
							.ThenInclude(s => s.User)
						.Include(r => r.Student)
							.ThenInclude(s => s.Major)
				);

			return registrations
				.OrderBy(r => r.Student.User.FullName)
				.Select(r => new StudentInfoResponse
				{
					Id = r.Student.Id,
					Mssv = r.Student.Mssv,
					FullName = r.Student.User.FullName,
					Email = r.Student.User.Email,
					MajorName = r.Student.Major.MajorName,
					EnrollmentDate = r.Student.EnrollmentDate,
					ImageUrl = r.Student.User.Image
				}).ToList();
		}
	}
}