namespace CDQTSystem_API.Payload.Request
{
	public class DegreeAuditUpdateRequest
	{
		public List<string> NotesToAdd { get; set; } = new List<string>();
		public string AdditionalRequirements { get; set; }
		public bool EligibleForGraduation { get; set; }
		public List<CourseAdjustment> CourseAdjustments { get; set; } = new List<CourseAdjustment>();
	}

	public class CourseAdjustment
	{
		public Guid CourseId { get; set; }
		public bool IsRequired { get; set; }
		public bool IsWaived { get; set; }
	}
}
