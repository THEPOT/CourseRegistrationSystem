namespace CDQTSystem_API.Payload.Request
{
	public class CourseEvaluationCreateRequest
	{
		public Guid CourseOfferingId { get; set; }
		public Guid StudentId { get; set; }
		public int TeachingQualityRating { get; set; }
		public int CourseContentRating { get; set; }
		public int AssessmentFairnessRating { get; set; }
		public int OverallSatisfactionRating { get; set; }
		public string? Feedback { get; set; }
	}
}
