namespace CDQTSystem_API.Payload.Response
{
    public class ProgramRegistrationStatisticsResponse
    {
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public int TotalStudents { get; set; }
        public int RegisteredStudents { get; set; }
        public double RegistrationRate { get; set; }
        public double AverageCredits { get; set; }

        public YearLevelStatistics YearLevelBreakdown { get; set; }
        public List<CourseEnrollmentSummary> TopCourses { get; set; } = new List<CourseEnrollmentSummary>();
        public List<RegistrationTrend> DailyTrends { get; set; } = new List<RegistrationTrend>();
    }

    public class YearLevelStatistics
    {
        public YearStats FirstYear { get; set; }
        public YearStats SecondYear { get; set; }
        public YearStats ThirdYear { get; set; }
        public YearStats FourthYear { get; set; }
    }

    public class YearStats
    {
        public int TotalStudents { get; set; }
        public int RegisteredCount { get; set; }
        public double RegistrationRate { get; set; }
        public double AverageCredits { get; set; }
    }

    public class CourseEnrollmentSummary
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int EnrolledCount { get; set; }
        public double EnrollmentRate { get; set; }
        public int WaitlistedCount { get; set; }
    }

    public class RegistrationTrend
    {
        public DateTime Date { get; set; }
        public int NewRegistrations { get; set; }
        public int TotalRegistered { get; set; }
        public int WaitlistAdditions { get; set; }
        public int CourseDrops { get; set; }
    }
}
