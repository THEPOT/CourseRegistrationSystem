namespace CourseRegistration_API.Payload.Response
{
    public class StudentTuitionResponse
    {
        public Guid StudentId { get; set; }
        public string Mssv { get; set; }
        public string StudentName { get; set; }
        public List<TuitionInfo> Tuitions { get; set; } = new List<TuitionInfo>();
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }

    public class TuitionInfo
    {
        public Guid TuitionPolicyId { get; set; }
        public string PolicyName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal DiscountAmount { get; set; }
        public string PaymentStatus { get; set; }
        public DateOnly PaymentDueDate { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public string Semester { get; set; }
        public bool IsPaid { get; set; }
        public DateOnly? PaymentDate { get; set; }
    }
}