using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBanking.Core.Helper.Entities
{
    public class AppClaim
    {
        public string UserId { get; set; } 
        public string CustomerId { get; set; }
        public string AccountId { get; set; }
        public string PersonalCode { get; set; }
        public string Email { get; set; }
        public string UserTypeId { get; set; }
        public string SubscriptionId { get; set; }

        public string DBName { get; set; }
        public string DBUserId { get; set; }
        public string DBUserPassword { get; set; }
    }
}
