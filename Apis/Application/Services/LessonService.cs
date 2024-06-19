using Application.Interfaces;
using Application.ViewModels.ChapterViewModels;
using Application.ViewModels.LessonViewModels.Requests;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LessonService : ILessonService
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IFirebaseService _firebaseService;

        public LessonService(IUnitOfWork unit, IMapper mapper, ICloudinaryService cloudinaryService, IFirebaseService firebaseService)
        {
            _unit = unit;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _firebaseService = firebaseService;
        }
        public async Task CreateLesson(LessonModel model, string userId)
        {
            var chapter = await _unit.ChapterRepository.GetAllQueryable().Include(x=>x.Course).Include(x=>x.Lessons).FirstOrDefaultAsync(x=>x.Id == model.ChapterId);
            if (chapter == null) throw new Exception("Không tìm thấy chương mà bạn tìm.");
            var mentor = await _unit.MentorRepository.GetByIdAsync(chapter.Course.MentorId);
            if (mentor == null) throw new Exception("Bạn không có quyền truy cập vào tính năng này.");
            if(!mentor.UserId.ToLower().Equals(userId.ToLower())) throw new Exception("Bạn không có quyền truy cập vào tính năng này.");
            await CheckName(model, null);
            var result = await _cloudinaryService.UploadVideoAsync(model.VideoFile);
            if (result == null) throw new Exception("Đã xảy ra lỗi trong quá trình tải video.");
            Lesson lesson = new Lesson { ChapterId = model.ChapterId, Name = model.Name, IsOpen = model.IsOpen};
            lesson.Url = result.Url.ToString();
            lesson.SortNumber = chapter.Lessons.Count() + 1;
            _unit.BeginTransaction();
            try
            {
                await _unit.LessonRepository.AddAsync(lesson);
                await _unit.SaveChangeAsync();
                await AddDocument(model.Documents, lesson.Id);
                await _unit.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _unit.RollbackTransaction();
                throw new Exception(ex.Message);
            }
        }

        private async Task AddDocument(IList<IFormFile> documents, Guid lessonId)
        {
            var listUrl = new List<Document>();
            foreach (var item in documents)
            {
                var random = new Random();
                string newImageName = lessonId + "_i" + item.Name.Trim().ToString() + "_" + random.Next(1, 10001);
                string folderName = $"Document/{lessonId}";
                string imageExtension = Path.GetExtension(item.FileName);
                var url = await _firebaseService.UploadFileToFirebaseStorage(item, newImageName, folderName);
                if (url == null)
                    throw new Exception("Lỗi khi đăng ảnh lên Firebase!");
                var doc = new Document { LessonId = lessonId, DocumentUrl= url};
                listUrl.Add(doc);
            }
            await _unit.DocumentRepository.AddRangeAsync(listUrl);
            await _unit.SaveChangeAsync();
        }

        private async Task CheckName(LessonModel model, Guid? id)
        {

            var lessons = await _unit.LessonRepository.GetAllQueryable().Where(x => x.Name.Equals(model.Name) && x.ChapterId == model.ChapterId).AsNoTracking().ToListAsync();
            if (lessons.Any())
                throw new Exception("Tên bài giảng này đã tồn tại!");
            if (id != null)
            {
                var temp  = lessons.FirstOrDefault(x => x.Id != id);
                if (temp!= null)
                    throw new Exception("Tên chương này đã tồn tại!");
            }
        }
    }
}
