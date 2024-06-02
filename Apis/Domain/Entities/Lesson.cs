using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Lesson : BaseEntity
    {
        [ForeignKey("Chapter")]
        public Guid ChapterId { get; set; }
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public string Url { get; set; }
        public virtual Chapter Chapter { get; set; }
    }
}
