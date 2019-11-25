using MiniBanking.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBanking.Core.Models.DTOModels.Inventory
{
    public class AccountTransactionDto : AccountTransaction
    {
        public string TransferText { get; set; }
        public string CurrencyTypeText { get; set; }
        public decimal Amount { get; set; }
    }
}
