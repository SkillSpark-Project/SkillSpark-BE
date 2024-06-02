using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Connection: BaseEntity
    {
        [ForeignKey("Mentor")]
        public Guid MentorId { get; set; }
        [ForeignKey("ConnectionType")]
        public Guid ConnectionTypeId { get; set; }
        public string Url { get; set; }
        public virtual Mentor Mentor { get; set;}
        public virtual ConnectionType ConnectionType { get; set; }
    }
}
