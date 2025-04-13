namespace CDQTSystem_API.Payload.Request
{
    public class TuitionPeriodCreateRequest
    {
        public Guid SemesterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal BaseTuitionFee { get; set; }
        public decimal CreditFee { get; set; }
        public decimal LatePaymentFee { get; set; }
        public List<string> PaymentMethods { get; set; } = new List<string>();
        public string PaymentInstructions { get; set; }
        public List<TuitionPolicyItem> Policies { get; set; } = new List<TuitionPolicyItem>();
    }

    public class TuitionPolicyItem
    {
        public string PolicyType { get; set; }  // Regular, International, Scholarship
        public decimal Amount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string Description { get; set; }
    }
}
