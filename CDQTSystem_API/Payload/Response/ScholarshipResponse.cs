using System;

namespace CDQTSystem_API.Payload.Response
{
    public class ScholarshipResponse
    {
        public Guid Id { get; set; }
        public string ScholarshipName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string EligibilityCriteria { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }

    public class PaginatedScholarshipResponse
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public List<ScholarshipResponse> Items { get; set; }
    }
} 