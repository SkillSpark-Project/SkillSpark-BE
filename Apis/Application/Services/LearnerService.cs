using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LearnerService : ILearnerService
    {
        private readonly IUnitOfWork _unit;
        private readonly UserManager<ApplicationUser> _userManager;

        public LearnerService(IUnitOfWork unit, UserManager<ApplicationUser> userManager)
        {
            _unit = unit;
            _userManager = userManager;
        }
        public async Task CreateLearner(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng");
            try
            {
                var learner = new Learner { UserId = userId };
                await _unit.LearnerRepository.AddAsync(learner);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
