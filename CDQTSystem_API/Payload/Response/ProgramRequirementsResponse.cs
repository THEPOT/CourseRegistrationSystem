namespace CDQTSystem_API.Payload.Response
{
    public class ProgramRequirementsResponse
    {
        public Guid ProgramId { get; set; }
		public string MajorName { get; set; }
		public int RequiredCredits { get; set; }
		public List<RequiredCoursesResponse> Requirements { get; set; }
	}
	public class RequiredCoursesResponse
	{
		public Guid CourseId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public int Credits { get; set; }
	}
} 