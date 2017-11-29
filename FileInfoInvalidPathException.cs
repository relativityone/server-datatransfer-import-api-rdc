using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Represent error that occures when there are illegal characters in the file path.
    /// </summary>
    public class FileInfoInvalidPathException : Exception
    {
        /// <summary>
        /// Constructor for <see cref="FileInfoInvalidPathException"/> class
        /// </summary>
        public FileInfoInvalidPathException(string message) : base(message)
        {
        }
    }
}
