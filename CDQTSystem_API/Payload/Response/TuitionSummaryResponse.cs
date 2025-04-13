namespace CDQTSystem_API.Payload.Response
{
    public class TuitionSummaryResponse
    {
        public string SemesterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        // Overall Statistics
        public int TotalStudents { get; set; }
        public decimal TotalExpectedAmount { get; set; }
        public decimal TotalCollectedAmount { get; set; }
        public decimal CollectionRate { get; set; }  // Percentage

        // Payment Status Breakdown
        public PaymentStatusBreakdown StatusBreakdown { get; set; }
        
        // Daily Collection Summary
        public List<DailyCollection> DailyCollections { get; set; } = new List<DailyCollection>();
        
        // Payment Method Summary
        public List<PaymentMethodSummary> PaymentMethods { get; set; } = new List<PaymentMethodSummary>();
        
        // Outstanding Payments
        public List<OutstandingPayment> OutstandingPayments { get; set; } = new List<OutstandingPayment>();
    }

    public class PaymentStatusBreakdown
    {
        public int PaidCount { get; set; }
        public decimal PaidAmount { get; set; }
        public int PartialCount { get; set; }
        public decimal PartialAmount { get; set; }
        public int UnpaidCount { get; set; }
        public decimal UnpaidAmount { get; set; }
        public int OverdueCount { get; set; }
        public decimal OverdueAmount { get; set; }
    }

    public class DailyCollection
    {
        public DateTime Date { get; set; }
        public int TransactionCount { get; set; }
        public decimal TotalAmount { get; set; }
        public Dictionary<string, decimal> ByPaymentMethod { get; set; }
    }

    public class OutstandingPayment
    {
        public string Mssv { get; set; }
        public string StudentName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateOnly DueDate { get; set; }
        public int DaysOverdue { get; set; }
    }
}