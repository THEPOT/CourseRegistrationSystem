namespace CourseRegistration_API.Payload.Response
{
    public class StudentFinancialInfoResponse
    {
        public List<ScholarshipInfo> Scholarships { get; set; } = new List<ScholarshipInfo>();
        public List<FinancialAidInfo> FinancialAids { get; set; } = new List<FinancialAidInfo>();
    }

    public class ScholarshipInfo
    {
        public Guid ScholarshipId { get; set; }
        public string ScholarshipName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string EligibilityCriteria { get; set; }
        public DateOnly AwardDate { get; set; }
    }

    public class FinancialAidInfo
    {
        public Guid FinancialAidId { get; set; }
        public string AidName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateOnly AwardDate { get; set; }
    }
}