namespace CourseRegistration_API.Payload.Request
{
	public class StudentProgramCoursesCreateRequest
	{
		public Guid ProgramId { get; set; }
		public List<Guid> SelectedCourseIds { get; set; } = new List<Guid>(); // Optional course selection
		public string EnrollmentSemester { get; set; } // Term name for course registration
	}
}
