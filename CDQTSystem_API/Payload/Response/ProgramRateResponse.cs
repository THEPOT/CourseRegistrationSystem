using System;

namespace CDQTSystem_API.Payload.Response
{
    public class ProgramRateResponse
    {
        public Guid ProgramId { get; set; }
        public string ProgramName { get; set; }
        public decimal Amount { get; set; }
    }
} 