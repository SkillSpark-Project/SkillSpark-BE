using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Identication: BaseEntity
    {
        [ForeignKey("Mentor")]
        public Guid MentorId { get; set; }
        public string FrontUrl { get; set; }
        public string BackUrl { get; set; }
        public virtual Mentor Mentor { get; set; }
    }
}
