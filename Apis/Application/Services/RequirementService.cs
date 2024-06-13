using Application.Interfaces;
using Application.ViewModels.RequirementViewModels.Requests;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RequirementService:IRequirementService
    {
        private readonly IUnitOfWork _unit;

        public RequirementService(IUnitOfWork unit)
        {
            _unit = unit;
        }
        public async Task AddRange(IList<string> reqs, Guid courseId)
        {
            var reqsList = new List<Requirement>();
            foreach (var item in reqs.Distinct())
            {
                var req = new Requirement { CourseId = courseId , Detail = item};      
                reqsList.Add(req);
            }
            await _unit.RequirementRepository.AddRangeAsync(reqsList);
            await _unit.SaveChangeAsync();
        }
    }
}
