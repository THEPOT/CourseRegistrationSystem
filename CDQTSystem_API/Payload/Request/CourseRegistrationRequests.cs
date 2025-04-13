namespace CDQTSystem_API.Payload.Request
{

	public class BatchCourseRegistrationRequest
	{
		public Guid CourseOfferingId { get; set; } 
	}

	public class CourseRegistrationUpdateRequest
	{
		public string Status { get; set; } // "Registered", "Waitlisted", "Dropped"
	}
}
