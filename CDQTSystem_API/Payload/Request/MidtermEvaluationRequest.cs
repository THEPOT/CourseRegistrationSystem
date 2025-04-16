namespace CDQTSystem_API.Payload.Request
{
	public class CreateMidtermEvaluationRequest
	{
		public Guid StudentId { get; set; }
		public Guid ClassSectionId { get; set; }
		public string Status { get; set; }
		public string Comments { get; set; }
		public string Recommendation { get; set; }
	}
	public class UpdateMidtermEvaluationRequest
	{
		public string Status { get; set; }
		public string Comments { get; set; }
		public string Recommendation { get; set; }
	}

	public class MidtermEvaluationPeriodRequest
	{
		public Guid SemesterId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool IsActive { get; set; }
	}
}
