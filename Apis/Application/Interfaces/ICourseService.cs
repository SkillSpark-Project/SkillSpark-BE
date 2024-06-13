using Application.ViewModels.CourseViewModels.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICourseService
    {
        public Task AddCourse(CourseModel model);
    }
}
