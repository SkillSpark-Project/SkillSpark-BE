using Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Course : BaseEntity
    {
        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        [ForeignKey("Mentor")]
        public Guid MentorId { get; set; }
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NameUnsign = RemoveDiacritics(value);
            }
        }
        private string RemoveDiacritics(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormD);
            StringBuilder result = new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (c == 'đ' || c == 'Đ')
                {
                    result.Append('d');
                }
                else if (category != UnicodeCategory.NonSpacingMark)
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
        public string NameUnsign { get; private set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public float Rate { get; set; } = 0;
        public double? Price { get; set; }
        public int NumberLearner { get; set; } = 0;
        public string ShortDescripton { get; set; }
        public string? CancelReason { get; set; }

        public CourseStatus CourseStatus { get; set; } = CourseStatus.Preparing;

        public virtual Category Category { get; set; }
        public virtual Mentor Mentor { get; set; }
        public IList<Content> Contents { get; set; }
        public IList<Requirement> Requirements { get; set; }
        public IList<CourseTag> CourseTags { get; set; }
        public IList<Chapter> Chapter { get; set; }
        


    }
}
