using Application.ViewModels.LessonViewModels.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILessonService
    {
        public Task CreateLesson(LessonModel model, string userId);

    }
}
