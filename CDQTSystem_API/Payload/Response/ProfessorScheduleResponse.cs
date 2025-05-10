namespace CDQTSystem_API.Payload.Response
{
	public class ProfessorScheduleResponse
	{
		public Guid ClassSectionId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public int Credits { get; set; }
		public string SemesterName { get; set; }
		public string Classroom { get; set; }
		public List<ScheduleSlot> Schedule { get; set; } = new List<ScheduleSlot>();
		public List<ClassSessionSlot> Sessions { get; set; } = new List<ClassSessionSlot>();
	}

	public class ClassSessionSlot
	{
		public string Date { get; set; }
		public string DayOfWeek { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public string Status { get; set; }
		public string Note { get; set; }
	}
}