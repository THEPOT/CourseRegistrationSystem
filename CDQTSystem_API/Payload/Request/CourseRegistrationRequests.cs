namespace CDQTSystem_API.Payload.Request
{
	public class CourseRegistrationRequest
	{
		public Guid StudentId { get; set; }
		public Guid CourseOfferingId { get; set; }
	}

	public class BatchCourseRegistrationRequest
	{
		public Guid StudentId { get; set; }
		public List<Guid> CourseOfferingIds { get; set; } = new List<Guid>();
	}

	public class CourseRegistrationUpdateRequest
	{
		public string Status { get; set; } // "Registered", "Waitlisted", "Dropped"
	}
}
