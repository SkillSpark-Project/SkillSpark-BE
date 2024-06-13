using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RequirementService: IRequirementService
    {
        private readonly IUnitOfWork _unit;

        public RequirementService(IUnitOfWork unit)
        {
            _unit = unit;
        }
        
    }
}
