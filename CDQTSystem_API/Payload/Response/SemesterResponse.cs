namespace CDQTSystem_API.Payload.Response
{
    public class SemesterResponse
    {
        public Guid Id { get; set; }
        public required string SemesterName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public  string AcademicYear { get; set; }
        public  string Status { get; set; }
    }
}
