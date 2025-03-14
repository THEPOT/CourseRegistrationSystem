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
			public const string RefreshToken = AuthenticationEndpoint + "/refresh-token";
		}
		public static class Students
		{
			public const string StudentsEndpoint = ApiEndpoint + "/students";
			public const string GetById = StudentsEndpoint + "/{id}";
			public const string GetAll = StudentsEndpoint;
			public const string Create = StudentsEndpoint;
			public const string FinancialInfo = StudentsEndpoint + "/{id}/financial-info";
			public const string StudentProgram = StudentsEndpoint + "/{id}/program";
			public const string StudentTranscript = StudentsEndpoint + "/{id}/transcript";
			public const string StudentTermGPA = StudentsEndpoint + "/{id}/term/{termId}/gpa";
			public const string FailedCourses = StudentsEndpoint + "/{id}/failed-courses";
		}
	}
}
