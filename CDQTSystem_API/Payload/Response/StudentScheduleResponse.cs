namespace CDQTSystem_API.Payload.Response
{
    public class StudentScheduleResponse
    {
        public Guid CourseRegistrationId { get; set; }
        public Guid CourseOfferingId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public string ProfessorName { get; set; }
        public string Classroom { get; set; }
        public List<ScheduleSlot> Schedule { get; set; } = new List<ScheduleSlot>();
        public string Status { get; set; }
    }

    public class ScheduleSlot
    {
        public string DayOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
} 