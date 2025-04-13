using CDQTSystem_API.Payload.Request;
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
		public async Task<ActionResult<List<CourseDetailsResponse>>> GetAllCourses()
		{
			var courses = await _courseService.GetAllCourses();
			return Ok(courses);
		}

		[HttpGet("code/{courseCode}")]
		[Authorize]
		public async Task<ActionResult<CourseDetailsResponse>> GetCourseByCode(string courseCode)
		{
			var course = await _courseService.GetCourseByCode(courseCode);
			if (course == null)
				return NotFound($"Course with code '{courseCode}' not found");
			return Ok(course);
		}

		[HttpGet("{courseId:guid}")]
		[Authorize]
		public async Task<ActionResult<CourseDetailsResponse>> GetCourseById(Guid courseId)
		{
			var course = await _courseService.GetCourseById(courseId);
			if (course == null)
				return NotFound($"Course with ID '{courseId}' not found");
			return Ok(course);
		}

		[HttpGet("search")]
		[Authorize]
		public async Task<ActionResult<List<CourseDetailsResponse>>> SearchCourses([FromQuery] string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
				return BadRequest("Search keyword cannot be empty");
				
			var courses = await _courseService.SearchCourses(keyword);
			return Ok(courses);
		}

		[HttpPost]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<CourseDetailsResponse>> CreateCourse([FromBody] CourseCreateRequest request)
		{
			try
			{
				var course = await _courseService.CreateCourse(request);
				return CreatedAtAction(nameof(GetCourseById), new { courseId = course.Id }, course);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{courseId:guid}")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<CourseDetailsResponse>> UpdateCourse(Guid courseId, [FromBody] CourseUpdateRequest request)
		{
			try
			{
				var course = await _courseService.UpdateCourse(courseId, request);
				if (course == null)
					return NotFound($"Course with ID '{courseId}' not found");
				return Ok(course);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{courseId:guid}")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult> DeleteCourse(Guid courseId)
		{
			var result = await _courseService.DeleteCourse(courseId);
			if (!result)
				return NotFound($"Course with ID '{courseId}' not found");
			return NoContent();
		}

		[HttpGet("{courseId:guid}/syllabus/latest")]
		[Authorize]
		public async Task<ActionResult<CourseSyllabusResponse>> GetLatestSyllabus(Guid courseId)
		{
			var syllabus = await _courseService.GetLatestSyllabus(courseId);
			if (syllabus == null)
				return NotFound($"No syllabus found for course with ID '{courseId}'");
			return Ok(syllabus);
		}

		[HttpGet("{courseId:guid}/syllabus/versions")]
		[Authorize]
		public async Task<ActionResult<List<CourseSyllabusResponse>>> GetAllSyllabusVersions(Guid courseId)
		{
			var syllabusVersions = await _courseService.GetAllSyllabusVersions(courseId);
			return Ok(syllabusVersions);
		}

		[HttpPost("{courseId:guid}/syllabus")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<CourseSyllabusResponse>> CreateSyllabus(Guid courseId, [FromBody] SyllabusCreateRequest request)
		{
			try
			{
				var syllabus = await _courseService.CreateSyllabus(courseId, request);
				return CreatedAtAction(nameof(GetLatestSyllabus), new { courseId }, syllabus);
			}
			catch (BadHttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
	
}
