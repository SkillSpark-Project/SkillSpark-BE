using Application.Interfaces;
using Application.ViewModels.AuthViewModel;
using Application.ViewModels.MentorViewModels.Requests;
using AutoMapper;
using Domain.Entities;
using Firebase.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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

        public UserService(UserManager<ApplicationUser> userManager, IUnitOfWork unit, IMapper mapper, IConfiguration configuration)
        {
            _userManager = userManager;
            _unit = unit;
            _mapper = mapper;
            _configuration = configuration;
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
    }
}
