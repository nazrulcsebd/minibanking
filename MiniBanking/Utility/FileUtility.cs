using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeBonds.Utility
{
    public class FileUtility
    {
        private IHostingEnvironment _hostingEnvironment;

        public FileUtility(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public void SaveFiles(Guid Id, IEnumerable<IFormFile> files)
        {   
           
        }
    }
}
