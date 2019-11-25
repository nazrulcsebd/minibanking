namespace MiniBanking.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AccountTransaction")]
    public partial class AccountTransaction : BaseEntity
    {
        public Guid CustomerId { get; set; }

        public Guid AccountId { get; set; }

        [Column(TypeName = "date")]
        public DateTime TransactionDate { get; set; }

        public int TransferType { get; set; }

        [Column(TypeName = "money")]
        public decimal DrAmount { get; set; }

        [Column(TypeName = "money")]
        public decimal CrAmount { get; set; }

        //public virtual Customer Customer { get; set; }

        //public virtual CustomerAccount CustomerAccount { get; set; }
    }
}
