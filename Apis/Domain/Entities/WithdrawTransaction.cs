using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public  class WithdrawTransaction:BaseEntity
    {
        [ForeignKey("WalletHistory")]
        public Guid WalletHistoryId { get; set; }
        public virtual WalletHistory WalletHistory { get; set; }
    }
}
