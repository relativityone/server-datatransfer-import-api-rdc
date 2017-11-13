using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// 
    /// </summary>
    public class IoWarningPublisher
    {

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<IoWarningEventArgs> IoWarningEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void OnIoWarningEvent(IoWarningEventArgs e)
        {
            IoWarningEvent?.Invoke(this, e);
        }
    }
}
