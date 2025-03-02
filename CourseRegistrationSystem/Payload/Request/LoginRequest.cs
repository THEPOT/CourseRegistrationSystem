using System.ComponentModel.DataAnnotations;

namespace CourseRegistration_API.Payload.Request
{
	public class LoginRequest
	{
		[Required(ErrorMessage = "Email is required")]
		[MaxLength(64, ErrorMessage = "Email's max length is 64 characters")]
		[EmailAddress(ErrorMessage = "Email is not valid")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Password is required")]
		[MaxLength(64, ErrorMessage = "Password's max length is 64 characters")]
		public string Password { get; set; }
	}
}
