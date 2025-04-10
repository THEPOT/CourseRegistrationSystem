namespace CourseRegistration_API.Payload.Response
{
    public class AvailableCourseResponse
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string ClassName { get; set; }
        public int Capacity { get; set; }
        public int AvailableSlots { get; set; }
        public string Schedule { get; set; }
        public Guid? ProfessorId { get; set; }  // Make it nullable
    }
}
