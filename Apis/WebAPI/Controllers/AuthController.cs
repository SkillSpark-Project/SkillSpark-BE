using Application.Interfaces;
using Application.Services;
using Application.ViewModels;
using Application.ViewModels.AuthViewModel;
using Domain.Entities;
using Infrastructures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Text;
using WebAPI.Validations.Auth;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly IAuthService _auth;
        private readonly IConfiguration _configuration;
        public readonly IWebHostEnvironment _environment;

        public AuthController(UserManager<ApplicationUser> userManager,
             SignInManager<ApplicationUser> signInManager,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            IAuthService authService,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _auth = authService;
            _configuration = configuration;
            _environment = environment;
        }
        [HttpPost]
        [Route("/Login")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        return NotFound("Tài khoản này không tồn tại!");
                    }
                }
                //lấy host để redirect về send email
                var referer = Request.Headers["Referer"].ToString().Trim();
                var callbackUrl = await GetCallbackUrlAsync(user, referer, "EmailConfirm");

                var result = await _auth.Login(model.Email, model.Password, callbackUrl);
                if (result == null)
                {
                    return BadRequest("Đăng nhập không thành công!");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("/Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var _auth = new AuthService(_userManager, _signInManager, _configuration, _environment);
            var existEmailUser = await _userManager.FindByEmailAsync(model.Email);
            if (existEmailUser == null)
            {
                var existUsernameUser = await _userManager.FindByNameAsync(model.Username);
                if (existUsernameUser == null)
                {
                    try
                    {
                        var validator = new RegisterModelValidator();
                        var result = validator.Validate(model);
                        if (result.IsValid)
                        {
                            //kết thúc lấy host để redirect về và tạo link
                            var temp = await _auth.Register(model);
                            if (temp == null)
                            {
                                var user = await _userManager.FindByEmailAsync(model.Email);
                                string callbackUrl = "";
                                //lấy host để redirect về
                                var referer = Request.Headers["Referer"].ToString();
                                string schema;
                                string host;
                                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                                if (Uri.TryCreate(referer, UriKind.Absolute, out var uri))
                                {
                                    schema = uri.Scheme; // Lấy schema (http hoặc https) của frontend
                                    host = uri.Host; // Lấy host của frontend
                                    callbackUrl = schema + "://" + host + Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = code });
                                }
                                if (callbackUrl.Equals(""))
                                {
                                    callbackUrl = "https://localhost:5001/" + Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = code });
                                }

                               // await _auth.SendEmailConfirmAsync(model.Email.Trim(), callbackUrl);
                                return Ok("Đăng ký tài khoản WarehouseBridge thành công. Vui lòng kiểm tra email để kích hoạt tài khoản!");
                            }
                            else
                            {
                                return BadRequest(temp);
                            }
                        }
                        else
                        {
                            ErrorViewModel errors = new ErrorViewModel();
                            errors.Errors = new List<string>();
                            errors.Errors.AddRange(result.Errors.Select(x => x.ErrorMessage));
                            return BadRequest(errors);
                        }
                    }
                    catch (Exception ex)
                    {
                        return NotFound(ex.Message);
                    }
                }
                else
                {
                    return NotFound("Tên đăng nhập này đã được sử dụng!");
                }
            }
            else
            {
                return NotFound("Email này đã được sử dụng!");
            }
        }

        [HttpGet]
        [Route("/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string? code, string? userId)
        {
            if (userId == null || code == null)
            {
                return BadRequest("Xác nhận Email không thành công! Link xác nhận không chính xác ! Vui lòng sử dụng đúng link được gửi từ WarehouseBridge tới Email của bạn!");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("Xác nhận Email không thành công! Link xác nhận không chính xác! Vui lòng sử dụng đúng link được gửi từ WarehouseBridge tới Email của bạn!");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            string StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            if (result.Succeeded)
            {
                return Ok("Xác nhận Email thành công!Bây giờ bạn có thể đăng nhập vào tài khoản của mình bằng Email hoặc Username vừa xác thực !");
            }
            else
            {
                return BadRequest("Xác nhận Email không thành công! Link xác nhận không chính xác hoặc đã hết hạn! Vui lòng sử dụng đúng link được gửi từ WarehouseBridge tới Email của bạn!");

            }
        }

        [NonAction]
        public async Task<string> GetCallbackUrlAsync(ApplicationUser user, string referer, string type)
        {
           
            string callbackUrl = "";
            string schema;
            string host;
            var code = "";
            var action = "";
            switch (type)
            {
                case "EmailConfirm":
                    code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    action = "ConfirmEmail";
                    break;

                case "ResetPassword":
                    code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    action = "ResetPassword";
                    break;
            }
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            if (!referer.Equals("") && Uri.TryCreate(referer, UriKind.Absolute, out var uri))
            {
                schema = uri.Scheme; // Lấy schema (http hoặc https) của frontend
                host = uri.Host; // Lấy host của frontend
                callbackUrl = schema + "://" + host + Url.Action(action, "Auth", new { userId = user.Id, code = code });
            }
            if (referer.Equals("https://localhost:5001/swagger/index.html"))
            {
                callbackUrl = "https://localhost:5001" + Url.Action(action, "Auth", new { userId = user.Id, code = code });
            }
            else if (referer.Contains("http://localhost:5173"))
            {
                callbackUrl = "http://localhost:5173" + Url.Action(action, "Auth", new { userId = user.Id, code = code });
            }
            return callbackUrl;
        }
    }
}
