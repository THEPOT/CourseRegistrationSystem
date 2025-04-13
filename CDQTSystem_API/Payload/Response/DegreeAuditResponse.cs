namespace CDQTSystem_API.Payload.Response
{
	public class DegreeAuditResponse
	{
		public Guid StudentId { get; set; }
		public string Mssv { get; set; }
		public string StudentName { get; set; }
		public string MajorName { get; set; }
		public int RequiredCredits { get; set; }
		public int CompletedCredits { get; set; }
		public int RemainingCredits { get; set; }
		public double CompletionPercentage { get; set; }
		public List<CourseAuditInfo> RequiredCourses { get; set; } = new List<CourseAuditInfo>();
		public List<CourseAuditInfo> CompletedCourses { get; set; } = new List<CourseAuditInfo>();
		public List<CourseAuditInfo> InProgressCourses { get; set; } = new List<CourseAuditInfo>();
		public List<CourseAuditInfo> RemainingCourses { get; set; } = new List<CourseAuditInfo>();
		public double GPA { get; set; }
		public bool EligibleForGraduation { get; set; }
		public string AdditionalRequirements { get; set; }
		public List<string> Notes { get; set; } = new List<string>();
	}

	public class CourseAuditInfo
	{
		public Guid CourseId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public int Credits { get; set; }
		public string Grade { get; set; }
		public decimal? QualityPoints { get; set; }
		public string Semester { get; set; }
		public bool IsRequired { get; set; }
	}
}
