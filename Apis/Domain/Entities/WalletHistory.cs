using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WalletHistory:BaseEntity
    {
        [ForeignKey("Mentor")]
        public Guid MentorId { get; set; }
        public WalletHistoryType WalletHistoryType { get; set; }
        public double Amount { get; set; }
        public WalletHistoryStatus WalletHistoryStatus { get; set; }
        
        public virtual Mentor Mentor { get; set; }
    }
}
