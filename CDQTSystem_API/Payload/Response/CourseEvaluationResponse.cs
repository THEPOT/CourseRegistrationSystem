namespace CDQTSystem_API.Payload.Response
{
	public class CourseEvaluationSummaryResponse
	{
		public Guid CourseId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public Guid ProfessorId { get; set; }
		public string ProfessorName { get; set; }
		public int TotalResponses { get; set; }
		public double AverageTeachingQuality { get; set; }
		public double AverageCourseContent { get; set; }
		public double AverageAssessmentFairness { get; set; }
		public double AverageOverallSatisfaction { get; set; }
		public List<string> CommonFeedback { get; set; }
	}

	public class CourseOfferingForEvaluationResponse
	{
		public Guid CourseOfferingId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public Guid ProfessorId { get; set; }
		public string ProfessorName { get; set; }
	}

	public class ProfessorEvaluationSummaryResponse
	{
		public Guid ProfessorId { get; set; }
		public string ProfessorName { get; set; }
		public int TotalCoursesTaught { get; set; }
		public int TotalEvaluations { get; set; }
		public double AverageTeachingQuality { get; set; }
		public double AverageAssessmentFairness { get; set; }
		public double AverageOverallSatisfaction { get; set; }
		public List<CourseEvaluationSummaryResponse> CourseEvaluations { get; set; }
	}

	public class StudentEvaluationStatusResponse
	{
		public Guid SemesterId { get; set; }
		public string SemesterName { get; set; }
		public List<CourseEvaluationStatus> Courses { get; set; }
	}

	public class CourseEvaluationStatus
	{
		public Guid ClassSectionId { get; set; }
		public Guid CourseId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public Guid ProfessorId { get; set; }
		public string ProfessorName { get; set; }
		public bool HasCompleted { get; set; }
		public DateTime? CompletionDate { get; set; }
	}

	public class CourseEvaluationPeriodResponse
	{
		public Guid Id { get; set; }
		public Guid SemesterId { get; set; }
		public string SemesterName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool IsActive { get; set; }
		public bool IsCurrentlyOpen { get; set; }
	}
}
