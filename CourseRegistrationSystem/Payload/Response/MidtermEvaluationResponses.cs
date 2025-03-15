namespace CourseRegistration_API.Payload.Response
{
	public class StudentForEvaluationResponse
	{
		public Guid StudentId { get; set; }
		public string Mssv { get; set; }
		public string FullName { get; set; }
		public bool HasExistingEvaluation { get; set; }
		public string ExistingStatus { get; set; }
	}

	public class MidtermEvaluationSummaryResponse
	{
		public Guid ClassSectionId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public string TermName { get; set; }
		public List<StudentEvaluationDetail> StudentEvaluations { get; set; } = new List<StudentEvaluationDetail>();
	}

	public class StudentEvaluationDetail
	{
		public Guid StudentId { get; set; }
		public string Mssv { get; set; }
		public string FullName { get; set; }
		public string Status { get; set; }
		public string Recommendation { get; set; }
	}
}
