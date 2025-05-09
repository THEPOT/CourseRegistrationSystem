namespace CDQTSystem_API.Payload.Response
{
	public class GradeResponse
	{
		public Guid Id { get; set; }
		public Guid CourseRegistrationId { get; set; }
		public decimal QualityPoints { get; set; }
		public string GradeValue { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}