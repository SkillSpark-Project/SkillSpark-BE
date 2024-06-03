using Application.Commons;
using Application.ViewModels.CategoryViewModels.Requests;
using Application.ViewModels.TagViewModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITagService
    {
        public Task<Pagination<Tag>> GetTags();
        public Task<Tag?> GetById(Guid id);
        public Task Add(TagModel categoryModel);
        public Task Update(Guid id, TagModel categoryModel);
        public Task Delete(Guid id);
    }
}
