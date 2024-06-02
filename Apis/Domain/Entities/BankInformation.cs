using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BankInformation: BaseEntity
    {
        [ForeignKey("Mentor")]
        public Guid MentorId { get; set; }
        public string BankName { get; set; }
        public string BankAccountHolder { get; set; }
        public string BankNumber { get; set; }
        public virtual Mentor Mentor { get; set; }
    }
}
