using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Document:BaseEntity
    {
        [ForeignKey("Lesson")]
        public Guid LessonId { get; set; }
        public string DocumentUrl { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
}
