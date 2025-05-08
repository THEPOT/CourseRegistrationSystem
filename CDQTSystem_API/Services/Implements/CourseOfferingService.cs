using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_API.Services.Implements
{
    public class CourseOfferingService : BaseService<CourseOfferingService>, ICourseOfferingService
    {
        public CourseOfferingService(IUnitOfWork<DbContext> unitOfWork, ILogger<CourseOfferingService> logger,
                          IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<List<CourseOfferingResponse>> CreateCourseOfferings(CourseOfferingCreateRequest request)
        {
            try
            {
                var semester = await _unitOfWork.GetRepository<Semester>()
                    .SingleOrDefaultAsync(predicate: s => s.Id == request.SemesterId);

                if (semester == null)
                    throw new BadHttpRequestException("Semester not found");

                var offerings = new List<ClassSection>();

                foreach (var offering in request.Offerings)
                {
                    var classSection = new ClassSection
                    {
                        Id = Guid.NewGuid(),
                        CourseId = offering.CourseId,
                        SemesterId = request.SemesterId,
                        MaxCapacity = offering.Capacity,
                        ProfessorId = offering.InstructorId,
                        ClassSectionSchedules = offering.Schedules.Select(s => new ClassSectionSchedule
                        {
                            Id = Guid.NewGuid(),
                            DayOfWeek = s.DayOfWeek,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime
                        }).ToList()
                    };

                    offerings.Add(classSection);
                }

                await _unitOfWork.GetRepository<ClassSection>().InsertRangeAsync(offerings);
                await _unitOfWork.CommitAsync();

                return await GetOfferingsBySemester(request.SemesterId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course offerings");
                throw;
            }
        }

        public async Task<List<CourseOfferingResponse>> GetOfferingsBySemester(Guid semesterId)
        {
            var offerings = await _unitOfWork.GetRepository<ClassSection>()
                .GetListAsync(
                    predicate: co => co.SemesterId == semesterId,
                    include: q => q
                        .Include(co => co.Course)
                        .Include(co => co.Professor)
                            .ThenInclude(p => p.User)
                        .Include(co => co.CourseRegistrations)
                        .Include(co => co.Semester)
                );

            return offerings.Select(co => new CourseOfferingResponse
            {
                CourseOfferingId = co.Id,
                CourseId = co.CourseId,
                CourseCode = co.Course.CourseCode,
                CourseName = co.Course.CourseName,
                Credits = co.Course.Credits,
                ProfessorId = co.ProfessorId,
                ProfessorName = co.Professor?.User.FullName,
                Classroom = co.Classroom?.RoomName ?? "TBD",
                Schedule = string.Join(", ", co.ClassSectionSchedules.Select(s => 
                    $"{s.DayOfWeek} {s.StartTime:hh\\:mm}-{s.EndTime:hh\\:mm}")),
                Capacity = co.MaxCapacity,
                RegisteredCount = co.CourseRegistrations?.Count ?? 0,
                AvailableSlots = co.MaxCapacity - (co.CourseRegistrations?.Count ?? 0),
                SemesterName = co.Semester.SemesterName,
                StartDate = co.Semester.StartDate,
                EndDate = co.Semester.EndDate
            }).ToList();
        }

        public async Task<List<StudentInfoResponse>> GetEnrolledStudents(Guid courseOfferingId)
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
                    Mssv = r.Student.User.UserCode,
                    FullName = r.Student.User.FullName,
                    Email = r.Student.User.Email,
                    MajorName = r.Student.Major.MajorName,
                    EnrollmentDate = r.Student.EnrollmentDate,
                    ImageUrl = r.Student.User.Image
                }).ToList();
        }

        public async Task<CourseOfferingResponse> UpdateCourseOffering(Guid courseOfferingId, CourseOfferingUpdateRequest request)
        {
            var offering = await _unitOfWork.GetRepository<ClassSection>()
                .SingleOrDefaultAsync(
                    predicate: co => co.Id == courseOfferingId,
                    include: q => q
                        .Include(co => co.Course)
                        .Include(co => co.Professor)
                            .ThenInclude(p => p.User)
                        .Include(co => co.CourseRegistrations)
                        .Include(co => co.ClassSectionSchedules)
                        .Include(co => co.Semester)
                );

            if (offering == null)
                throw new BadHttpRequestException("Course offering not found");

            // Update schedules
            offering.ClassSectionSchedules.Clear();
            offering.ClassSectionSchedules = request.Schedules.Select(s => new ClassSectionSchedule
            {
                Id = Guid.NewGuid(),
                ClassSectionId = courseOfferingId,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            }).ToList();

            offering.MaxCapacity = request.Capacity;
            offering.ProfessorId = request.InstructorId;
            offering.ClassroomId = request.ClassroomId;

            await _unitOfWork.CommitAsync();

            return new CourseOfferingResponse
            {
                CourseOfferingId = offering.Id,
                CourseId = offering.CourseId,
                CourseCode = offering.Course.CourseCode,
                CourseName = offering.Course.CourseName,
                Credits = offering.Course.Credits,
                ProfessorId = offering.ProfessorId,
                ProfessorName = offering.Professor?.User.FullName,
                Classroom = offering.Classroom?.RoomName ?? "TBD",
                Schedule = string.Join(", ", offering.ClassSectionSchedules.Select(s => 
                    $"{s.DayOfWeek} {s.StartTime:hh\\:mm}-{s.EndTime:hh\\:mm}")),
                Capacity = offering.MaxCapacity,
                RegisteredCount = offering.CourseRegistrations?.Count ?? 0,
                AvailableSlots = offering.MaxCapacity - (offering.CourseRegistrations?.Count ?? 0),
                SemesterName = offering.Semester.SemesterName,
                StartDate = offering.Semester.StartDate,
                EndDate = offering.Semester.EndDate
            };
        }

        public async Task<bool> DeleteCourseOffering(Guid courseOfferingId)
        {
            var offering = await _unitOfWork.GetRepository<ClassSection>()
                .SingleOrDefaultAsync(
                    predicate: co => co.Id == courseOfferingId,
                    include: q => q.Include(co => co.CourseRegistrations)
                );

            if (offering == null)
                return false;

            if (offering.CourseRegistrations?.Any() == true)
                throw new BadHttpRequestException("Cannot delete course offering with existing registrations");

            _unitOfWork.GetRepository<ClassSection>().DeleteAsync(offering);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<CourseOfferingResponse> GetOfferingById(Guid offeringId)
        {
            var offering = await _unitOfWork.GetRepository<ClassSection>()
                .SingleOrDefaultAsync(
                    predicate: co => co.Id == offeringId,
                    include: q => q
                        .Include(co => co.Course)
                        .Include(co => co.Professor)
                            .ThenInclude(p => p.User)
                        .Include(co => co.CourseRegistrations)
                        .Include(co => co.ClassSectionSchedules)
                        .Include(co => co.Classroom)
                        .Include(co => co.Semester)
                );

            if (offering == null)
                return null;

            return new CourseOfferingResponse
            {
                CourseOfferingId = offering.Id,
                CourseId = offering.CourseId,
                CourseCode = offering.Course.CourseCode,
                CourseName = offering.Course.CourseName,
                Credits = offering.Course.Credits,
                ProfessorId = offering.ProfessorId,
                ProfessorName = offering.Professor?.User.FullName,
                Classroom = offering.Classroom?.RoomName ?? "TBD",
                Schedule = string.Join(", ", offering.ClassSectionSchedules.Select(s => 
                    $"{s.DayOfWeek} {s.StartTime:hh\\:mm}-{s.EndTime:hh\\:mm}")),
                Capacity = offering.MaxCapacity,
                RegisteredCount = offering.CourseRegistrations?.Count ?? 0,
                AvailableSlots = offering.MaxCapacity - (offering.CourseRegistrations?.Count ?? 0),
                SemesterName = offering.Semester.SemesterName,
                StartDate = offering.Semester.StartDate,
                EndDate = offering.Semester.EndDate
            };
        }

        public async Task<CourseOfferingResponse> UpdateOffering(Guid offeringId, CourseOfferingUpdateRequest request)
        {
            try
            {
                var offering = await _unitOfWork.GetRepository<ClassSection>()
                    .SingleOrDefaultAsync(
                        predicate: co => co.Id == offeringId,
                        include: q => q
                            .Include(co => co.Course)
                            .Include(co => co.Professor)
                                .ThenInclude(p => p.User)
                            .Include(co => co.CourseRegistrations)
                            .Include(co => co.ClassSectionSchedules)
                            .Include(co => co.Classroom)
                            .Include(co => co.Semester)
                    );

                if (offering == null)
                    throw new BadHttpRequestException("Course offering not found");

                // Verify if the new capacity is valid
                if (request.Capacity < (offering.CourseRegistrations?.Count ?? 0))
                    throw new BadHttpRequestException("New capacity cannot be less than current registered students");

                // Verify if professor exists
                var professor = await _unitOfWork.GetRepository<Professor>()
                    .SingleOrDefaultAsync(predicate: p => p.Id == request.InstructorId);
                if (professor == null)
                    throw new BadHttpRequestException("Professor not found");

                // Verify classroom if provided
                if (request.ClassroomId.HasValue)
                {
                    var classroom = await _unitOfWork.GetRepository<Classroom>()
                        .SingleOrDefaultAsync(predicate: c => c.Id == request.ClassroomId);
                    if (classroom == null)
                        throw new BadHttpRequestException("Classroom not found");
                }

                // Update schedules
                offering.ClassSectionSchedules.Clear();
                offering.ClassSectionSchedules = request.Schedules.Select(s => new ClassSectionSchedule
                {
                    Id = Guid.NewGuid(),
                    ClassSectionId = offeringId,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToList();

                // Update other properties
                offering.MaxCapacity = request.Capacity;
                offering.ProfessorId = request.InstructorId;
                offering.ClassroomId = request.ClassroomId;

                await _unitOfWork.CommitAsync();

                return await GetOfferingById(offeringId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course offering: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteOffering(Guid offeringId)
        {
            try
            {
                var offering = await _unitOfWork.GetRepository<ClassSection>()
                    .SingleOrDefaultAsync(
                        predicate: co => co.Id == offeringId,
                        include: q => q
                            .Include(co => co.CourseRegistrations)
                            .Include(co => co.ClassSectionSchedules)
                    );

                if (offering == null)
                    return false;

                // Check if there are any registered students
                if (offering.CourseRegistrations?.Any() == true)
                    throw new BadHttpRequestException("Cannot delete course offering with existing registrations");

                // Delete schedules
                foreach (var schedule in offering.ClassSectionSchedules.ToList())
                {
                    _unitOfWork.GetRepository<ClassSectionSchedule>().DeleteAsync(schedule);
                }

                // Delete the offering
                _unitOfWork.GetRepository<ClassSection>().DeleteAsync(offering);
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course offering: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<List<CourseOfferingResponse>> GetProfessorOfferingsBySemester(Guid professorId, Guid semesterId)
        {
            var offerings = await _unitOfWork.GetRepository<ClassSection>()
                .GetListAsync(
                    predicate: cs => cs.SemesterId == semesterId && cs.ProfessorId == professorId,
                    include: q => q
                        .Include(cs => cs.Course)
                        .Include(cs => cs.Professor).ThenInclude(p => p.User)
                        .Include(cs => cs.ClassSectionSchedules)
                        .Include(cs => cs.Semester)
                );
            return offerings.Select(cs => new CourseOfferingResponse
            {
                CourseOfferingId = cs.Id,
                CourseId = cs.CourseId,
                CourseCode = cs.Course.CourseCode,
                CourseName = cs.Course.CourseName,
                Credits = cs.Course.Credits,
                ProfessorId = cs.ProfessorId,
                ProfessorName = cs.Professor?.User.FullName,
                Classroom = cs.Classroom?.RoomName ?? "TBD",
                Schedule = string.Join(", ", cs.ClassSectionSchedules.Select(s => $"{s.DayOfWeek} {s.StartTime:hh\\:mm}-{s.EndTime:hh\\:mm}")),
                Capacity = cs.MaxCapacity,
                RegisteredCount = cs.CourseRegistrations?.Count ?? 0,
                AvailableSlots = cs.MaxCapacity - (cs.CourseRegistrations?.Count ?? 0),
                SemesterName = cs.Semester.SemesterName,
                StartDate = cs.Semester.StartDate,
                EndDate = cs.Semester.EndDate
            }).ToList();
        }

        public async Task<List<StudentInfoResponse>> GetStudentsInOffering(Guid offeringId)
        {
            var registrations = await _unitOfWork.GetRepository<CourseRegistration>()
                .GetListAsync(
                    predicate: r => r.ClassSectionId == offeringId && r.Status != "Dropped",
                    include: q => q
                        .Include(r => r.Student).ThenInclude(s => s.User)
                        .Include(r => r.Student).ThenInclude(s => s.Major)
                );
            return registrations
                .OrderBy(r => r.Student.User.FullName)
                .Select(r => new StudentInfoResponse
                {
                    Id = r.Student.Id,
                    Mssv = r.Student.User.UserCode,
                    FullName = r.Student.User.FullName,
                    Email = r.Student.User.Email,
                    MajorName = r.Student.Major.MajorName,
                    EnrollmentDate = r.Student.EnrollmentDate,
                    ImageUrl = r.Student.User.Image
                }).ToList();
        }
    }
}

