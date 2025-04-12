using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDQTSystem_API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class CourseController : ControllerBase
	{
		private readonly ICourseService _courseService;
		public CourseController(ICourseService courseService)
		{
			_courseService = courseService;
		}
		[HttpGet]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<List<CourseBasicInfoResponse>>> GetAllCourses()
		{
			var courses = await _courseService.GetAllCourses();
			return Ok(courses);
		}

	}
	
}
