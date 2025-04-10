namespace CourseRegistration_API.Payload.Response
{
	public class CourseOfferingResponse
	{
		public Guid CourseOfferingId { get; set; }
		public Guid CourseId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public int Credits { get; set; }
		public Guid? ProfessorId { get; set; }
		public string LecturerName { get; set; }
		public string Classroom { get; set; }
		public string Schedule { get; set; }
		public int Capacity { get; set; }
		public int RegisteredCount { get; set; }
		public int AvailableSlots { get; set; }
		public string PrerequisiteStatus { get; set; }
		public List<string> ConflictingCourses { get; set; }
		public string SemesterName { get; set; }
		public DateOnly StartDate { get; set; }
		public DateOnly EndDate { get; set; }
	}

	public class CourseRegistrationSummaryResponse
	{
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public int RegisteredStudents { get; set; }
		public int Capacity { get; set; }
		public double FillPercentage { get; set; }
	}
}
