using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace kCura.WinEDDS.TApi
{
    class FileSystemService : IFileSystemService
    {
        public long GetFileLength(string filename)
        {
            var fileInfo = new FileInfo(filename);
            return fileInfo.Length;
        }
    }
}
