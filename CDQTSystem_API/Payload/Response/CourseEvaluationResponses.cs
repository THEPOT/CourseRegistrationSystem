namespace CDQTSystem_API.Payload.Response
{
	public class CourseOfferingForEvaluationResponse
	{
		public Guid CourseOfferingId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public Guid ProfessorId { get; set; }
		public string ProfessorName { get; set; }
	}

	public class CourseEvaluationSummaryResponse
	{
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public double AverageRating { get; set; }
		public int TotalEvaluations { get; set; }
		public List<string> Comments { get; set; } = new List<string>();
	}
}
