namespace CDQTSystem_API.Payload.Request
{
    public class ScholarshipAssignmentRequest
    {
        public Guid ScholarshipId { get; set; }
        public DateOnly AwardDate { get; set; }
    }
}