using Application.Commons;
using Application.Interfaces;
using Application.Validations.Auths;
using Application.ViewModels;
using Application.ViewModels.AuthViewModel;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Application.Services
{
    public class AuthService :IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        public readonly IWebHostEnvironment _environment;

        public AuthService
            (UserManager<ApplicationUser> userManager,
             SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _environment = environment;
        }
        public async Task<LoginViewModel> Login(string email, string pass, string callbackUrl)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null )
                {
                    throw new KeyNotFoundException($"Không tìm thấy tên đăng nhập hoặc địa chỉ email '{email}'");
                }
            }
            if (user.EmailConfirmed == false)
            {
                var result = await SendEmailAsync(user, callbackUrl, "EmailConfirm");
                throw new Exception("Tài khoản này chưa xác thực Email. Vui lòng kiểm tra Email được vừa gửi đến hoặc liên hệ quản trị viên để được hỗ trợ!");
            }
            else
            {
                var result = await AuthenticateAsync(email.Trim(), pass.Trim());

                if (result != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var userModel = new LoginViewModel();
                    userModel.Id = user.Id;
                    userModel.Email = user.Email;
                    userModel.FullName = user.Fullname;
                    userModel.Username = user.UserName;
                    userModel.Avatar = user.Avatar;
                    userModel.listRoles = roles.ToList();
                    userModel.Token = result;
                    return userModel;
                }

                throw new AuthenticationException("Đăng nhập không thành công!");
            }
        }
        public async Task<bool> SendEmailAsync(ApplicationUser user, string callbackUrl, string type)
        {
            var isLock = await _userManager.IsLockedOutAsync(user);
            if (isLock)
            {
                throw new KeyNotFoundException($"Tài khoản này hiện tại đang bị khóa. Vui lòng liên hệ quản trị viên để được hỗ trợ");
            }
            MailService mail = new MailService();
            var temp = false;
            switch (type)
            {
                case "EmailConfirm":
                    temp = mail.SendEmail(user.Email, "Xác nhận tài khoản từ Thanh Sơn Garden",
            $"<h2 style=\" color: #00B214;\">Xác thực tài khoản từ Thanh Sơn Garden</h2>\r\n<p style=\"margin-bottom: 10px;\r\n    text-align: left;\">Xin chào <strong>{user.Fullname}</strong>"
            + ",</p>\r\n<p style=\"margin-bottom: 10px;\r\n    text-align: left;\"> Cảm ơn bạn đã đăng ký tài khoản tại Thanh Sơn Garden." +
            " Để có được trải nghiệm dịch vụ và được hỗ trợ tốt nhất, bạn cần hoàn thiện xác thực tài khoản.</p>"
            + $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}' style=\"display: inline-block; background-color: #00B214;  color: #fff;" +
            $"    padding: 10px 20px;\r\n    border: none;\r\n    border-radius: 5px;\r\n    cursor: pointer;\r\n    text-decoration: none;\">Xác thực ngay</a>"
            );
                    break;
                case "ResetPassword":
                    string body = "<h3 style=\" color: #00B214;\">Xác nhận yêu cầu đổi mật khẩu truy cập vào Thạch Sơn Garden</h3>\r\n" + $"<p style=\"margin-bottom: 10px;\r\n    text-align: left;\">Xin chào <strong>{user.Fullname}</strong>,</p>\r\n<p style=\"margin-bottom: 10px;\r\n   " +
                    " text-align: left;\"> Bạn đã yêu cầu đổi mật khẩu. Vui lòng nhấp vào liên kết bên dưới để xác nhận yêu cầu. Vui lòng\r\n  lưu ý rằng đường dẫn xác nhận chỉ có hiệu lực trong vòng 30 phút. Sau thời gian đó, đường đãn sẽ hết hiệu lực và bạn\r\n" +
                    "  sẽ cần yêu cầu xác nhận lại. Nếu bạn không có bất kỳ yêu cầu thay đổi nào vui lòng không nhấn bất kỳ đường dẫn nào. Cảm ơn\r\n</p>" + $"<a href=\"{callbackUrl}\" style=\"display: inline-block;\r\n   " +
                    " background-color: #00B214;\r\n    color: #fff;\r\n    padding: 10px 20px;\r\n    border: none;\r\n    border-radius: 5px;\r\n    cursor: pointer;\r\n    text-decoration: none;\">Xác thực ngay</a>";
                    temp = mail.SendEmail(user.Email, "Xác thực yêu cầu đổi mật khẩu tài khoản từ Thanh Sơn Garden", body);
                    break;
                case "ResetPasswordForMobile":
                    string body1 = "<h3 style=\" color: #00B214;\">Mã xác thực yêu cầu đổi mật khẩu truy cập vào Thạch Sơn Garden</h3>\r\n" + $"<p style=\"margin-bottom: 10px;\r\n    text-align: left;\">Xin chào <strong>{user.Fullname}</strong>,</p>\r\n<p style=\"margin-bottom: 10px;\r\n   " +
                    " text-align: left;\"> Bạn đã yêu cầu đổi mật khẩu. Vui lòng sử dụng OTP bên dưới để thực hiện xác thực tài khoản. Vui lòng\r\n  lưu ý rằng mã OTP chỉ có hiệu lực trong vòng 30 phút. Sau thời gian đó, đường đãn sẽ hết hiệu lực và bạn\r\n" +
                    "  sẽ cần yêu cầu xác nhận lại. Nếu bạn không có bất kỳ yêu cầu thay đổi nào vui lòng không nhấn bất kỳ đường dẫn nào. Cảm ơn\r\n</p>" + $"Mã xác thực: " + callbackUrl;
                    temp = mail.SendEmail(user.Email, "Mã xác thực yêu cầu đổi mật khẩu tài khoản từ Thanh Sơn Garden", body1);
                    break;
            }
            var result = (temp) ? true : false;
            return result;
        }
        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(username);
                if (user == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy tên đăng nhập hoặc địa chỉ email '{username}'");
                }
            }
            if (user.LockoutEnd != null && user.LockoutEnd.Value > DateTime.Now)
            {
                throw new KeyNotFoundException($"Tài khoản này hiện tại đang bị khóa. Vui lòng liên hệ quản trị viên để được hỗ trợ!");
            }
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            //sign in  
            var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (signInResult.Succeeded)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Manager");
                var isMentor = await _userManager.IsInRoleAsync(user, "Mentor");
                var isLearner = await _userManager.IsInRoleAsync(user, "Learner");
                var roles = await _userManager.GetRolesAsync(user);
                List<Claim> authClaims = new List<Claim>();
                authClaims.Add(new Claim(ClaimTypes.Email, user.Email));
                authClaims.Add(new Claim("userId", user.Id));
                authClaims.Add(new Claim(ClaimTypes.Name, user.UserName));
                authClaims.Add(new Claim("isAdmin", isAdmin.ToString()));
                authClaims.Add(new Claim("isMentor", isMentor.ToString()));
                authClaims.Add(new Claim("isLearner", isLearner.ToString()));
                authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                foreach (var item in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, item));
                }

                var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecrectKey"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddDays(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                    );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            throw new InvalidOperationException("Sai mật khẩu. Vui lòng đăng nhập lại!");
        }
        public async Task<ErrorViewModel> Register(RegisterModel model)
        {
            var resultData = await CreateUserAsync(model);
            if (resultData.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var addRoleResult = await _userManager.AddToRoleAsync(user, "Customer");
                if (addRoleResult.Succeeded)
                {
                    return null;
                }
                else
                {
                    await _userManager.DeleteAsync(user);
                    throw new Exception("Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại!");
                }
            }
            else
            {
                ErrorViewModel errors = new ErrorViewModel();
                errors.Errors = new List<string>();
                errors.Errors.AddRange(resultData.Errors.Select(x => x.Description));
                return errors;
            }
        }
        public async Task<IdentityResult> CreateUserAsync(RegisterModel model)

        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Fullname = model.Fullname,
                Birthday = model.Birthday,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            return result;
        }

        public async Task CheckAccountExist(RegisterModel model)
        {
            var existEmailUser = await _userManager.FindByEmailAsync(model.Email);
            if (existEmailUser != null)
            {
                throw new Exception("Email này đã được sử dụng!");
            }
            var existUsernameUser = await _userManager.FindByNameAsync(model.Username);
            if (existUsernameUser != null)
            {
                throw new Exception("Tên đăng nhập này đã được sử dụng!");
            }
            return;
        }
        public async Task ConfirmEmailAsync(string? code, string? userId)
        {
            if (userId == null || code == null)
            {
                throw new Exception("Xác nhận Email không thành công! Link xác nhận không chính xác ! Vui lòng sử dụng đúng link được gửi từ Thanh Sơn Garden tới Email của bạn!");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Xác nhận Email không thành công! Link xác nhận không chính xác! Vui lòng sử dụng đúng link được gửi từ Thanh Sơn Garden tới Email của bạn!");
            }
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new Exception("Xác nhận Email không thành công! Link xác nhận không chính xác hoặc đã hết hạn! Vui lòng sử dụng đúng link được gửi từ Thanh Sơn Garden tới Email của bạn!");
            }
        }

        public async Task<IList<string>> ValidateAsync(RegisterModel model)
        {
            var validator = new RegisterModelValidator();
            var result = await validator.ValidateAsync(model);
            if (!result.IsValid)
            {
                var errors = new List<string>();
                errors.AddRange(result.Errors.Select(x => x.ErrorMessage));
                return errors;
            }
            return null;
        }

    }
}
