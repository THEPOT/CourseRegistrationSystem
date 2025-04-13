namespace CDQTSystem_API.Payload.Response
{
    public class AvailableCourseResponse
    {
        public Guid CourseOfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public Guid? ProfessorId { get; set; }
        public string ProfessorName { get; set; }
        public string Schedule { get; set; }
        public int Capacity { get; set; }
        public int RegisteredCount { get; set; }
        public int AvailableSlots { get; set; }
        public bool PrerequisitesSatisfied { get; set; }
        public List<string> Prerequisites { get; set; } = new List<string>();
    }
}

