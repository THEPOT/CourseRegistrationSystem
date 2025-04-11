namespace CDQTSystem_API.Payload.Response
{
    public class TuitionPeriodResponse
    {
        public Guid Id { get; set; }
        public Guid SemesterId { get; set; }
        public string SemesterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public decimal BaseTuitionFee { get; set; }
        public decimal CreditFee { get; set; }
        public string PaymentInstructions { get; set; }
        
        public List<TuitionPolicyInfo> Policies { get; set; } = new List<TuitionPolicyInfo>();
        public TuitionStatistics Statistics { get; set; }
    }

    public class TuitionPolicyInfo
    {
        public Guid Id { get; set; }
        public string PolicyType { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string Description { get; set; }
        public int StudentsApplied { get; set; }
    }

    public class TuitionStatistics
    {
        public int TotalStudents { get; set; }
        public int PaidStudents { get; set; }
        public int PartiallyPaidStudents { get; set; }
        public int UnpaidStudents { get; set; }
        public decimal TotalExpectedAmount { get; set; }
        public decimal TotalCollectedAmount { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
        public List<PaymentMethodSummary> PaymentMethods { get; set; } = new List<PaymentMethodSummary>();
    }

    public class PaymentMethodSummary
    {
        public string Method { get; set; }
        public int TransactionCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}