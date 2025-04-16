namespace CDQTSystem_API.Payload.Response
{
	public class RegistrationPeriodResponse
	{
		public Guid Id { get; set; }
		public Guid SemesterId { get; set; }
		public string SemesterName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Status { get; set; }
		public int MaxCredits { get; set; }
		public int TotalRegisteredStudents { get; set; }
		public int TotalWaitlistedStudents { get; set; } 
		public List<ClassSectionInfo> ClassSections { get; set; } = new List<ClassSectionInfo>();
		public RegistrationStatistics CurrentStatistics { get; set; }
	}

	public class ClassSectionInfo
	{
		public string CourseName { get; set; }
		public string CourseCode { get; set; }
		public int Credits { get; set; }
		public int MaxCapacity { get; set; }
		public List<ScheduleInfo> Schedules { get; set; } = new List<ScheduleInfo>();
	}

	public class ScheduleInfo{
		public string DayOfWeek { get; set; }
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }
	}


	public class RegistrationStatistics
    {
        public int TotalEligibleStudents { get; set; }
        public int RegisteredStudents { get; set; }
        public int WaitlistedStudents { get; set; }
        public double RegistrationRate { get; set; }
        public int TotalCourseOfferings { get; set; }
        public int FullCourses { get; set; }
    }
}