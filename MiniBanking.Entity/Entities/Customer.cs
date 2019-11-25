namespace MiniBanking.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Customer")]
    public partial class Customer : BaseEntity
    {
        //public Guid CustomerId { get; set; }

        [Required]
        [StringLength(150)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(150)]
        public string LastName { get; set; }

        [StringLength(150)]
        public string FatherName { get; set; }

        [StringLength(150)]
        public string MotherName { get; set; }

        public int CustomerType { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string ZIpCode { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string PersonalCode { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        //public virtual ICollection<AccountDeposit> AccountDeposit { get; set; }
        //public virtual ICollection<AccountTransaction> AccountTransaction { get; set; }
        //public virtual ICollection<AccountTransfer> AccountTransfer { get; set; }
        //public virtual ICollection<CustomerAccount> CustomerAccount { get; set; }
    }
}
