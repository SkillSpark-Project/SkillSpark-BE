using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CourseViewModels.Responses
{
    public class FilterModel
    {
        public int IndexPage { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
