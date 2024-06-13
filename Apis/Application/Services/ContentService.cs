using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ContentService : IContentService 
    {
        private readonly IUnitOfWork _unit;

        public ContentService(IUnitOfWork unit)
        {
            _unit = unit;
        }
        
    }
}
