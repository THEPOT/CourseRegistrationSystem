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

		public List<RegistrationPhaseInfo> Phases { get; set; } = new List<RegistrationPhaseInfo>();
        public RegistrationStatistics CurrentStatistics { get; set; }
    }

    public class RegistrationPhaseInfo
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }  // Upcoming, Active, Completed
        public string Description { get; set; }
        public List<string> EligibleYears { get; set; }
        public int RegisteredStudents { get; set; }
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