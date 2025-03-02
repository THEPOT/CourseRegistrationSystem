public class StudentTuitionCreateRequest
{
    public Guid TuitionPolicyId { get; set; }
    public string Semester { get; set; }
    public bool IsPaid { get; set; }
    public DateOnly? PaymentDate { get; set; }
} 