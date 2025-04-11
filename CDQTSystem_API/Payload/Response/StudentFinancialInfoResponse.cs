namespace CDQTSystem_API.Payload.Response
{
	public class StudentFinancialInfoResponse
	{
		public Guid StudentId { get; set; }
		public string StudentName { get; set; }
		public decimal TotalTuitionFee { get; set; }
		public decimal PaidAmount { get; set; }
		public decimal RemainingBalance { get; set; }
		public List<ScholarshipInfo> Scholarships { get; set; }
		public List<TuitionPaymentHistory> PaymentHistory { get; set; }
		public List<FinancialAidInfo> FinancialAids { get; set; } = new List<FinancialAidInfo>();

		public string PaymentStatus { get; set; }
		public DateTime? LastPaymentDate { get; set; }
	}

	public class ScholarshipInfo
	{
		public Guid ScholarshipId { get; set; }
		public string ScholarshipName { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public string EligibilityCriteria { get; set; }
		public DateOnly AwardDate { get; set; }
	}

	public class TuitionPaymentHistory
	{
		public DateTime PaymentDate { get; set; }
		public decimal Amount { get; set; }
		public string PaymentMethod { get; set; }
		public string TransactionId { get; set; }
		public string Status { get; set; }
	}

	public class FinancialAidInfo
	{
		public Guid FinancialAidId { get; set; }
		public string AidName { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }

		public DateOnly AwardDate { get; set; }
	}
}
