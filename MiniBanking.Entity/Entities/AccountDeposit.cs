namespace MiniBanking.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AccountDeposit")]
    public partial class AccountDeposit : BaseEntity
    {
        public Guid CustomerId { get; set; }

        public Guid AccountId { get; set; }

        [Column(TypeName = "date")]
        public DateTime DepositDate { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        //public virtual Customer Customer { get; set; }

        //public virtual CustomerAccount CustomerAccount { get; set; }
    }
}
