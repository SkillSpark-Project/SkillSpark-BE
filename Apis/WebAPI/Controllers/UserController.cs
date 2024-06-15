using Application.Commons.Exeptions;
using Application.Interfaces;
using Application.Services;
using Application.ViewModels.MentorViewModels.Requests;
using Application.ViewModels.UserViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IClaimsService _claimsService;

        public UserController(IUserService userService, IClaimsService claimsService)
        {
            _userService = userService;
            _claimsService = claimsService;
        }

        [HttpPost]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> CreateMentor([FromForm] MentorModel model)
        {
            try
            {
                var userId = _claimsService.GetCurrentUserId;
                var result = await _userService.CreateMentorAccount(model, userId.ToString().ToLower());
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetList(int indexPage = 0, int pageSize = 20)
        {
            try
            {
                var list = await _userService.GetListUserAsync(indexPage, pageSize);
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

        [HttpGet("id")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetList(string id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                return Ok(user);
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

        [Authorize]
        [HttpPost("Profile")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassModel model)
        {
            string userId = _claimsService.GetCurrentUserId.ToString().ToLower();
            try
            {
                await _userService.ChangePasswordAsync(model, userId);
                return Ok("Đổi mật khẩu thành công");
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

        [Authorize]
        [HttpPut("Profile")]
        public async Task<IActionResult> ChangeProfile([FromForm] UserRequestModel model)
        {
            string userId = _claimsService.GetCurrentUserId.ToString().ToLower();
            try
            {
                var result = await _userService.UpdateUserAsync(model, userId);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    status = BadRequest().StatusCode,
                    title = "Đã xảy ra 1 hoặc vài lỗi xác thực.",
                    validateError = ex.Errors
                });
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