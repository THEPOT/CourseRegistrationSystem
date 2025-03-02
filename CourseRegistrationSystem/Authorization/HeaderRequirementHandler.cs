using Microsoft.AspNetCore.Authorization;

namespace CourseRegistration_API.Authorization
{
	public class HeaderRequirementHandler : AuthorizationHandler<HeaderRequirement>
	{
		private readonly ILogger<HeaderRequirementHandler> _logger;

		public HeaderRequirementHandler(ILogger<HeaderRequirementHandler> logger)
		{
			_logger = logger;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HeaderRequirement requirement)
		{
			var httpContext = context.Resource as HttpContext;
			if (httpContext != null && httpContext.Request.Headers.TryGetValue(requirement.HeaderName, out var value))
			{
				if (string.IsNullOrEmpty(requirement.HeaderValue) || value == requirement.HeaderValue)
				{
					_logger.LogInformation($"HeaderRequirementHandler: Successfully validated header {requirement.HeaderName}.");
					context.Succeed(requirement);
				}
				else
				{
					_logger.LogWarning($"HeaderRequirementHandler: Header {requirement.HeaderName} value {value} did not match expected {requirement.HeaderValue}.");
				}
			}
			else
			{
				_logger.LogWarning($"HeaderRequirementHandler: Header {requirement.HeaderName} not found.");
			}
			return Task.CompletedTask;
		}
	}
}
