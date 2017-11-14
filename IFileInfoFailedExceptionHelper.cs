using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Publish exception when file info operation failed
    /// </summary>
    public interface IFileInfoFailedExceptionHelper
    {
        /// <summary>
        /// Throws new FileInfoFailedException
        /// </summary>
        /// <param name="message"></param>
        void ThrowNewException(string message);
    }
}
