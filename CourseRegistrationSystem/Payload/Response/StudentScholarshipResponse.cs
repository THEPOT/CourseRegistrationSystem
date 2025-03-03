namespace CourseRegistration_API.Payload.Response
{
	public class StudentScholarshipResponse
	{
		public string Mssv { get; set; }
		public string FullName { get; set; }
		public string ScholarshipName { get; set; }
		public decimal Amount { get; set; }
		public DateOnly AwardDate { get; set; }
	}
}
