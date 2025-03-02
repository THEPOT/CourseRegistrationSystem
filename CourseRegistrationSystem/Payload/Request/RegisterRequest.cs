using CourseRegistration_API.Enums;

namespace CourseRegistration_API.Payload.Request
{
	public class RegisterRequest
	{
		public string FullName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }

		public Guid ProgramId { get; set; }

		public string ImageUrl { get; set; }
		public RoleEnum Role { get; set; }

	}
}
