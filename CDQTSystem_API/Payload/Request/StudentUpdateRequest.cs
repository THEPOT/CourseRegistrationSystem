namespace CDQTSystem_API.Payload.Request
{
    public class StudentUpdateRequest
    {
        public string Email { get; set; }  // Added field
        public string FullName { get; set; }
        public string Mssv { get; set; }  // Added field
        public Guid? MajorId { get; set; }
        public DateOnly? EnrollmentDate { get; set; }  // Added field
        public DateOnly? AdmissionDate { get; set; }
        public string AdmissionStatus { get; set; }
        public string ImageUrl { get; set; }
    }
}