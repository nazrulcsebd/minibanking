using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBanking.Core.Models.SearchModels
{
    public class SearchTransaction
    {
        public Guid CustomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? TransactionType { get; set; }
    }
}
