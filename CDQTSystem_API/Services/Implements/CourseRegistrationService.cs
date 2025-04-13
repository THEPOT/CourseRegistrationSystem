using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using CDQTSystem_API.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using CDQTSystem_API.Messages;
using static CDQTSystem_API.Constants.ApiEndPointConstant;

namespace CDQTSystem_API.Services.Implements
{
	public class CourseRegistrationService : BaseService<CourseRegistrationService>, ICourseRegistrationService
	{
		private readonly IRequestClient<CourseRegistrationMessage> _requestClient;

		public CourseRegistrationService(
			IUnitOfWork<DbContext> unitOfWork,
			ILogger<CourseRegistrationService> logger,
			IMapper mapper,
			IHttpContextAccessor httpContextAccessor,
			IRequestClient<CourseRegistrationMessage> requestClient)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
			_requestClient = requestClient;
		}

		public async Task<List<CourseRegistrationResponse>> GetRegistrations(Guid studentId)
		{
			try
			{
				var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
					.GetListAsync(
						predicate: r => r.StudentId == studentId,
						include: q => q
							.Include(r => r.ClassSection)
								.ThenInclude(cs => cs.Course)
							.Include(r => r.ClassSection)
								.ThenInclude(cs => cs.Professor)
									.ThenInclude(p => p.User)
							.Include(r => r.ClassSection)
								.ThenInclude(cs => cs.Classroom)
							.Include(r => r.ClassSection)
								.ThenInclude(cs => cs.CourseRegistrations)
					);

				return registrations.Select(r => new CourseRegistrationResponse
				{
					RegistrationId = r.Id,
					CourseOfferingId = r.ClassSection.Id,
					CourseId = r.ClassSection.CourseId,
					CourseCode = r.ClassSection.Course.CourseCode,
					CourseName = r.ClassSection.Course.CourseName,
					Credits = r.ClassSection.Course.Credits,
					ProfessorId = r.ClassSection.ProfessorId ?? Guid.Empty,
					ProfessorName = r.ClassSection.Professor?.User.FullName ?? "TBA",
					Classroom = r.ClassSection.Classroom?.RoomName ?? "TBA",
					Schedule = GetScheduleString(r.ClassSection),
					Status = r.Status,
					RegistrationDate = r.RegistrationDate,
					TuitionStatus = r.TuitionStatus
				}).ToList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting registrations for student {StudentId}", studentId);
				throw;
			}
		}

		private string GetScheduleString(ClassSection classSection)
		{
			if (classSection.ClassSectionSchedules == null || !classSection.ClassSectionSchedules.Any())
				return "TBA";

			return string.Join(", ", classSection.ClassSectionSchedules
				.OrderBy(s => s.DayOfWeek)
				.Select(s => $"{s.DayOfWeek} {s.StartTime:hh\\:mm}-{s.EndTime:hh\\:mm}"));
		}

		private async Task<string> CheckPrerequisiteStatus(Guid studentId, Guid courseId)
		{
			var course = await _unitOfWork.GetRepository<Course>()
				.SingleOrDefaultAsync(
					predicate: c => c.Id == courseId,
					include: q => q.Include(c => c.PrerequisiteCourses)
				);

			if (course == null || !course.PrerequisiteCourses.Any())
				return "Satisfied";

			var studentCompletedCourses = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: r => r.StudentId == studentId && 
								   r.Status == "Completed",
					include: q => q
						.Include(r => r.ClassSection)
							.ThenInclude(cs => cs.Course)
						.Include(r => r.Grades)
				);

			var completedCourseIds = studentCompletedCourses
				.Select(r => r.ClassSection.CourseId)
				.Distinct()
				.ToList();

