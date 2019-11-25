namespace MiniBanking.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CustomerAccount")]
    public partial class CustomerAccount : BaseEntity
    {
        public Guid CustomerId { get; set; }

        public int AccountType { get; set; }

        public long AccountNumber { get; set; }

        public int CurrencyType { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        public bool RecordStatus { get; set; }

        //public virtual ICollection<AccountDeposit> AccountDeposit { get; set; }
        //public virtual ICollection<AccountTransaction> AccountTransaction { get; set; }
        //public virtual ICollection<AccountTransfer> AccountTransfer { get; set; }
        //public virtual ICollection<AccountTransfer> AccountTransfer1 { get; set; }

        //public virtual Customer Customer { get; set; }
    }
}
