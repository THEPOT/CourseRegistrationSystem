namespace CDQTSystem_API.Payload.Request
{
    public class SemesterCreateRequest
    {
        public required string Name { get; set; }
        public required string StartDate { get; set; }
        public required string EndDate { get; set; }
        public required string AcademicYear { get; set; }
        public required string Status { get; set; }
    }
}
