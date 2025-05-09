namespace CDQTSystem_API.Payload.Request
{
    public class ScholarshipRequest
    {
        public string ScholarshipName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string EligibilityCriteria { get; set; }
        public Guid DepartmentId { get; set; }
    }
} 