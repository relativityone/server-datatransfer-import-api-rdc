// ----------------------------------------------------------------------------
// <copyright file="IoWarningEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;

	/// <summary>
	/// Class for IO warning event arguments.
	/// </summary>
	public sealed class IoWarningEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IoWarningEventArgs"/> class.
		/// </summary>
		/// <param name="message">
		/// The warning message.
		/// </param>
		/// <param name="lineNumber">
		/// The line number.
		/// </param>
		public IoWarningEventArgs(string message, long lineNumber)
		{
			this.Message = message;
			this.CurrentLineNumber = lineNumber;
			this.WarningType = IoWarningType.Message;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IoWarningEventArgs"/> class.
		/// </summary>
		/// <param name="waitTime">
		/// The wait time in seconds.
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="lineNumber">
		/// The line number.
		/// </param>
		public IoWarningEventArgs(int waitTime, Exception exception, long lineNumber)
		{
			this.WaitTime = waitTime;
			this.Exception = exception;
			this.CurrentLineNumber = lineNumber;
			this.WarningType = waitTime > 0 ? IoWarningType.WaitRetryError : IoWarningType.InstantRetry;
		}

		/// <summary>
		/// Gets the current line number.
		/// </summary>
		/// <value>
		/// The line number.
		/// </value>
		public long CurrentLineNumber { get; }

		/// <summary>
		/// Gets the exception.
		/// </summary>
		/// <value>
		/// The <see cref="Exception"/> instance.
		/// </value>
		public Exception Exception { get; }

		/// <summary>
		/// Gets the warning message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public string Message { get; }

		/// <summary>
		/// Gets the wait time in seconds.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		public int WaitTime { get; }

		/// <summary>
		/// Gets the warning type.
		/// </summary>
		/// <value>
		/// The <see cref="IoWarningType"/> value.
		/// </value>
		public IoWarningType WarningType { get; }
	}
}
