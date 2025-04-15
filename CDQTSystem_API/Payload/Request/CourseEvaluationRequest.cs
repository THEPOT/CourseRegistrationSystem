namespace CDQTSystem_API.Payload.Request
{
	public class SubmitCourseEvaluationRequest
	{
		public Guid ClassSectionId { get; set; }
		public int TeachingQualityRating { get; set; }
		public int CourseContentRating { get; set; }
		public int AssessmentFairnessRating { get; set; }
		public int OverallSatisfactionRating { get; set; }
		public string Feedback { get; set; }
	}

	public class CourseEvaluationPeriodRequest
	{
		public Guid SemesterId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool IsActive { get; set; }
	}
}
