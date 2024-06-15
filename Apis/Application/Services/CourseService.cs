using Application.Commons;
using Application.Interfaces;
using Application.ViewModels.CourseViewModels.Requests;
using Application.ViewModels.CourseViewModels.Responses;
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
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly IFirebaseService _firebaseService;
        private readonly IContentService _contentService;
        private readonly IRequirementService _requirementService;

        public CourseService(IUnitOfWork unit, IMapper mapper, IFirebaseService firebaseService, IContentService contentService, IRequirementService requirementService)
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
        public async Task AddCourse(CourseModel model, string userId)
        {
            var mentor = _unit.MentorRepository.GetAllQueryable().FirstOrDefault(x=>x.UserId.ToLower().Equals(userId.ToLower()));
            if (mentor == null) throw new Exception("Không tìm thấy thông tin giảng viên");
            var course = _mapper.Map<Course>(model);
            course.MentorId = mentor.Id;
            course.Id = Guid.NewGuid();
            var url = await AddCourseImage(course.Id, model.Image);
            course.Image = url;
            _unit.BeginTransaction();
            try
            {
                await _unit.CourseRepository.AddAsync(course);
                await _unit.SaveChangeAsync();
                await AddRangeContents(model.Contents, course.Id);
                await AddRangeReqs(model.Requirements, course.Id);
                await AddRangeTags(model.Tags, course.Id);
                await _unit.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _unit.RollbackTransaction();
                throw new Exception(ex.Message);
            }
        }

        private async Task AddRangeReqs(IList<string> reqs, Guid courseId)
        {
            var list = new List<Requirement>();
            foreach (var item in reqs.Distinct())
            {
                var req = new Requirement { CourseId = courseId, Detail = item };
                list.Add(req);
            }
            await _unit.RequirementRepository.AddRangeAsync(list);
            await _unit.SaveChangeAsync();
        }

        private async Task AddRangeContents(IList<string> contents, Guid courseId)
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

        private async Task AddRangeTags(IList<Guid> tags, Guid courseId)
        {
            var list = new List<CourseTag>();
            foreach (var item in tags.Distinct())
            {
                var tag = new CourseTag { CourseId = courseId, TagId = item };
                list.Add(tag);
            }
            await _unit.CourseTagRepository.AddRangeAsync(list);
            await _unit.SaveChangeAsync();
        }

        public async Task<Pagination<Course>> GetList(FilterModel model)
        {
            var list = await _unit.CourseRepository.GetAllQueryable().Include(x=>x.Category).Include(x=>x.Mentor.ApplicationUser).ToListAsync();
            if (model.Keyword != null) list.Where(x => x.NameUnsign.ToLower().Contains(model.Keyword.ToLower()));
            if (model.MinPrice != null) list = list.Where(x => x.Price.Value >= model.MinPrice).ToList();
            if (model.MaxPrice != null) list = list.Where(x => x.Price.Value <= model.MaxPrice).ToList();
            if (model.CategoryId != null) list =  list.Where(x => x.CategoryId == model.CategoryId).ToList();
            var items = list.Skip(model.IndexPage * model.PageSize)
                                    .Take(model.PageSize)
                                    .ToList();
            var result = new Pagination<Course>()
            {
                PageIndex = model.IndexPage,
                PageSize = model.PageSize,
                TotalItemsCount = list.Count,
                Items = items,
            };
            return result;
        }

        public async Task<Course> GetById(Guid id)
        {
            var course = await _unit.CourseRepository.GetAllQueryable().Include(x => x.Category)
                .Include(x => x.Mentor.ApplicationUser)
                .Include(x=>x.Contents)
                .Include(x => x.Requirements)
                .Include(x => x.CourseTags).ThenInclude(x=>x.Tag)
                .FirstOrDefaultAsync(x=>x.Id == id);
            if (course == null) throw new Exception("Không tìm thấy khóa học.");
            return course;
        }
    }
}
