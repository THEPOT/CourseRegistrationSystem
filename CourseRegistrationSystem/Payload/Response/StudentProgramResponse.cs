namespace CourseRegistration_API.Payload.Response
{
    public class StudentProgramResponse
    {
        public Guid ProgramId { get; set; }
        public string ProgramName { get; set; }
        public int RequiredCredits { get; set; }
        public string FacultyName { get; set; }
        public List<CourseInfo> Courses { get; set; } = new List<CourseInfo>();
    }

    public class CourseInfo
    {
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public string Description { get; set; }
    }
}