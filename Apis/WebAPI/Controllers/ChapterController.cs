using Application.Interfaces;
using Application.ViewModels.CategoryViewModels.Requests;
using Application.ViewModels.ChapterViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;

        public ChapterController(IChapterService chapterService)
        {
            _chapterService = chapterService;
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
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> Post([FromBody] ChapterModel model)
        {
            try
            {
                await _chapterService.AddChapter(model);
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
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] ChapterModel model)
        {
            try
            {
                await _chapterService.UpdateChapter(id, model);
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
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                await _chapterService.DeleteChapter(id);
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
