using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.LessonViewModels.Requests
{
    public class LessonModel
    {
        public Guid ChapterId { get; set; }
        public string Name { get; set; }
        public bool IsOpen { get; set; } = false;
        public IFormFile VideoFile { get; set; }
        public IList<IFormFile> Documents { get; set; }
    }
}
