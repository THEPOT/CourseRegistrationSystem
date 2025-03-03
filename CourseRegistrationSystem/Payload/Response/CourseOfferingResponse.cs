namespace CourseRegistration_API.Payload.Response
{
	public class CourseOfferingResponse
	{
		public Guid CourseOfferingId { get; set; }
		public Guid CourseId { get; set; }
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public int Credits { get; set; }
		public Guid LecturerId { get; set; }
		public string LecturerName { get; set; }
		public string Classroom { get; set; }
		public string Schedule { get; set; }
		public int Capacity { get; set; }
		public int RegisteredCount { get; set; }
		public string PrerequisiteStatus { get; set; } // "Satisfied", "Missing", "Not Required"
		public List<string> ConflictingCourses { get; set; } = new List<string>(); // For schedule conflicts
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
