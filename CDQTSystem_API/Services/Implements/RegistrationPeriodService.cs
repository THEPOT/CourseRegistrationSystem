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
                    SemesterId = registrationPeriod.SemesterId,
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
                );

            if (registrationPeriod == null)
                return null;

            return new RegistrationPeriodResponse
            {
                Id = registrationPeriod.Id,
                SemesterId = registrationPeriod.SemesterId,
                SemesterName = registrationPeriod.Semester.SemesterName,
                StartDate = registrationPeriod.StartDate,
                EndDate = registrationPeriod.EndDate,
                Status = registrationPeriod.Status
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
                    if (DateTime.UtcNow > registrationPeriod.EndDate)
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
                        include: q => q
                            .Include(rp => rp.Semester)
                            .Include(rp => rp.CourseRegistrations),
                        orderBy: q => q
                            .OrderByDescending(rp => rp.StartDate)
                    );

                return registrationPeriods.Select(rp => new RegistrationPeriodResponse
                {
                    Id = rp.Id,
                    SemesterId = rp.SemesterId,
                    SemesterName = rp.Semester.SemesterName,
                    StartDate = rp.StartDate,
                    EndDate = rp.EndDate,
                    Status = rp.Status,
                    MaxCredits = rp.MaxCredits,
                }).ToList();
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
                // Thực hiện các truy vấn song song để cải thiện hiệu suất
                var termSummariesTask = GetTermRegistrationSummaries();
                var courseSummariesTask = GetCourseRegistrationSummaries(termId);
                var programSummariesTask = GetProgramEnrollmentSummaries(termId);
                var dailyCountsTask = GetDailyRegistrationCounts(termId);

                await Task.WhenAll(termSummariesTask, courseSummariesTask, programSummariesTask, dailyCountsTask);

                var termSummaries = await termSummariesTask;
                var courseSummaries = await courseSummariesTask;
                var programSummaries = await programSummariesTask;
                var dailyCounts = await dailyCountsTask;

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
            // Tách thành nhiều truy vấn nhỏ hơn
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
                .GroupBy(cs => cs.CourseId)
                .Select(g =>
                {
                    var courseRegistrations = registrations
                        .Where(r => r.ClassSection.CourseId == g.Key)
                        .ToList();

                    return new CourseRegistrationSummary
                    {
                        CourseCode = g.First().Course.CourseCode,
                        CourseName = g.First().Course.CourseName,
                        TotalRegistrations = courseRegistrations.Count,
                        TotalSections = g.Count(),
                        AverageStudentsPerSection = g.Average(cs => 
                            courseRegistrations.Count(r => r.ClassSectionId == cs.Id)),
                        RegistrationsByProgram = courseRegistrations
                            .GroupBy(r => r.Student.Major.MajorName)
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
                .GroupBy(r => r.Student.MajorId)
                .Select(g =>
                {
                    var program = g.First().Student.Major;
                    var studentRegistrations = g.GroupBy(r => r.StudentId);

                    return new ProgramEnrollmentSummary
                    {
                        ProgramCode = program.MajorName,
                        ProgramName = program.MajorName,
                        TotalStudents = studentRegistrations.Count(),
                        AverageCoursesPerStudent = studentRegistrations.Average(sr => sr.Count()),
                        YearLevelBreakdown = g
                            .GroupBy(r => GetYearLevel(r.Student.EnrollmentDate))
                            .Select(yg => new YearLevelEnrollment
                            {
                                YearLevel = yg.Key,
                                StudentCount = yg.Select(r => r.StudentId).Distinct().Count(),
                                AverageCoursesPerStudent = yg.GroupBy(r => r.StudentId).Average(sg => sg.Count())
                            })
                            .ToList()
                    };
                })
                .ToList();
        }

        private int GetYearLevel(DateOnly enrollmentDate)
        {
            var years = (DateTime.UtcNow.Year - enrollmentDate.Year);
            return Math.Min(Math.Max(1, years), 4); // Ensure between 1 and 4
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









