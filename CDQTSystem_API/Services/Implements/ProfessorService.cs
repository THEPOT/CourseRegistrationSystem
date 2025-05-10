using AutoMapper;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Domain.Paginate;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_API.Services.Implements
{
	public class ProfessorService : BaseService<ProfessorService>, IProfessorService
	{
		public ProfessorService(IUnitOfWork<DbContext> unitOfWork,
			ILogger<ProfessorService> logger,
			IMapper mapper,
			IHttpContextAccessor httpContextAccessor)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public async Task<IPaginate<ProfessorResponse>> GetAllProfessors(int page, int size, string? search)
		{
			try
			{
				IPaginate<ProfessorResponse> professors = await _unitOfWork.GetRepository<Professor>()
					.GetPagingListAsync(
					selector: p => new ProfessorResponse
					{
						Id = p.Id,
						FullName = p.User.FullName,
						Email = p.User.Email,
						PhoneNumber = p.User.PhoneNumber,
						DepartmentName = p.Department.DepartmentName,
					},
					predicate: p => string.IsNullOrEmpty(search) || p.User.FullName.Contains(search),
						include: q => q
							.Include(p => p.User)
							.Include(p => p.Department),
						orderBy: q => q.OrderBy(p => p.User.FullName)
					);
				return professors;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all professors");
				throw;
			}
		}

		public async Task<List<ProfessorScheduleResponse>> GetProfessorSchedule(Guid userId, int? year, int? week)
		{
			// Lấy thông tin professor
			var professor = await _unitOfWork.GetRepository<Professor>()
				.SingleOrDefaultAsync(predicate: p => p.UserId == userId, include: q => q.Include(p => p.User));
			if (professor == null)
				throw new BadHttpRequestException("Professor not found");

			// Lấy danh sách các lớp mà giảng viên phụ trách
			var classSections = await _unitOfWork.GetRepository<ClassSection>()
				.GetListAsync(
					predicate: cs => cs.ProfessorId == professor.Id,
					include: q => q
						.Include(cs => cs.Course)
						.Include(cs => cs.Semester)
						.Include(cs => cs.Classroom)
						.Include(cs => cs.ClassSectionSchedules)
						.Include(cs => cs.ClassSessions)
				);

			// Lấy tất cả các buổi học thực tế từ ClassSession
			var allSessions = classSections
				.SelectMany(cs => cs.ClassSessions.Select(sess => new { Section = cs, Session = sess }))
				.ToList();

			// Nếu có week/year thì lọc theo tuần
			if (week != null && year != null)
			{
				var (startOfWeek, endOfWeek) = GetDateRangeOfWeek(week.Value, year.Value);
				allSessions = allSessions
					.Where(x => x.Session.Date.ToDateTime(TimeOnly.MinValue) >= startOfWeek && x.Session.Date.ToDateTime(TimeOnly.MinValue) <= endOfWeek)
					.ToList();
			}

			// Gom nhóm lại theo lớp
			var grouped = allSessions
				.GroupBy(x => x.Section)
				.Select(g => new ProfessorScheduleResponse
				{
					ClassSectionId = g.Key.Id,
					CourseCode = g.Key.Course.CourseCode,
					CourseName = g.Key.Course.CourseName,
					Credits = g.Key.Course.Credits,
					SemesterName = g.Key.Semester.SemesterName,
					Classroom = g.Key.Classroom != null ? g.Key.Classroom.RoomName : "Online",
					Schedule = g.Key.ClassSectionSchedules.Select(s => new ScheduleSlot
					{
						DayOfWeek = s.DayOfWeek,
						StartTime = s.StartTime.ToString("HH:mm"),
						EndTime = s.EndTime.ToString("HH:mm")
					}).ToList(),
					Sessions = g.Select(x => new ClassSessionSlot
					{
						Date = x.Session.Date.ToString("yyyy-MM-dd"),
						DayOfWeek = x.Session.DayOfWeek,
						StartTime = x.Session.StartTime.ToString("HH:mm"),
						EndTime = x.Session.EndTime.ToString("HH:mm"),
						Status = x.Session.Status,
						Note = x.Session.Note ?? string.Empty
					}).ToList()
				})
				.Where(r => r.Sessions.Any())
				.ToList();

			return grouped;
		}
		public static (DateTime start, DateTime end) GetDateRangeOfWeek(int week, int year)
		{
			var jan1 = new DateTime(year, 1, 1);
			int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
			var firstMonday = jan1.AddDays(daysOffset);
			var startOfWeek = firstMonday.AddDays((week - 1) * 7);
			// endOfWeek là 23:59:59 của ngày Chủ nhật
			var endOfWeek = startOfWeek.AddDays(6).Date.AddDays(1).AddTicks(-1);
			return (startOfWeek, endOfWeek);
		}
	}
}