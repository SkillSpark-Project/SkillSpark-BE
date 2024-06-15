using Application.Interfaces;
using Application.Services;
using Application.ViewModels.CourseViewModels.Requests;
using Application.ViewModels.CourseViewModels.Responses;
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

        [HttpPost("Filter")]
        public async Task<IActionResult> GetAsync([FromForm]FilterModel model)
        {
            try
            {
                var list = await _courseService.GetList(model);
                return Ok(list);
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

        [HttpGet("Id")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            try
            {
                var result = await _courseService.GetById(Id);
                return Ok(result);
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
