namespace CourseRegistration_API.Payload.Response
{
	public class ServiceRequestResponse
	{
		public Guid Id { get; set; }
		public Guid StudentId { get; set; }
		public string StudentName { get; set; }
		public string Mssv { get; set; }
		public string ServiceType { get; set; }
		public DateTime RequestDate { get; set; }
		public string Status { get; set; }
		public string Details { get; set; }
		public string StaffComments { get; set; }
	}
}
