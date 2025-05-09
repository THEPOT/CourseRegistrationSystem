using System;

namespace CDQTSystem_API.Payload.Response
{
    public class ScholarshipRecipientResponse
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public string Mssv { get; set; }
        public DateTime AwardDate { get; set; }
        public decimal Amount { get; set; }
    }
} 