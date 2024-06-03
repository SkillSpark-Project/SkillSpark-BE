using Application.Commons;
using Application.ViewModels.CategoryViewModels.Requests;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        public Task<Pagination<Category>> GetCategories();
        public Task<Category?> GetCategoryById(Guid id);
        public Task AddCategory(CategoryModel categoryModel);
        public Task UpdateCategory(Guid id, CategoryModel categoryModel);
        public Task DeleteCategory(Guid id);
    }
}
