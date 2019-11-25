using MiniBanking.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBanking.Core.Models.DTOModels.Inventory
{
    public class AccountBalanceDto
    {
        public Guid CustomerId { get; set; }
        public Guid AccountId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string AccountType { get; set; }
        public long AccountNumber { get; set; }
        public decimal BalanceAmount { get; set; }
    }
}
