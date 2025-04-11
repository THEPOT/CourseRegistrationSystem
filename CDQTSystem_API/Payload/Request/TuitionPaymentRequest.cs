namespace CDQTSystem_API.Payload.Request
{
    public class TuitionPaymentRequest
    {
        public Guid StudentId { get; set; }
        public Guid SemesterId { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; }  // Bank Transfer, Cash, Online Payment
        public string TransactionReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Notes { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string DiscountReason { get; set; }
        public List<Guid> CourseRegistrationIds { get; set; } = new List<Guid>();
    }
}
