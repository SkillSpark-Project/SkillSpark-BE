using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CourseViewModels.Requests
{
    public class CourseModel
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public string Description { get; set; }
        public string ShortDescripton { get; set; }
        public IList<string> Requirements { get; set; }
        public IList<Guid> Tags { get; set; }
        public IList<string> Contents { get; set; }
    }
}
