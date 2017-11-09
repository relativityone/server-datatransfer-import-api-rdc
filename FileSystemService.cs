using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace kCura.WinEDDS.TApi
{
    /// <inheritdoc />
    public class FileSystemService : IFileSystemService
    {
        /// <inheritdoc />
        public long GetFileLength(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            return fileInfo.Length;
        }
    }
}
