using Application.Interfaces;
using Application.ViewModels.CourseViewModels.Requests;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly FirebaseService _firebaseService;
        private readonly IContentService _contentService;
        private readonly IRequirementService _requirementService;

        public CourseService(IUnitOfWork unit, IMapper mapper, FirebaseService firebaseService, IContentService contentService, IRequirementService requirementService)
        {
            _unit = unit;
            _mapper = mapper;
            _firebaseService = firebaseService;
            _contentService = contentService;
            _requirementService = requirementService;
        }

        public async Task<string> AddCourseImage(Guid id, IFormFile file)
        {
            string newImageName = id + "_i" + file.Name.Trim().ToString();
            string folderName = $"Course/{id}";
            string imageExtension = Path.GetExtension(file.FileName);
            //Kiểm tra xem có phải là file ảnh không.
            string[] validImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            const long maxFileSize = 20 * 1024 * 1024;
            if (Array.IndexOf(validImageExtensions, imageExtension.ToLower()) == -1 || file.Length > maxFileSize)
            {
                throw new Exception("Có chứa file không phải ảnh hoặc quá dung lượng tối đa(>20MB)!");
            }
            var url = await _firebaseService.UploadFileToFirebaseStorage(file, newImageName, folderName);
            if (url == null)
                throw new Exception("Lỗi khi đăng ảnh lên Firebase!");
            return url;
        }
        public async Task AddCourse(CourseModel model)
        {
            var course = _mapper.Map<Course>(model);
            var url = await AddCourseImage(course.Id, model.Image);
            course.Image = url;
            _unit.BeginTransaction();
            try
            {
                await _unit.CourseRepository.AddAsync(course);
                await _unit.SaveChangeAsync();
                await _contentService.AddRange(model.Contents, course.Id);
                await _requirementService.AddRange(model.Requirements, course.Id);
                await _unit.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _unit.RollbackTransaction();
                throw new Exception(ex.Message);
            }
        }

    }
}
