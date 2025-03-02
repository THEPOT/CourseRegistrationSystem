using CourseRegistration_API.Enums;

namespace CourseRegistration_API.Payload.Response
{
	public class RegisterResponse
	{
		public Guid Id { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public string ImageUrl { get; set; }
		public RoleEnum Role { get; set; }
	}
}
