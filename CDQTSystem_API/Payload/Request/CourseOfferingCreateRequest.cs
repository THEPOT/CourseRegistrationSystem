namespace CDQTSystem_API.Payload.Request
{
    public class CourseOfferingCreateRequest
    {
        public Guid SemesterId { get; set; }
        public List<OfferingDetail> Offerings { get; set; }
    }

    public class OfferingDetail
    {
        public Guid CourseId { get; set; }
        public int Capacity { get; set; }
        public Guid InstructorId { get; set; }
        public List<ScheduleDetail> Schedules { get; set; }
    }

    public class ScheduleDetail
    {
        public string DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
