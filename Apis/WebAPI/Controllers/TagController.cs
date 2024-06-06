using Application.Interfaces;
using Application.ViewModels.CategoryViewModels.Requests;
using Application.ViewModels.TagViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var tag = await _tagService.GetTags();

                return Ok(tag);
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                var tag = await _tagService.GetById(id);
                if (tag == null)
                {
                    return NotFound(new
                    {
                        status = NotFound().StatusCode,
                        title = "Không tìm thấy"
                    });
                }
                else
                {
                    return Ok(tag);
                }
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
        public async Task<IActionResult> Post([FromBody] TagModel model)
        {
            try
            {
                await _tagService.Add(model);
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
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] TagModel model)
        {
            try
            {
                await _tagService.Update(id, model);
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
                await _tagService.Delete(id);
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
