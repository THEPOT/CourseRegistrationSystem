namespace CourseRegistration_API.Payload.Request
{
	public class CourseEvaluationCreateRequest
	{
		public Guid CourseOfferingId { get; set; }
		public Guid StudentId { get; set; }
		public int Rating { get; set; }
		public string Comments { get; set; }
	}
}
