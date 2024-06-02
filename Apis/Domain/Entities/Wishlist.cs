using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Wishlist: BaseEntity
    {
        [ForeignKey("Learner")]
        public Guid LearnerId { get; set; }
        [ForeignKey("Course")]
        public Guid CourseId { get; set; }
        public virtual Learner Learner { get; set; }
        public virtual Course Course { get; set; }

    }
}
