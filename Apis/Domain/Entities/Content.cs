using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Content:BaseEntity
    {
        [ForeignKey("Course")]
        public Guid CourseId { get; set; }
        public string Detail {  get; set; }
        [JsonIgnore]
        public virtual Course Course { get; set; }
    }
}
