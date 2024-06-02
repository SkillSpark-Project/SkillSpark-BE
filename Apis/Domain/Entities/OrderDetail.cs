using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderDetail:BaseEntity
    {
        [ForeignKey("Course")]
        public Guid CourseId { get; set; }
        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        public double Price { get; set; }
        public virtual Course Course { get; set; }
        public virtual Order Order { get; set; }

    }
}
