
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CourseTag:BaseEntity
    {
        [ForeignKey("Course")]
        public Guid CourseId { get; set; }
        [ForeignKey("Tag")]
        public Guid TagId { get; set; }
        public virtual Course Course { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
