namespace CDQTSystem_API.Payload.Request
{
    public class RegistrationPeriodCreateRequest
    {
        public Guid SemesterId { get; set; }
        public int MaxCredits { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}