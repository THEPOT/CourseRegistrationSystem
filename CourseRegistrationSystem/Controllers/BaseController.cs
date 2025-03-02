using Microsoft.AspNetCore.Mvc;
using CourseRegistration_API.Constants;
namespace CourseRegistration_API.Controllers
{
	[Route(ApiEndPointConstant.ApiEndpoint)]
	[ApiController]
	public class BaseController<T> : ControllerBase where T : BaseController<T>
	{
		protected ILogger<T> _logger;

		public BaseController(ILogger<T> logger)
		{
			_logger = logger;
		}
	}
}
