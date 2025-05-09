namespace CDQTSystem_API.Payload.Request
{
    public class AuditExportRequest
    {
        public string ProgramCode { get; set; }
        public int? Year { get; set; }
        public string Term { get; set; }
    }
} 