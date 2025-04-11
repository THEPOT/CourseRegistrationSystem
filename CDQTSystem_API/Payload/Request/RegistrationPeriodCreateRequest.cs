namespace CDQTSystem_API.Payload.Request
{
    public class RegistrationPeriodCreateRequest
    {
        public Guid SemesterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxCredits { get; set; } = 24;
        public int MinCredits { get; set; } = 12;
        public bool AllowWaitlist { get; set; } = true;
        public int WaitlistLimit { get; set; } = 10;
        public List<RegistrationPhase> Phases { get; set; } = new List<RegistrationPhase>();
    }

    public class RegistrationPhase
    {
        public string Name { get; set; }  // Senior, Junior, Sophomore, Freshman
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public List<string> EligibleYears { get; set; } = new List<string>();
    }
}