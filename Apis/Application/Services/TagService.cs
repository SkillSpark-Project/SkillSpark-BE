using Application.Commons;
using Application.Interfaces;
using Application.ViewModels.CategoryViewModels.Requests;
using Application.ViewModels.TagViewModels;
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
    public class TagService: ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TagService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Pagination<Tag>> GetTags()
        {
            var tags = await _unitOfWork.TagRepository.GetAsync(isTakeAll: true, expression: x => !x.IsDeleted, isDisableTracking: true, orderBy: x => x.OrderBy(cate => cate.Name.Contains("Khác")));
            return tags;
        }
        public async Task Add(TagModel model)
        {
            var tag = _mapper.Map<Tag>(model);
            await CheckName(tag.Name, null);
            try
            {
                await _unitOfWork.TagRepository.AddAsync(tag);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw new Exception("Đã xảy ra lỗi trong quá trình tạo mới. Vui lòng thử lại!");
            }
        }
        public async Task<Tag?> GetById(Guid id)
        {
            var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);
            return tag;
        }
        public async Task Update(Guid id, TagModel model)
        {
            var tag = _mapper.Map<Tag>(model);
            tag.Id = id;
            var result = await _unitOfWork.TagRepository.GetByIdAsync(tag.Id);
            if (result == null)
                throw new Exception("Không tìm thấy phân loại!");
            await CheckName(tag.Name, id);
            try
            {
                _unitOfWork.TagRepository.Update(tag);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw new Exception("Đã xảy ra lỗi trong quá trình cập nhật. Vui lòng thử lại!");
            }
        }
        public async Task Delete(Guid id)
        {
            var result = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (result == null)
                throw new Exception("Không tìm thấy!");
            var courses = await _unitOfWork.CourseRepository.GetAsync(pageIndex: 0, pageSize: 1, expression: x => x.CategoryId == id && !x.IsDeleted);
            if (courses.TotalItemsCount > 0)
            {
                throw new Exception("Còn tồn tại khóa học thuộc về nhãn dán này, không thể xóa!");
            }
            try
            {
                _unitOfWork.TagRepository.SoftRemove(result);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw new Exception("Đã xảy ra lỗi trong quá trình xóa. Vui lòng thử lại!");
            }
        }
        private async Task CheckName(string categoryName, Guid? id)
        {

            var tags = await _unitOfWork.TagRepository.GetAllQueryable().Where(x => x.Name.Equals(categoryName))
               .AsNoTracking().ToListAsync();
            if (tags.Any())
                throw new Exception("Nhãn dán này đã tồn tại!");
            if (id != null)
            {
                if (tags.Any() && tags.First().Id != id)
                    throw new Exception("Nhãn dán này đã tồn tại!");
            }
        }
    }
}
