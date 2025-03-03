namespace CourseRegistration_API.Payload.Request
{
	public class StudentTuitionUpdateRequest
	{
		public Guid TuitionPolicyId { get; set; }
		public string Semester { get; set; }
		public bool IsPaid { get; set; }
		public decimal? AmountPaid { get; set; }
		public decimal? DiscountAmount { get; set; }
		public DateOnly? PaymentDueDate { get; set; }
		public DateOnly? PaymentDate { get; set; }
		public string Notes { get; set; }
	}
}
