// ----------------------------------------------------------------------------
// <copyright file="ProcessEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents the process event argument data.
	/// </summary>
	[Serializable]
	public sealed class ProcessEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
		/// </summary>
		/// <param name="eventType">
		/// The process event type.
		/// </param>
		/// <param name="recordInfo">
		/// The current record information.
		/// </param>
		/// <param name="message">
		/// The event message.
		/// </param>
		public ProcessEventArgs(ProcessEventType eventType, string recordInfo, string message)
		{
			this.EventType = eventType;
			this.RecordInfo = recordInfo;
			this.Message = message;
			this.Timestamp = DateTime.Now;
		}

		/// <summary>
		/// Gets the process event type.
		/// </summary>
		/// <value>
		/// The <see cref="ProcessEventType"/> value.
		/// </value>
		public ProcessEventType EventType { get; }

		/// <summary>
		/// Gets the event message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public string Message { get; }

		/// <summary>
		/// Gets the record information associated with this event.
		/// </summary>
		/// <value>
		/// The record information.
		/// </value>
		public string RecordInfo { get; }

		/// <summary>
		/// Gets the event timestamp.
		/// </summary>
		/// <value>
		/// The <see cref="DateTime"/> value.
		/// </value>
		public DateTime Timestamp { get; }
	}
}