namespace CourseRegistration_API.Constants
{
	public static class ApiEndPointConstant
	{
		static ApiEndPointConstant()
		{
		}

		public const string RootEndPoint = "/api";
		public const string ApiVersion = "/v1";
		public const string ApiEndpoint = RootEndPoint + ApiVersion;
		public static class Authentication
		{
			public const string AuthenticationEndpoint = ApiEndpoint + "/auth";
			public const string Login = AuthenticationEndpoint + "/login";
			public const string Register = AuthenticationEndpoint + "/register";
		}

	}
}
