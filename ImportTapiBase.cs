using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Base class for Files Importertes
    /// </summary>
    public class ImportTapiBase
    {
        private IFileSystemService fileSystemService;
        private IWaitAndRetryPolicy waitAndRetryPolicy;

        /// <inheritdoc />
        public ImportTapiBase(IFileSystemService fileService, IWaitAndRetryPolicy waitAndRetry)
        {
            fileSystemService = fileService;
            waitAndRetryPolicy = waitAndRetry;
        }

        public long GetFileLength(string fileName)
        {
            
        }
    }
}
