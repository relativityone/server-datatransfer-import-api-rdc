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
		/// This is an event which can be raised by any method which handles IO Warnings.
		/// </summary>
		public event EventHandler<IoWarningEventArgs> IoWarningEvent;

		/// <summary>
		/// Raises IO Warning event.
		/// </summary>
		/// <param name="eventArgs"></param>
		public void PublishIoWarningEvent(IoWarningEventArgs eventArgs)
		{
			IoWarningEvent?.Invoke(this, eventArgs);
		}
	}
}
