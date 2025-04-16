using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDQTSystem_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CourseOfferingsController : BaseController<CourseOfferingsController>
	{
        private readonly ICourseOfferingService _courseOfferingService;

        public CourseOfferingsController(ILogger<CourseOfferingsController> logger, ICourseOfferingService courseOfferingService) : base(logger)
		{
            _courseOfferingService = courseOfferingService;
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
    }
}

