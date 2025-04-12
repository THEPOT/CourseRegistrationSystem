namespace CDQTSystem_API.Messages
{
    public class CourseRegistrationMessage
    {
        public Guid RequestId { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseOfferingId { get; set; }
        public DateTime RequestTimestamp { get; set; }
    }

    public class CourseRegistrationResult
    {
        public Guid RequestId { get; set; }
        public bool Success { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
