using Application.Commons;
using Application.Commons.Exeptions;
using Application.Interfaces;
using Application.ViewModels.AuthViewModel;
using Application.ViewModels.MentorViewModels.Requests;
using Application.ViewModels.UserViewModels.Requests;
using Application.ViewModels.UserViewModels.Responses;
using AutoMapper;
using Domain.Entities;
using Firebase.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IFirebaseService _firebaseService;

        public UserService(UserManager<ApplicationUser> userManager, IUnitOfWork unit,
            IMapper mapper, IConfiguration configuration, IFirebaseService firebaseService)
        {
            _userManager = userManager;
            _unit = unit;
            _mapper = mapper;
            _configuration = configuration;
            _firebaseService = firebaseService;
        }

        public async Task<LoginViewModel> CreateMentorAccount(MentorModel model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new Exception("Không tìm thấy người dùng.");
            var mentor = await _unit.MentorRepository.GetAllQueryable().FirstOrDefaultAsync(x => x.UserId.ToLower().Equals(userId.ToLower()));
            if (mentor != null) throw new Exception("Tài khoản này đã đăng ký trở thành giảng viên.");
            else
            {
                _unit.BeginTransaction();
                try
                {
                    //thêm vao bang mentor
                    var newMentor = _mapper.Map<Mentor>(model);
                    newMentor.UserId = userId;
                    await _unit.MentorRepository.AddAsync(newMentor);
                    await _unit.SaveChangeAsync();

                    // thêm role
                    var addRoleResult = await _userManager.AddToRoleAsync(user, "Mentor");
                    if (addRoleResult.Succeeded)
                    {
                        //regenerate token
                        var token = await GenerateJwtToken(user);
                        if (token != null)
                        {
                            var roles = await _userManager.GetRolesAsync(user);
                            var userModel = new LoginViewModel();
                            userModel.Id = user.Id;
                            userModel.Email = user.Email;
                            userModel.FullName = user.Fullname;
                            userModel.Username = user.UserName;
                            userModel.Avatar = user.Avatar;
                            userModel.listRoles = roles.ToList();
                            userModel.Token = token;
                            await _unit.CommitTransactionAsync();
                            return userModel;
                        }
                        else
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Mentor");
                            throw new Exception("Đã xảy ra lỗi trong quá trình tạo token. Vui lòng thử lại!");
                        }
                    }
                    else
                    {
                        _unit.RollbackTransaction();
                        throw new Exception("Đã xảy ra lỗi trong quá trình thêm role. Vui lòng thử lại!");
                    }

                }
                catch (Exception ex)
                {
                    _unit.RollbackTransaction();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<string> GenerateJwtToken(ApplicationUser user)
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

        public async Task<Pagination<UserViewModel>> GetListUserAsync(int pageIndex = 0, int pageSize = 20)
        {
            var manager = await _userManager.GetUsersInRoleAsync("Admin");
            var listUser = await _userManager.Users.AsNoTracking().Where(x => x.Id != manager.FirstOrDefault().Id).OrderBy(x => x.Email).ToListAsync();
            var itemCount = listUser.Count();
            var items = listUser.Skip(pageIndex * pageSize)
                                    .Take(pageSize)
                                    .ToList();
            var result = new Pagination<ApplicationUser>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = itemCount,
                Items = items,
            };
            var paginationList = _mapper.Map<Pagination<UserViewModel>>(result);
            foreach (var item in paginationList.Items)
            {
                var user = await _userManager.FindByIdAsync(item.Id);
                var isLockout = await _userManager.IsLockedOutAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                item.IsLockout = isLockout;
                item.Roles = roles;
            }
            return paginationList;
        }

        public async Task<UserViewModel> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = _mapper.Map<UserViewModel>(user);
            var isLockout = await _userManager.IsLockedOutAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            result.IsLockout = isLockout;
            result.Roles = roles;
            return result;
        }

        public async Task ChangePasswordAsync(ChangePassModel model, string userId)
        {
            if (!model.NewPassword.Equals(model.ConfirmPassword))
            {
                throw new Exception("Mật khẩu xác nhận không trùng khớp.");
            }
            if (model.NewPassword.Equals(model.OldPassword))
            {
                throw new Exception("Mật khẩu mới phải khác mật khẩu cũ.");
            }
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!result.Succeeded)
                    throw new Exception($"Mật khẩu không chính xác.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        public async Task<LoginViewModel> UpdateUserAsync(UserRequestModel model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Không tìm thấy người dùng.");
            }
            else
            {
                var temp = await _userManager.Users.Where(x => !x.Id.ToLower().Equals(user.Id.ToLower()) && x.UserName.ToLower().Equals(model.Username.ToLower())).FirstOrDefaultAsync();
                if (temp != null)
                    throw new Exception("Tên đăng nhập này đã được sử dụng.");

                string url = null;
                if (model.Avatar != null)
                {
                    Random random = new Random();
                    string newImageName = user.Id + "_i" + model.Avatar.Name.Trim() + random.Next(1, 10000).ToString();
                    string folderName = $"user/{user.Id}/Image";
                    string imageExtension = Path.GetExtension(model.Avatar.FileName);
                    //Kiểm tra xem có phải là file ảnh không.
                    string[] validImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    const long maxFileSize = 20 * 1024 * 1024;
                    if (Array.IndexOf(validImageExtensions, imageExtension.ToLower()) == -1 || model.Avatar.Length > maxFileSize)
                    {
                        throw new Exception("Có chứa file không phải ảnh hoặc quá dung lượng tối đa(>20MB)!");
                    }
                    url = await _firebaseService.UploadFileToFirebaseStorage(model.Avatar, newImageName, folderName);
                    if (url == null)
                        throw new Exception("Lỗi khi đăng ảnh lên firebase!");
                    user.Avatar = url;
                }
                user.UserName = model.Username;
                user.NormalizedUserName = model.Username.ToUpper();
                user.Fullname = model.Fullname;
                user.PhoneNumber = model.PhoneNumber;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(x => x.Description).ToList();
                    throw new ValidationException(errors);
                }
                else
                {
                    var updateUser = await _userManager.FindByIdAsync(userId);
                    var roles = await _userManager.GetRolesAsync(updateUser);
                    var userModel = new LoginViewModel();
                    userModel.Id = updateUser.Id;
                    userModel.Email = updateUser.Email;
                    userModel.FullName = updateUser.Fullname;
                    userModel.Username = updateUser.UserName;
                    userModel.Avatar = updateUser.Avatar;
                    userModel.listRoles = roles.ToList();
                    userModel.Token = null;
                    return userModel;
                }
            }
        }

    }
}
