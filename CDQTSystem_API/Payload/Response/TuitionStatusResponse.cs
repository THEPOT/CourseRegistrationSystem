namespace CDQTSystem_API.Payload.Response
{
    public class TuitionStatusResponse
    {
        public Guid StudentId { get; set; }
        public string Mssv { get; set; }
        public string StudentName { get; set; }
        public string SemesterName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string PaymentStatus { get; set; }  // Unpaid, Partial, Paid
        public DateTime? DueDate { get; set; }
        public DateOnly? LastPaymentDate { get; set; }
        public Guid TuitionId { get; set; }
        public Guid SemesterId { get; set; }
        public decimal AmountPaid { get; set; }
        
        public List<TuitionItemDetail> Items { get; set; } = new List<TuitionItemDetail>();
        public List<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
    }

    public class TuitionItemDetail
    {
        public string ItemType { get; set; }  // Base Fee, Credit Fee, Additional Fee
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string DiscountReason { get; set; }
        public decimal FinalAmount { get; set; }
    }

    public class PaymentTransaction
    {
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionReference { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}