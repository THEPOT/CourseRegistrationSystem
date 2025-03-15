namespace CourseRegistration_API.Payload.Request
{
	public class CourseCreateRequest
	{
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public int Credits { get; set; }
		public string Description { get; set; }
		public string LearningOutcomes { get; set; }
		public Guid DepartmentId { get; set; }
		public List<Guid> PrerequisiteCourseIds { get; set; } = new List<Guid>();
		public List<Guid> CorequisiteCourseIds { get; set; } = new List<Guid>();
	}

	public class CourseUpdateRequest
	{
		public string CourseName { get; set; }
		public int Credits { get; set; }
		public string Description { get; set; }
		public string LearningOutcomes { get; set; }
		public Guid DepartmentId { get; set; }
		public List<Guid> PrerequisiteCourseIds { get; set; } = new List<Guid>();
		public List<Guid> CorequisiteCourseIds { get; set; } = new List<Guid>();
	}

	public class SyllabusCreateRequest
	{
		public string SyllabusContent { get; set; }
		public string Version { get; set; }
	}
}
