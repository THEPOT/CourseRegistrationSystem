namespace CourseRegistration_API.Payload.Request
{
	public class MidtermEvaluationCreateRequest
	{
		public Guid CourseRegistrationId { get; set; }
		public string Status { get; set; } // Good, Satisfactory, At Risk, Failing, etc.
		public string Recommendation { get; set; }
	}

	public class MidtermEvaluationBatchRequest
	{
		public Guid CourseOfferingId { get; set; }
		public List<MidtermEvaluationItem> Evaluations { get; set; } = new List<MidtermEvaluationItem>();
	}

	public class MidtermEvaluationItem
	{
		public Guid StudentId { get; set; }
		public string Status { get; set; }
		public string Recommendation { get; set; }
	}
}
