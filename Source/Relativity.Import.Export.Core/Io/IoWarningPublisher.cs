// ----------------------------------------------------------------------------
// <copyright file="IoWarningPublisher.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;

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
		/// Publishes the I/O warning event.
		/// </summary>
		/// <param name="eventArgs">
		/// The event arguments.
		/// </param>
		public void PublishIoWarningEvent(IoWarningEventArgs eventArgs)
		{
			this.IoWarningEvent?.Invoke(this, eventArgs);
		}
	}
}