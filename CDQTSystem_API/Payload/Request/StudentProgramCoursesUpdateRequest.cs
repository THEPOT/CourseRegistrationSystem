namespace CDQTSystem_API.Payload.Request
{
	public class StudentProgramCoursesUpdateRequest
	{
		public Guid ProgramId { get; set; }
		public List<Guid> CoursesToAdd { get; set; } = new List<Guid>();
		public List<Guid> CoursesToRemove { get; set; } = new List<Guid>();
		public string EnrollmentSemester { get; set; } // Term name for course registration
		public bool ReplaceAllCourses { get; set; } = false; // If true, remove all current courses first
	}
}
