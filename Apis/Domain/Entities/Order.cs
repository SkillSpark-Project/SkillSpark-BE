using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order:BaseEntity
    {
        [ForeignKey("Learner")]
        public Guid LearnerId { get; set; }
        public double Total { get; set; }
        public virtual Learner Learner { get; set; }
    }
}
