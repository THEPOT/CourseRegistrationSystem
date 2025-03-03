namespace CourseRegistration_API.Payload.Response
{
    public class StudentInfoResponse
    {
        public Guid Id { get; set; }
        public string Mssv { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProgramName { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public DateOnly? AdmissionDate { get; set; }
        public string AdmissionStatus { get; set; }
        public string ImageUrl { get; set; }
    }
}
