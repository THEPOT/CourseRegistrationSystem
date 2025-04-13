namespace CDQTSystem_API.Payload.Response
{
    public class RegistrationStatisticsResponse
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int TotalSections { get; set; }
        public int TotalCapacity { get; set; }
        public int RegisteredStudents { get; set; }
        public int WaitlistedStudents { get; set; }
        public double FillRate { get; set; }
        
        public List<SectionStatistics> Sections { get; set; } = new List<SectionStatistics>();
        public List<YearLevelBreakdown> YearLevelStats { get; set; } = new List<YearLevelBreakdown>();
        public List<ProgramBreakdown> ProgramStats { get; set; } = new List<ProgramBreakdown>();
    }

    public class SectionStatistics
    {
        public string SectionCode { get; set; }
        public string Professor { get; set; }
        public string Schedule { get; set; }
        public int Capacity { get; set; }
        public int Registered { get; set; }
        public int Waitlisted { get; set; }
        public double FillRate { get; set; }
    }

    public class YearLevelBreakdown
    {
        public string YearLevel { get; set; }  // Freshman, Sophomore, etc.
        public int RegisteredCount { get; set; }
        public int WaitlistedCount { get; set; }
        public double Percentage { get; set; }
    }

    public class ProgramBreakdown
    {
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public int RegisteredCount { get; set; }
        public int WaitlistedCount { get; set; }
        public double Percentage { get; set; }
    }
}