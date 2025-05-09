using System;

namespace CDQTSystem_API.Payload.Response
{
    public class TuitionPaymentResponse
    {
        public Guid PaymentId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public string Mssv { get; set; }
        public Guid TermId { get; set; }
        public string TermName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime PaymentDate { get; set; }
    }
} 