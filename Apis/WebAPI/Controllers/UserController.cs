using Application.Interfaces;
using Application.Services;
using Application.ViewModels.MentorViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult > CreateMentor([FromForm] MentorModel model)
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
    }
}
