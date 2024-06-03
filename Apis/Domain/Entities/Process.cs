using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Process:BaseEntity
    {
        [ForeignKey("Learner")]
        public Guid LearnerId { get; set; }
        [ForeignKey("Lesson")]
        public Guid LessonId { get; set; }
        public bool IsDone { get; set; }

        public virtual Learner Learner { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
}
