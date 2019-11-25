using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MiniBanking.Core.Helper
{
    public class Enums
    {
        public enum Status
        {
            [Description("All")]
            All = -1,

            [Description("Active")]
            Active = 1,

            [Description("In-Active")]
            InActive = 0
        }
        public enum TransactionType
        {
            [Description("Deposit")]
            Deposit = 1,
            [Description("Witwdrawal")]
            Witwdrawal = 2,
            [Description("Transfer")]
            Transfer = 3
        }
        public enum NotificationType
        {
            Success,
            Info,
            Warning,
            Error
        }
    }
}
