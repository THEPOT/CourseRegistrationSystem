namespace CDQTSystem_API.Payload.Response
{
	public class CourseDetailsResponse
	{
		public Guid Id { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public int Credits { get; set; }
		public string Description { get; set; }
		public string LearningOutcomes { get; set; }
		public Guid DepartmentId { get; set; }
		public string DepartmentName { get; set; }
		public List<CourseBasicInfo> Prerequisites { get; set; } = new List<CourseBasicInfo>();
		public List<CourseBasicInfo> Corequisites { get; set; } = new List<CourseBasicInfo>();
	}

	public class CourseBasicInfo
	{
		public Guid Id { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
	}

	public class CourseSyllabusResponse
	{
		public Guid Id { get; set; }
		public Guid CourseId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public string SyllabusContent { get; set; }
		public string Version { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
	}
}
