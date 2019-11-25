using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBanking.Core.Helper
{
    public class Constants
    {
        public static string SqlServer { get; set; }
        public static string SqlServerUserId { get; set; }
        public static string SqlServerPassword { get; set; }
        public static string DefaultConnectionString { get; set; }
        public static string MasterConnectionString { get; set; }

        public static string UploadDocumentLocation = "~/Uploads/ContractDocuments/";
    }
}
