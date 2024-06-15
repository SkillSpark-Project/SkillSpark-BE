using Application.Commons;
using Application.ViewModels.AuthViewModel;
using Application.ViewModels.MentorViewModels.Requests;
using Application.ViewModels.UserViewModels.Requests;
using Application.ViewModels.UserViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        public Task<LoginViewModel> CreateMentorAccount(MentorModel model, string userId);
        public Task<Pagination<UserViewModel>> GetListUserAsync(int pageIndex = 0, int pageSize = 20);
        public Task<UserViewModel> GetUserById(string id);
        public Task ChangePasswordAsync(ChangePassModel model, string userId);
        public Task<LoginViewModel> UpdateUserAsync(UserRequestModel model, string userId);


    }
}
