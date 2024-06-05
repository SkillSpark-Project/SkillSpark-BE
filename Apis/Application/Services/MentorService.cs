using Application.Interfaces;
using Application.ViewModels.MentorViewModels.Requests;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class MentorService : IMentorService
    {
        private readonly IUnitOfWork _unit;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public MentorService(IUnitOfWork unit, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _unit = unit;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task Create(string userId, MentorModel model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng");
            try
            {
                var mentor = _mapper.Map<Mentor>(model);
                mentor.UserId = userId;
                await _unit.MentorRepository.AddAsync(mentor);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
