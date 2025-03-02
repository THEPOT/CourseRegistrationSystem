using Microsoft.AspNetCore.Authorization;

namespace CourseRegistration_API.Authorization
{
	public class HeaderRequirement : IAuthorizationRequirement
	{
		public string HeaderName { get; }
		public string HeaderValue { get; }

		public HeaderRequirement(string headerName, string headerValue = null)
		{
			HeaderName = headerName;
			HeaderValue = headerValue;
		}
	}
}
