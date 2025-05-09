namespace CDQTSystem_API.Payload.Request
{
    public class BatchProgressFilterRequest
    {
        public int? Year { get; set; }
        public string ProgramCode { get; set; }
        public string Term { get; set; }
    }
} 