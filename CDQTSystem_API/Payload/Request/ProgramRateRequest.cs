using System;

namespace CDQTSystem_API.Payload.Request
{
    public class ProgramRateRequest
    {
        public Guid ProgramId { get; set; }
        public decimal Amount { get; set; }
    }
} 