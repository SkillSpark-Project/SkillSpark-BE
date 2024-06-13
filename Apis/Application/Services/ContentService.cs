using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ContentService: IContentService
    {
        private readonly IUnitOfWork _unit;

        public ContentService(IUnitOfWork unit)
        {
            _unit = unit;
        }
        public async Task AddRange(IList<string> contents, Guid courseId)
        {
            var list = new List<Content>();
            foreach (var item in contents.Distinct())
            {
                var req = new Content { CourseId = courseId, Detail = item };
                list.Add(req);
            }
            await _unit.ContentRepository.AddRangeAsync(list);
            await _unit.SaveChangeAsync();
        }
    }
}
