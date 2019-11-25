namespace MiniBanking.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AccountWithdrawal")]
    public partial class AccountWithdrawal : BaseEntity
    {
        public Guid CustomerId { get; set; }

        public Guid AccountId { get; set; }

        [Column(TypeName = "date")]
        public DateTime WithdrawalDate { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        //public virtual Customer Customer { get; set; }

        //public virtual CustomerAccount CustomerAccount { get; set; }
    }
}
