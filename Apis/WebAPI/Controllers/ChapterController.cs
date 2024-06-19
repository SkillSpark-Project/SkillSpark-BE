using Application.Interfaces;
using Application.ViewModels.CategoryViewModels.Requests;
using Application.ViewModels.ChapterViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;
        private readonly IClaimsService _claimsService;

        public ChapterController(IChapterService chapterService, IClaimsService claimsService)
        {
            _chapterService = chapterService;
            _claimsService = claimsService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _chapterService.GetChapters();
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
        
        [HttpPost]
        [Authorize(Roles = "Mentor")]
        public async Task<IActionResult> Post([FromBody] ChapterModel model)
        {
            try
            {
                var userId = _claimsService.GetCurrentUserId;
                await _chapterService.AddChapter(model, userId.ToString().ToLower());
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = BadRequest().StatusCode,
                    title = ex.Message
                });
            }
            return Ok("Tạo mới thành công");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Mentor")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] ChapterModel model)
        {
            try
            {
                var userId = _claimsService.GetCurrentUserId;

                await _chapterService.UpdateChapter(id, model, userId.ToString().ToLower());
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = BadRequest().StatusCode,
                    title = ex.Message
                });
            }
            return Ok("Cập nhật thành công");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Mentor")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var userId = _claimsService.GetCurrentUserId;
                await _chapterService.DeleteChapter(id, userId.ToString().ToLower());
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = BadRequest().StatusCode,
                    title = ex.Message
                });
            }
            return Ok("Xóa thành công");
        }

    }
}
