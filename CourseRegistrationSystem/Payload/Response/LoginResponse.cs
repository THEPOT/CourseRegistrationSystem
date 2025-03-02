using CourseRegistration_API.Enums;

namespace CourseRegistration_API.Payload.Response
{
	public class LoginResponse
	{
		public string Name { get; set; }
		public string Role { get; set; }

		public string ImgUrl { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }

		public LoginResponse(string fullName, string roleName, string? image)
		{
			Name = fullName;
			Role = roleName;
			ImgUrl = image;
		}
	}
}
