namespace CourseRegistration_API.Payload.Request
{
    public class StudentCreateRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public Guid MajorId { get; set; }
        public DateOnly? AdmissionDate { get; set; }
        public string AdmissionStatus { get; set; }
        public string ImageUrl { get; set; }

        // Add scholarship information (optional)
        public Guid? ScholarshipId { get; set; }
        public DateOnly? ScholarshipAwardDate { get; set; }
    }
}