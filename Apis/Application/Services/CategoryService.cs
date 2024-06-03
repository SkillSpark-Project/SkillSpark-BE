using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using Application.ViewModels.CategoryViewModels.Requests;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService:ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Pagination<Category>> GetCategories()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAsync(isTakeAll: true, expression: x => !x.IsDeleted, isDisableTracking: true, orderBy: x => x.OrderBy(cate => cate.Name.Contains("Khác")));
            return categories;
        }
        public async Task AddCategory(CategoryModel categoryModel)
        {
            var category = _mapper.Map<Category>(categoryModel);
            await CheckName(category.Name, null);
            try
            {
                    await _unitOfWork.CategoryRepository.AddAsync(category);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw new Exception("Đã xảy ra lỗi trong quá trình tạo mới. Vui lòng thử lại!");
            }
        }
        public async Task<Category?> GetCategoryById(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            return category;
        }
        public async Task UpdateCategory(Guid id, CategoryModel categoryModel)
        {
            var category = _mapper.Map<Category>(categoryModel);
            category.Id = id;
            var result = await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id);
            if (result == null)
                throw new Exception("Không tìm thấy phân loại!");
            await CheckName(category.Name, id);
            try
            {
                
                _unitOfWork.CategoryRepository.Update(category);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw new Exception("Đã xảy ra lỗi trong quá trình cập nhật. Vui lòng thử lại!");
            }
        }
        public async Task DeleteCategory(Guid id)
        {
            var result = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (result == null)
                throw new Exception("Không tìm thấy!");
            var bonsais = await _unitOfWork.CourseRepository.GetAsync(pageIndex: 0, pageSize: 1, expression: x => x.CategoryId == id && !x.IsDeleted);
            if (bonsais.TotalItemsCount > 0)
            {
                throw new Exception("Còn tồn tại khóa học thuộc về phân loại này, không thể xóa!");
            }
            try
            {
                _unitOfWork.CategoryRepository.SoftRemove(result);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw new Exception("Đã xảy ra lỗi trong quá trình xóa. Vui lòng thử lại!");
            }
        }
        private async Task CheckName(string categoryName, Guid? id)
        {
            
            var categories = await _unitOfWork.CategoryRepository.GetAllQueryable().Where(x=>x.Name.Equals(categoryName))
               .AsNoTracking().ToListAsync();
            if (categories.Any())
                throw new Exception("Tên phân loại này đã tồn tại!");
            if(id != null)
            {
                if(categories.Any() && categories.First().Id != id)
                    throw new Exception("Tên phân loại này đã tồn tại!");
            }
        }
    }
}
