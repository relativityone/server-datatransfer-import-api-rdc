using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Publisher fot IO Warning event
    /// </summary>
    public class IoWarningPublisher
    {
        /// <summary>
        /// Represents the method that will handle an IO Warning event
        /// </summary>
        public delegate void IoWarningEventHandler(IoWarningEventArgs e);

        /// <summary>
        /// Event which can cause any method that handles IO Warning
        /// </summary>
        public event IoWarningEventHandler IoWarningEvent();

        /// <summary>
        /// Raise IO Warning event
        /// </summary>
        /// <param name="e"></param>
        public void OnIoWarningEvent(IoWarningEventArgs e)
        {
            IoWarningEvent?.Invoke(e);
        }
    }
}
