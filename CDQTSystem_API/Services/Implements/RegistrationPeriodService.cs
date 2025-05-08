using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_API.Services.Implements
{
	public class RegistrationPeriodService : BaseService<RegistrationPeriodService>, IRegistrationPeriodService
	{
		public RegistrationPeriodService(IUnitOfWork<DbContext> unitOfWork, ILogger<RegistrationPeriodService> logger,
						  IMapper mapper, IHttpContextAccessor httpContextAccessor)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public async Task<RegistrationPeriodResponse> CreateRegistrationPeriod(RegistrationPeriodCreateRequest request, Guid userId)
		{
			try
			{
				// Validate semester exists
				var semester = await _unitOfWork.GetRepository<Semester>()
					.SingleOrDefaultAsync(predicate: s => s.Id == request.SemesterId);

				if (semester == null)
					throw new BadHttpRequestException("Semester not found");

				var startDate = request.StartDate;
				var endDate = request.EndDate;

				// Validate dates
				if (startDate >= endDate)
					throw new BadHttpRequestException("Start date must be before end date");


				// Create new registration period
				var registrationPeriod = new RegistrationPeriod
				{
					Id = Guid.NewGuid(),
					SemesterId = request.SemesterId,
					MaxCredits = request.MaxCredits,
					StartDate = startDate,
					EndDate = endDate,
					Status = "CLOSED",
					CreatedBy = userId,
					CreatedDate = DateTime.Now
				};

				await _unitOfWork.GetRepository<RegistrationPeriod>().InsertAsync(registrationPeriod);
				await _unitOfWork.CommitAsync();

				// Return response
				return new RegistrationPeriodResponse
				{
					Id = registrationPeriod.Id,
					SemesterName = semester.SemesterName,
					StartDate = registrationPeriod.StartDate,
					EndDate = registrationPeriod.EndDate,
					Status = registrationPeriod.Status,
					MaxCredits = registrationPeriod.MaxCredits
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating registration period: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<RegistrationPeriodResponse> GetRegistrationPeriodById(Guid id)
		{
			var registrationPeriod = await _unitOfWork.GetRepository<RegistrationPeriod>()
				.SingleOrDefaultAsync(
					predicate: rp => rp.Id == id,
					include: q => q.Include(rp => rp.Semester)
								.ThenInclude(s => s.ClassSections)
								.ThenInclude(cs => cs.Course)
								.Include(rp => rp.CourseRegistrations)
								.ThenInclude(cr => cr.ClassSection)
								.ThenInclude(cs => cs.ClassSectionSchedules)
				);

			

			if (registrationPeriod == null)
				return null;

			// Calculate total registered and waitlisted students
			var totalRegisteredStudents = registrationPeriod.CourseRegistrations
				.Count(cr => cr.Status == "Registered");

			var totalWaitlistedStudents = registrationPeriod.CourseRegistrations
				.Count(cr => cr.Status == "Waitlisted");

			// Create registration statistics
			var totalCourseOfferings = registrationPeriod.CourseRegistrations
				.Select(cr => cr.ClassSection.Id)
				.Distinct()
				.Count();

			var fullCourses = registrationPeriod.CourseRegistrations
				.GroupBy(cr => cr.ClassSectionId)
				.Count(g => g.Count(cr => cr.Status == "Registered") >=
					(g.First().ClassSection?.MaxCapacity ?? int.MaxValue));

			return new RegistrationPeriodResponse
			{
				Id = registrationPeriod.Id,
				SemesterName = registrationPeriod.Semester.SemesterName,
				StartDate = registrationPeriod.StartDate,
				EndDate = registrationPeriod.EndDate,
				Status = registrationPeriod.Status,
				MaxCredits = registrationPeriod.MaxCredits,
				TotalRegisteredStudents = totalRegisteredStudents,
				TotalWaitlistedStudents = totalWaitlistedStudents,
				ClassSections = registrationPeriod.CourseRegistrations
					.Select(cr => new ClassSectionInfo
					{
						CourseName = registrationPeriod.Semester.ClassSections.FirstOrDefault(cs => cs.Id == cr.ClassSectionId)?.Course?.CourseName,
						CourseCode = registrationPeriod.Semester.ClassSections.FirstOrDefault(cs => cs.Id == cr.ClassSectionId)?.Course?.CourseCode,
						Credits = (int)registrationPeriod.Semester.ClassSections.FirstOrDefault(cs => cs.Id == cr.ClassSectionId)?.Course?.Credits,
						MaxCapacity = (int)registrationPeriod.Semester.ClassSections.FirstOrDefault(cs => cs.Id == cr.ClassSectionId)?.MaxCapacity,
						Schedules = registrationPeriod.Semester.ClassSections.FirstOrDefault(cs => cs.Id == cr.ClassSectionId)?.ClassSectionSchedules
							.Select(s => new ScheduleInfo
							{
								DayOfWeek = s.DayOfWeek,
								StartTime = s.StartTime,
								EndTime = s.EndTime
							}).ToList()
					}).ToList(),
				CurrentStatistics = new RegistrationStatistics
				{
					RegisteredStudents = totalRegisteredStudents,
					WaitlistedStudents = totalWaitlistedStudents,
					TotalCourseOfferings = totalCourseOfferings,
					FullCourses = fullCourses,
					RegistrationRate = totalRegisteredStudents > 0 ?
						(double)totalRegisteredStudents / (totalRegisteredStudents + totalWaitlistedStudents) * 100 : 0
				}
			};
		}


		public Task<RegistrationPeriodResponse> GetCurrentRegistrationPeriod()
		{
			throw new NotImplementedException();
		}

		public Task<List<ProgramRegistrationStatisticsResponse>> GetProgramStatistics(Guid periodId)
		{
			throw new NotImplementedException();
		}

		public Task<RegistrationStatisticsResponse> GetRegistrationStatistics(Guid periodId, Guid? programId = null, Guid? courseId = null)
		{
			throw new NotImplementedException();
		}

		public Task<List<RegistrationSummaryResponse>> GetRegistrationSummary(Guid termId)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> UpdateRegistrationPeriodStatus(Guid periodId, string status)
		{
			try
			{
				var registrationPeriod = await _unitOfWork.GetRepository<RegistrationPeriod>()
					.SingleOrDefaultAsync(predicate: rp => rp.Id == periodId);

				if (registrationPeriod == null)
					throw new BadHttpRequestException("Registration period not found");

				// Validate status
				var validStatuses = new[] { "MAINTENANCE", "OPEN", "CLOSED" };
				if (!validStatuses.Contains(status))
					throw new BadHttpRequestException($"Invalid status. Valid statuses: {string.Join(", ", validStatuses)}");

				// Additional validation rules
				if (status == "OPEN")
				{
					// Check if there's already an open registration period
					var existingOpenPeriod = await _unitOfWork.GetRepository<RegistrationPeriod>()
						.SingleOrDefaultAsync(predicate: rp =>
							rp.Status == "OPEN" &&
							rp.Id != periodId &&
							rp.SemesterId == registrationPeriod.SemesterId);

					if (existingOpenPeriod != null)
						throw new BadHttpRequestException("Another registration period is already open for this semester");

					// Validate dates
					if (DateTime.UtcNow.AddHours(7) > registrationPeriod.EndDate)
						throw new BadHttpRequestException("Cannot open a registration period that has already ended");
				}

				registrationPeriod.Status = status;
				_unitOfWork.GetRepository<RegistrationPeriod>().UpdateAsync(registrationPeriod);
				await _unitOfWork.CommitAsync();
				_logger.LogInformation("Status updated successfully to: {Status}", registrationPeriod.Status);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating registration period status: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<List<RegistrationPeriodResponse>> GetAllRegistrationPeriods()
		{
			try
			{
				var registrationPeriods = await _unitOfWork.GetRepository<RegistrationPeriod>()
				.GetListAsync(
					include: q => q.Include(rp => rp.Semester)
									.ThenInclude(s => s.ClassSections)
									.ThenInclude(cs => cs.Course)
									.Include(rp => rp.CourseRegistrations)
									.ThenInclude(cr => cr.ClassSection)
									.ThenInclude(cs => cs.ClassSectionSchedules)
				);

				if (registrationPeriods == null || !registrationPeriods.Any())
					return new List<RegistrationPeriodResponse>();

				var responseList = registrationPeriods.Select(period =>
				{
					var totalRegisteredStudents = period.CourseRegistrations
						.Count(cr => cr.Status == "Registered");

					var totalWaitlistedStudents = period.CourseRegistrations
						.Count(cr => cr.Status == "Waitlisted");

					var totalCourseOfferings = period.CourseRegistrations
						.Select(cr => cr.ClassSectionId)
						.Distinct()
						.Count();

					var fullCourses = period.CourseRegistrations
						.GroupBy(cr => cr.ClassSectionId)
						.Count(g => g.Count(cr => cr.Status == "Registered") >= 
							(g.FirstOrDefault()?.ClassSection?.MaxCapacity ?? int.MaxValue));

					double registrationRate = 0;
					int totalStudentsConsidered = totalRegisteredStudents + totalWaitlistedStudents;
					if (totalStudentsConsidered > 0)
					{
						registrationRate = (double)totalRegisteredStudents / totalStudentsConsidered * 100;
					}

					return new RegistrationPeriodResponse
					{
						Id = period.Id,
						SemesterId = period.SemesterId,
						SemesterName = period.Semester?.SemesterName ?? "N/A",
						StartDate = period.StartDate,
						EndDate = period.EndDate,
						Status = period.Status,
						MaxCredits = period.MaxCredits,
						TotalRegisteredStudents = totalRegisteredStudents,
						TotalWaitlistedStudents = totalWaitlistedStudents,
						ClassSections = period.CourseRegistrations
							.Select(cr => new ClassSectionInfo
							{
								CourseName = cr.ClassSection.Course.CourseName,
								CourseCode = cr.ClassSection.Course.CourseCode,
								Credits = cr.ClassSection.Course.Credits,
								MaxCapacity = cr.ClassSection.MaxCapacity,
								Schedules = cr.ClassSection.ClassSectionSchedules
									.Select(s => new ScheduleInfo
									{
										DayOfWeek = s.DayOfWeek,
										StartTime = s.StartTime,
										EndTime = s.EndTime
									}).ToList()
							}).ToList(),
						CurrentStatistics = new RegistrationStatistics
						{
							RegisteredStudents = totalRegisteredStudents,
							WaitlistedStudents = totalWaitlistedStudents,
							TotalCourseOfferings = totalCourseOfferings,
							FullCourses = fullCourses,
							RegistrationRate = registrationRate
						}
					};
				}).ToList();

				return responseList;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all registration periods");
				throw;
			}
		}

		public async Task<RegistrationAnalyticsResponse> GetRegistrationAnalytics(Guid termId)
		{
			try
			{
				var termSummaries = await GetTermRegistrationSummaries();
				var courseSummaries = await GetCourseRegistrationSummaries(termId);
				var programSummaries = await GetProgramEnrollmentSummaries(termId);
				var dailyCounts = await GetDailyRegistrationCounts(termId);

				return new RegistrationAnalyticsResponse
				{
					TermSummaries = termSummaries,
					CourseSummaries = courseSummaries,
					ProgramSummaries = programSummaries,
					DailyRegistrationCounts = dailyCounts
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting registration analytics");
				throw;
			}
		}

		public async Task<List<TermRegistrationSummary>> GetTermRegistrationSummaries()
		{
			var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					include: q => q
						.Include(r => r.ClassSection)
							.ThenInclude(cs => cs.Semester)
						.Include(r => r.Student)
							.ThenInclude(s => s.Major)
				);

			return registrations
				.GroupBy(r => r.ClassSection.SemesterId)
				.Select(g => new TermRegistrationSummary
				{
					TermId = g.Key,
					TermName = g.First().ClassSection.Semester.SemesterName,
					TotalStudents = g.Select(r => r.StudentId).Distinct().Count(),
					TotalCourses = g.Select(r => r.ClassSection.CourseId).Distinct().Count(),
					AverageCoursesPerStudent = g.GroupBy(r => r.StudentId).Average(sg => sg.Count()),
					RegistrationsByProgram = g
						.GroupBy(r => r.Student.Major.MajorName)
						.ToDictionary(pg => pg.Key, pg => pg.Count())
				})
				.ToList();
		}

		public async Task<List<CourseRegistrationSummary>> GetCourseRegistrationSummaries(Guid termId)
		{
			var classSections = await _unitOfWork.GetRepository<ClassSection>()
				.GetListAsync(
					predicate: cs => cs.SemesterId == termId,
					include: q => q.Include(cs => cs.Course)
				);

			var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: r => r.ClassSection.SemesterId == termId,
					include: q => q
						.Include(r => r.Student)
							.ThenInclude(s => s.Major)
				);

			return classSections
				.Where(cs => cs.Course != null)
				.GroupBy(cs => cs.CourseId)
				.Select(g =>
				{
					var courseRegistrations = registrations
						.Where(r => r.ClassSection != null && r.ClassSection.CourseId == g.Key)
						.ToList();

					var firstCourse = g.FirstOrDefault(cs => cs.Course != null)?.Course;
					return new CourseRegistrationSummary
					{
						CourseCode = firstCourse?.CourseCode ?? "Unknown",
						CourseName = firstCourse?.CourseName ?? "Unknown",
						TotalRegistrations = courseRegistrations.Count,
						TotalSections = g.Count(),
						AverageStudentsPerSection = g.Any()
							? g.Average(cs => courseRegistrations.Count(r => r.ClassSectionId == cs.Id))
							: 0,
						RegistrationsByProgram = courseRegistrations
							.Where(r => r.Student?.Major != null)
							.GroupBy(r => r.Student.Major.MajorName ?? "Unknown")
							.ToDictionary(pg => pg.Key, pg => pg.Count())
					};
				})
				.ToList();
		}

		public async Task<List<ProgramEnrollmentSummary>> GetProgramEnrollmentSummaries(Guid termId)
		{
			var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: r => r.ClassSection.SemesterId == termId,
					include: q => q
						.Include(r => r.Student)
							.ThenInclude(s => s.Major)
				);

			return registrations
				.Where(r => r.Student?.Major != null)
				.GroupBy(r => r.Student.MajorId)
				.Select(g =>
				{
					var program = g.FirstOrDefault(r => r.Student?.Major != null)?.Student.Major;
					var studentRegistrations = g.GroupBy(r => r.StudentId);

					return new ProgramEnrollmentSummary
					{
						ProgramCode = program?.MajorName ?? "Unknown",
						ProgramName = program?.MajorName ?? "Unknown",
						TotalStudents = studentRegistrations.Count(),
						AverageCoursesPerStudent = studentRegistrations.Any()
							? studentRegistrations.Average(sr => sr.Count())
							: 0,
						YearLevelBreakdown = g
							.Where(r => r.Student != null)
							.GroupBy(r => GetYearLevel(r.Student.EnrollmentDate))
							.Select(yg => new YearLevelEnrollment
							{
								YearLevel = yg.Key,
								StudentCount = yg.Select(r => r.StudentId).Distinct().Count(),
								AverageCoursesPerStudent = yg.GroupBy(r => r.StudentId).Any()
									? yg.GroupBy(r => r.StudentId).Average(sg => sg.Count())
									: 0
							})
							.ToList()
					};
				})
				.ToList();
		}

		private int GetYearLevel(DateOnly enrollmentDate)
		{
			var years = (DateTime.UtcNow.Year - enrollmentDate.Year);
			return Math.Min(Math.Max(1, years), 4);
		}

		private async Task<Dictionary<string, int>> GetDailyRegistrationCounts(Guid termId)
		{
			var dailyCounts = await _unitOfWork.GetRepository<CourseRegistration>()
				.GetListAsync(
					predicate: r => r.ClassSection.SemesterId == termId,
					include: q => q.Include(r => r.ClassSection)
				);

			return dailyCounts
				.GroupBy(r => r.RegistrationDate.Date)
				.OrderBy(g => g.Key)
				.ToDictionary(
					g => g.Key.ToString("yyyy-MM-dd"),
					g => g.Count()
				);
		}
	}
}