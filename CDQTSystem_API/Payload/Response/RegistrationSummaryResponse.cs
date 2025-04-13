namespace CDQTSystem_API.Payload.Response
{
    public class RegistrationSummaryResponse
    {
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int TotalCapacity { get; set; }
        public int RegisteredCount { get; set; }
        public int WaitlistedCount { get; set; }
        public double FillRate { get; set; }
        public List<SectionSummary> Sections { get; set; } = new List<SectionSummary>();
    }

    public class SectionSummary
    {
        public Guid SectionId { get; set; }
        public string SectionCode { get; set; }
        public string Instructor { get; set; }
        public int Capacity { get; set; }
        public int Registered { get; set; }
        public int Waitlisted { get; set; }
        public string Schedule { get; set; }
    }
}