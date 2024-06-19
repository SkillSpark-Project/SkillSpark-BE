using Application.Interfaces;
using Application.Services;
using Application.ViewModels.LessonViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly IClaimsService _claimsService;

        public LessonController(ILessonService lessonService,IClaimsService claimsService)
        {
            _lessonService  = lessonService;
            _claimsService = claimsService;
        }

        [HttpPost]
        [Authorize(Roles ="Mentor")]
        public async Task<IActionResult> PostAsync([FromForm]LessonModel model)
        {
            try
            {
                var userId = _claimsService.GetCurrentUserId;
                await _lessonService.CreateLesson(model, userId.ToString().ToLower());
                return Ok("Tạo mới thành công.");
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
