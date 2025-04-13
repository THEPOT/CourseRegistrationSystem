namespace CDQTSystem_API.Payload.Response
{
    public class RegistrationAnalyticsResponse
    {
        public List<TermRegistrationSummary> TermSummaries { get; set; } = new();
        public List<CourseRegistrationSummary> CourseSummaries { get; set; } = new();
        public List<ProgramEnrollmentSummary> ProgramSummaries { get; set; } = new();
        public Dictionary<string, int> DailyRegistrationCounts { get; set; } = new();
    }

    public class TermRegistrationSummary
    {
        public Guid TermId { get; set; }
        public string TermName { get; set; }
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public double AverageCoursesPerStudent { get; set; }
        public Dictionary<string, int> RegistrationsByProgram { get; set; } = new();
    }

    public class CourseRegistrationSummary
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalSections { get; set; }
        public double AverageStudentsPerSection { get; set; }
        public Dictionary<string, int> RegistrationsByProgram { get; set; } = new();
    }

    public class ProgramEnrollmentSummary
    {
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public int TotalStudents { get; set; }
        public double AverageCoursesPerStudent { get; set; }
        public List<YearLevelEnrollment> YearLevelBreakdown { get; set; } = new();
    }

    public class YearLevelEnrollment
    {
        public int YearLevel { get; set; }
        public int StudentCount { get; set; }
        public double AverageCoursesPerStudent { get; set; }
    }
}