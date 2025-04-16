namespace CDQTSystem_API.Payload.Response
{
	public class MidtermEvaluationResponse
	{
		public Guid Id { get; set; }
		public Guid StudentId { get; set; }
		public string StudentName { get; set; }
		public string StudentMssv { get; set; }
		public Guid ProfessorId { get; set; }
		public string ProfessorName { get; set; }
		public Guid CourseId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public Guid ClassSectionId { get; set; }
		public Guid SemesterId { get; set; }
		public string SemesterName { get; set; }
		public string Comments { get; set; }
		public string Recommendation { get; set; }
		public DateTime EvaluationDate { get; set; }
	}

	public class MidtermEvaluationPeriodResponse
	{
		public Guid Id { get; set; }
		public Guid SemesterId { get; set; }
		public string SemesterName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool IsActive { get; set; }
		public bool IsCurrentlyOpen => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate && IsActive;
	}

	public class MidtermEvaluationSummaryResponse
	{
		public Guid CourseId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public int TotalStudents { get; set; }
		public int EvaluatedStudents { get; set; }
		public Dictionary<string, int> StatusBreakdown { get; set; } // e.g., {"At Risk": 5, "Good Standing": 15}
		public Guid ClassSectionId { get; internal set; }
		public string TermName { get; internal set; }
		public List<StudentEvaluationDetail> StudentEvaluations { get; internal set; }
	}
	public class StudentForEvaluationResponse
	{
		public Guid StudentId { get; set; }
		public string Mssv { get; set; }
		public string FullName { get; set; }
		public bool HasExistingEvaluation { get; set; }
		public string ExistingStatus { get; set; }
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
