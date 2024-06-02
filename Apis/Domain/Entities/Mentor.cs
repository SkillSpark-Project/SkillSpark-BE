using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Mentor : BaseEntity
    {
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public string Introduction { get; set; }
        public string Description { get; set; }
        public int NumberLearner { get; set; } = 0;
        public double Balance { get; set; } = 0;
        public float Rate { get; set; } = 0;
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Identication? Identication { get; set; }
        public virtual BankInformation? BankInformation { get; set; }

    }
}
