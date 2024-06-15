using Application.Commons;
using Application.ViewModels.CourseViewModels.Requests;
using Application.ViewModels.CourseViewModels.Responses;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICourseService
    {
        public Task AddCourse(CourseModel model, string userID);
        public Task<Pagination<Course>> GetList(FilterModel model);
        public Task<Course> GetById(Guid id);

    }
}
