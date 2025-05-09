using System;

namespace CDQTSystem_API.Payload.Request
{
    public class TuitionPaymentFilterRequest
    {
        public Guid? StudentId { get; set; }
        public Guid? TermId { get; set; }
        public string Status { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 20;
    }
} 