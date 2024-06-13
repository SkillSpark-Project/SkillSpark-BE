using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Course:BaseEntity
    {
        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        [ForeignKey("Mentor")]
        public Guid MentorId { get; set; }
        public string Name { get; set; }
        public string Image {  get; set; }
        public string Description { get; set; }
        public float Rate { get; set; } = 0;
        public double? Price { get; set; }
        public int NumberLearner { get; set; } = 0;
        public string ShortDescripton { get; set; }
        public virtual Category Category { get; set; }
        public virtual Mentor Mentor { get; set; }


    }
}
