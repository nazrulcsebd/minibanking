using Microsoft.Extensions.Configuration;
using CodeBonds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBonds.Utility
{
    public static class ConfigurationUtility
    {
        public static string BLOBConnectionString()
        {
            var blob = Startup.StaticConfig.GetSection("BLOB").GetChildren()
                      .Select(item => new KeyValuePair<string, string>(item.Key, item.Value));

            return blob.Where(b => b.Key == "storageConnectionString").Select(s => s.Value).FirstOrDefault().ToString();
        }
    }
}
