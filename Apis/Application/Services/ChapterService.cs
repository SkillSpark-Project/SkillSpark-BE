using Application.Commons;
using Application.Interfaces;
using Application.ViewModels.CategoryViewModels.Requests;
using Application.ViewModels.ChapterViewModels;
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
    public class ChapterService : IChapterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChapterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IList<Chapter>> GetChapters()
        {
            var chapters = await _unitOfWork.ChapterRepository.GetAllQueryable().ToListAsync();
            return chapters;
        }
        public async Task AddChapter(ChapterModel model)
        {
            var couse = await _unitOfWork.CourseRepository.GetByIdAsync(model.CourseId);
            if (couse is null) throw new Exception("Không tìm thấy khóa học bạn yêu cầu.");
            var chapter = _mapper.Map<Chapter>(model);
            await CheckName(model, null);
            try
            {
                await _unitOfWork.ChapterRepository.AddAsync(chapter);
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
        public async Task UpdateChapter(Guid id, ChapterModel model)
        {
            var couse = await _unitOfWork.CourseRepository.GetByIdAsync(model.CourseId);
            if (couse is null) throw new Exception("Không tìm thấy khóa học bạn yêu cầu.");
            var chapter = _mapper.Map<Chapter>(model);
            chapter.Id = id;
            var result = await _unitOfWork.ChapterRepository.GetByIdAsync(chapter.Id);
            if (result == null)
                throw new Exception("Không tìm thấy chương học bạn muốn tìm!");
            await CheckName(model, id);
            try
            {

                _unitOfWork.ChapterRepository.Update(chapter);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw new Exception("Đã xảy ra lỗi trong quá trình cập nhật. Vui lòng thử lại!");
            }
        }
        public async Task DeleteChapter(Guid id)
        {
            var result = await _unitOfWork.ChapterRepository.GetAllQueryable().Include(x=>x.Lessons).AsNoTracking().FirstOrDefaultAsync(x=>x.Id == id && !x.IsDeleted);
            if (result == null)
                throw new Exception("Không tìm thấy!");
            if (result.Lessons.Count >= 0)
            {
                throw new Exception("Còn tồn tại bài học thuộc về chương học này, không thể xóa!");
            }
            try
            {
                _unitOfWork.ChapterRepository.SoftRemove(result);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw new Exception("Đã xảy ra lỗi trong quá trình xóa. Vui lòng thử lại!");
            }
        }
        private async Task CheckName(ChapterModel model, Guid? id)
        {

            var categories = await _unitOfWork.ChapterRepository.GetAllQueryable().Where(x => x.Name.Equals(model.Name) && x.CourseId == model.CourseId)
               .AsNoTracking().ToListAsync();
            if (categories.Any())
                throw new Exception("Tên chương này đã tồn tại!");
            if (id != null)
            {
                if (categories.Any() && categories.First().Id != id)
                    throw new Exception("Tên chương này đã tồn tại!");
            }
        }
    }
}
