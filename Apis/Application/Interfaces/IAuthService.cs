using Application.ViewModels.AuthViewModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        public Task<LoginViewModel> Login(string email, string pass, string callbackUrl);
        public Task<ApplicationUser> Register(RegisterModel model);
        //public Task CheckAccountExist(RegisterModel model);
        public Task ConfirmEmailAsync(string? code, string? userId);
        public Task<bool> SendEmailAsync(ApplicationUser user, string callbackUrl, string type);
        /* public Task<string> ResetPasswordAsync(ResetPassModel model);*/
        /*public Task<IList<string>> ValidateAsync(RegisterModel model);*/
        /* public Task<string> ResetPasswordAsync(ResetPassModel model);*/
    }
}
