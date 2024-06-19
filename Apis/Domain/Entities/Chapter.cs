using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Chapter : BaseEntity
    {
        [ForeignKey("Course")]
        public Guid CourseId { get; set; }
        public string Name { get; set; }
        public int SortNumber { get; set; } 
        [JsonIgnore]
        public virtual Course Course { get; set; }
        public IList<Lesson> Lessons { get; set; }
    }
}
