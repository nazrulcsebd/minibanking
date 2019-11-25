using MiniBanking.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBanking.Core.Models.DTOModels.Inventory
{
    public class CustomerDto : Customer
    {
        public int AccountType { get; set; }     
        public int CurrencyType { get; set; }
        public decimal Amount { get; set; }
    }
}
