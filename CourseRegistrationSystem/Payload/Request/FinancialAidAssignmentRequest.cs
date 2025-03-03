namespace CourseRegistration_API.Payload.Request
{
    public class FinancialAidAssignmentRequest
    {
        public Guid FinancialAidId { get; set; }
        public DateOnly AwardDate { get; set; }
    }
}