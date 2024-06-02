using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Chapter : BaseEntity
    {
        [ForeignKey("Course")]
        public Guid CourseId { get; set; }
        public string Name { get; set; }
        public virtual Course Course { get; set; }
    }
}
