namespace MiniBanking.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AccountTransfer")]
    public partial class AccountTransfer : BaseEntity
    {
        public Guid FromCustomerId { get; set; }
        public Guid ToCustomerId { get; set; }

        [Column(TypeName = "date")]
        public DateTime TransferDate { get; set; }

        public int TransferType { get; set; }

        public Guid FromAccountId { get; set; }

        public Guid ToAccountId { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        //public virtual Customer Customer { get; set; }

        //public virtual CustomerAccount CustomerAccount { get; set; }

        //public virtual CustomerAccount CustomerAccount1 { get; set; }
    }
}
