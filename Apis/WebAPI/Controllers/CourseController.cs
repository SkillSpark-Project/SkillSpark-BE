using Application.Interfaces;
using Application.Services;
using Application.ViewModels.CourseViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IClaimsService _claimsService;

        public CourseController(ICourseService courseService, IClaimsService claimsService)
        {
            _courseService = courseService;
            _claimsService = claimsService;
        }
        [HttpPost]
        [Authorize(Roles ="Mentor")]
        public async Task<IActionResult> PostAsync([FromForm] CourseModel model)
        {
            try
            {
                var userId = _claimsService.GetCurrentUserId;
                await _courseService.AddCourse(model, userId.ToString().ToLower());
                return Ok("Tạo khóa học thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = BadRequest().StatusCode,
                    title = ex.Message
                });
            }
        }
    }
}