			return course.PrerequisiteCourses.All(p => completedCourseIds.Contains(p.Id)) 
				? "Satisfied" 
				: "Missing";
		}

		public async Task<bool> RegisterCourse(CourseRegistrationRequest request, Guid userId)
		{
			try
			{
				var student = await _unitOfWork.GetRepository<Student>()
					.SingleOrDefaultAsync(predicate: s => s.UserId == userId);
				
				if (student == null)
				{
					_logger.LogError("Student not found for user {UserId}", userId);
					throw new BadHttpRequestException("Student not found");
				}

				var studentId = student.Id;
				await ValidateRegistrationRequest(request, studentId);

				var message = new CourseRegistrationMessage
				{
					RequestId = Guid.NewGuid(),
					StudentId = studentId,
					CourseOfferingId = request.CourseOfferingId,
					RequestTimestamp = DateTime.UtcNow
				};

				_logger.LogInformation(
					"Sending registration request - RequestId: {RequestId}, StudentId: {StudentId}, CourseOfferingId: {CourseOfferingId}",
					message.RequestId,
					message.StudentId,
					message.CourseOfferingId
				);

				// Increase timeout to 60 seconds and add retry logic
				var timeout = TimeSpan.FromSeconds(60);
				var retryCount = 3;
				
				for (int i = 0; i < retryCount; i++)
				{
					try
					{
						var response = await _requestClient.GetResponse<CourseRegistrationResult>(
							message,
							timeout: timeout
						);

						_logger.LogInformation(
							"Registration response received - RequestId: {RequestId}, Success: {Success}",
							message.RequestId,
							response.Message.Success
						);

						return response.Message.Success;
					}
					catch (RequestTimeoutException) when (i < retryCount - 1)
					{
						_logger.LogWarning(
							"Registration request timeout (attempt {Attempt}/{MaxAttempts}) - RequestId: {RequestId}",
							i + 1,
							retryCount,
							message.RequestId
						);
						await Task.Delay(1000 * (i + 1)); // Exponential backoff
						continue;
					}
				}

				throw new BadHttpRequestException("Registration request timed out after multiple attempts");
			}
			catch (BadHttpRequestException)
			{
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error registering course for user {UserId}", userId);
				throw new BadHttpRequestException("Failed to process registration request");
			}
		}

		private async Task ValidateRegistrationRequest(CourseRegistrationRequest request, Guid studentId)
		{
			var currentPeriod = await _unitOfWork.GetRepository<RegistrationPeriod>()
				.SingleOrDefaultAsync(
					predicate: rp => rp.Status == "Open" &&
									rp.StartDate <= DateTime.UtcNow &&
									rp.EndDate >= DateTime.UtcNow
				);

			if (currentPeriod == null)
				throw new BadHttpRequestException("Course registration is currently closed");

			var classSection = await _unitOfWork.GetRepository<ClassSection>()
				.SingleOrDefaultAsync(
					predicate: cs => cs.Id == request.CourseOfferingId,
					include: q => q
						.Include(cs => cs.Course)
						.Include(cs => cs.ClassSectionSchedules)
				);

			if (classSection == null)
				throw new BadHttpRequestException("Course section not found");

			var prerequisiteStatus = await CheckPrerequisiteStatus(studentId, classSection.CourseId);
			if (prerequisiteStatus == "Missing")
				throw new BadHttpRequestException("Prerequisites not satisfied for this course");

			var existingRegistrations = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: r => r.StudentId == studentId &&
								   r.ClassSection.SemesterId == classSection.SemesterId &&
								   r.Status != "Dropped",
					include: q => q
						.Include(r => r.ClassSection)
							.ThenInclude(cs => cs.ClassSectionSchedules)
				);

			foreach (var registration in existingRegistrations)
			{
				if (HasScheduleConflict(classSection.ClassSectionSchedules, 
									  registration.ClassSection.ClassSectionSchedules))
				{
					throw new BadHttpRequestException("Schedule conflict with existing registration");
				}
			}
		}

		private bool HasScheduleConflict(ICollection<ClassSectionSchedule> schedule1, 
									   ICollection<ClassSectionSchedule> schedule2)
		{
			foreach (var s1 in schedule1)
			{
				foreach (var s2 in schedule2)
				{
					if (s1.DayOfWeek == s2.DayOfWeek &&
						s1.StartTime < s2.EndTime &&
						s2.StartTime < s1.EndTime)
					{
						return true;
					}
				}
			}
			return false;
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
				ProfessorId = r.ClassSection.ProfessorId ?? Guid.Empty,
				ProfessorName = r.ClassSection.Professor.User.FullName,
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

		public async Task<List<AvailableCourseResponse>> GetAvailableCourseOfferings()
		{
			// Get current registration period
			var currentPeriod = await _unitOfWork.GetRepository<RegistrationPeriod>()
				.SingleOrDefaultAsync(
					predicate: rp => rp.Status == "Open" &&
									rp.StartDate <= DateTime.UtcNow &&
									rp.EndDate >= DateTime.UtcNow
				);

			if (currentPeriod == null)
				throw new BadHttpRequestException("No active registration period found");

			// Get course offerings with all related data in a single query
			var offerings = await _unitOfWork.GetRepository<ClassSection>()
				.GetListAsync(
					predicate: cs => cs.SemesterId == currentPeriod.SemesterId,
					include: q => q
						.Include(cs => cs.Course)
							.ThenInclude(c => c.PrerequisiteCourses)
						.Include(cs => cs.Professor)
							.ThenInclude(p => p.User)
						.Include(cs => cs.Classroom)
						.Include(cs => cs.CourseRegistrations)
						.Include(cs => cs.Semester)
						.Include(cs => cs.ClassSectionSchedules)
				);

			return offerings
				.Where(o => (o.CourseRegistrations?.Count(r => r.Status != "Dropped") ?? 0) < o.Capacity) // Chỉ lấy các khóa học còn slot
				.Select(o => new AvailableCourseResponse
				{
					CourseOfferingId = o.Id,
					CourseCode = o.Course.CourseCode,
					CourseName = o.Course.CourseName,
					Credits = o.Course.Credits,
					ProfessorId = o.ProfessorId,
					ProfessorName = o.Professor?.User.FullName,
					Schedule = GetScheduleString(o),
					Capacity = o.Capacity,
					RegisteredCount = o.CourseRegistrations?.Count(r => r.Status != "Dropped") ?? 0,
					AvailableSlots = o.Capacity - (o.CourseRegistrations?.Count(r => r.Status != "Dropped") ?? 0),
					PrerequisitesSatisfied = false,
					Prerequisites = o.Course.PrerequisiteCourses?.Select(p => p.CourseCode).ToList() ?? new List<string>()
				}).ToList();
		}

		public async Task<List<AvailableCourseResponse>> GetAvailableCourseOfferingsForStudent(Guid studentId)
		{
			var offerings = await GetAvailableCourseOfferings();
			
			if (!offerings.Any()) return new List<AvailableCourseResponse>();
			
			var completedCourses = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: r => r.StudentId == studentId && 
								   r.Status == "Completed",
					include: q => q
						.Include(r => r.ClassSection)
							.ThenInclude(cs => cs.Course)
				);

			var completedCourseIds = completedCourses
				.Select(r => r.ClassSection.CourseId)
				.Distinct()
				.ToHashSet();

			var coursesWithPrerequisites = await _unitOfWork.GetRepository<Course>()
				.GetListAsync(
					predicate: c => offerings.Select(o => o.CourseCode).Contains(c.CourseCode),
					include: q => q.Include(c => c.PrerequisiteCourses)
				);

			var coursePrerequisitesMap = coursesWithPrerequisites.ToDictionary(
				c => c.CourseCode,
				c => (
					Prerequisites: c.PrerequisiteCourses?.Select(p => p.CourseCode).ToList() ?? new List<string>(),
					Satisfied: !c.PrerequisiteCourses.Any() || c.PrerequisiteCourses.All(p => completedCourseIds.Contains(p.Id))
				)
			);

			foreach (var offering in offerings)
			{
				if (coursePrerequisitesMap.TryGetValue(offering.CourseCode, out var prereqInfo))
				{
					offering.Prerequisites = prereqInfo.Prerequisites;
					offering.PrerequisitesSatisfied = prereqInfo.Satisfied;
				}
			}

			return offerings;
		}

		public async Task<bool> CheckPrerequisites(Guid studentId, Guid courseId)
		{
			var status = await CheckPrerequisiteStatus(studentId, courseId);
			return status == "Satisfied";
		}
	}
}
