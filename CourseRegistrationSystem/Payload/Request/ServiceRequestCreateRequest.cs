namespace CourseRegistration_API.Payload.Request
{
	public class ServiceRequestCreateRequest
	{
		public Guid StudentId { get; set; }
		public string ServiceType { get; set; } // Certificate, Transcript, LeaveOfAbsence, etc.
		public string Details { get; set; }
	}

	public class ServiceRequestStatusUpdateRequest
	{
		public string Status { get; set; } // Pending, Approved, Denied
		public string Comments { get; set; } // Optional staff comments
	}
}
