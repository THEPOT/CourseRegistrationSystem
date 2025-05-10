namespace CDQTSystem_API.Payload.Request
{
    public class ProgramRequirementsUpdateRequest
	{
		public Guid ProgramId { get; set; }
		public string MajorName { get; set; }
		public int RequiredCredits { get; set; }
		public List<RequiredCourses> Requirements { get; set; }
	}
	public class RequiredCourses
	{
		public Guid CourseId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public int Credits { get; set; }
	}
} 