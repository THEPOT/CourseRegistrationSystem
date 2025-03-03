namespace CourseRegistration_API.Payload.Response
{
	public class StudentProgramCourseResponse
	{
		public string Mssv { get; set; }
		public string StudentName { get; set; }
		public string ProgramName { get; set; }
		public List<CourseInfo> Courses { get; set; } = new List<CourseInfo>();
	}
}
