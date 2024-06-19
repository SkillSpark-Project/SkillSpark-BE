using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Lesson : BaseEntity
    {
        [ForeignKey("Chapter")]
        public Guid ChapterId { get; set; }
        public string Name { get; set; }
        public bool IsOpen { get; set; }= false;
        public string Url { get; set; }
        public int SortNumber { get; set; } 
        [JsonIgnore]

        public virtual Chapter Chapter { get; set; }
        public IList<Document> Documents { get; set; }
    }
}
