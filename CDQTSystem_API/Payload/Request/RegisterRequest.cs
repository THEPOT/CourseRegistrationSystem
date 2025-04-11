using CDQTSystem_API.Enums;

namespace CDQTSystem_API.Payload.Request
{
	public class RegisterRequest
	{
		public string FullName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }

		public Guid MajorId { get; set; }

		public string ImageUrl { get; set; }
		public RoleEnum Role { get; set; }

	}
}
