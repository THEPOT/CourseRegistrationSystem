using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDQTSystem_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CourseOfferingsController : BaseController<CourseOfferingsController>
	{
        private readonly ICourseOfferingService _courseOfferingService;
        private readonly ICourseRegistrationService _courseRegistrationService;

		public CourseOfferingsController(ILogger<CourseOfferingsController> logger, ICourseOfferingService courseOfferingService, ICourseRegistrationService courseRegistrationService ) : base(logger)
		{
            _courseOfferingService = courseOfferingService;
			_courseRegistrationService = courseRegistrationService;
		}

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<List<CourseOfferingResponse>>> CreateCourseOfferings(
            [FromBody] CourseOfferingCreateRequest request)
        {
            try
            {
                var offerings = await _courseOfferingService.CreateCourseOfferings(request);
                return Ok(offerings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("semester/{semesterId}")]
        [Authorize(Roles = "Staff,Student,Professor")]
        public async Task<ActionResult<List<CourseOfferingResponse>>> GetOfferingsBySemester(Guid semesterId)
        {
            try
            {
                var offerings = await _courseOfferingService.GetOfferingsBySemester(semesterId);
                return Ok(offerings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{offeringId}")]
        [Authorize(Roles = "Staff,Student,Professor")]
        public async Task<ActionResult<CourseOfferingResponse>> GetOfferingById(Guid offeringId)
        {
            try
            {
                var offering = await _courseOfferingService.GetOfferingById(offeringId);
                if (offering == null)
                    return NotFound();
                    
                return Ok(offering);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{offeringId}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<CourseOfferingResponse>> UpdateOffering(
            Guid offeringId, 
            [FromBody] CourseOfferingUpdateRequest request)
        {
            try
            {
                var offering = await _courseOfferingService.UpdateOffering(offeringId, request);
                if (offering == null)
                    return NotFound();
                    
                return Ok(offering);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{offeringId}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> DeleteOffering(Guid offeringId)
        {
            try
            {
                var result = await _courseOfferingService.DeleteOffering(offeringId);
                if (!result)
                    return NotFound();
                    
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("professor/{semesterId}")]
        [Authorize(Roles = "Professor")]
        public async Task<ActionResult<List<CourseOfferingResponse>>> GetProfessorOfferingsBySemester(Guid semesterId)
        {
			var userId = Guid.Parse(User.FindFirst("UserId")?.Value);
			var offerings = await _courseRegistrationService.GetProfessorOfferingsBySemester(userId, semesterId);
            return Ok(offerings);
        }

        [HttpGet("{offeringId}/students")]
        [Authorize(Roles = "Professor")]
        public async Task<ActionResult<List<StudentInfoResponse>>> GetStudentsInOffering(Guid offeringId)
        {
            var students = await _courseRegistrationService.GetStudentsInOffering(offeringId);
            return Ok(students);
        }
    }
}

