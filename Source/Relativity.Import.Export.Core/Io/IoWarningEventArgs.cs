// ----------------------------------------------------------------------------
// <copyright file="IoWarningEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;

	/// <summary>
	/// Class for IO warning event arguments
	/// </summary>
	public class IoWarningEventArgs : EventArgs
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
		}

		/// <summary>
		/// Gets the current line number
		/// </summary>
		/// <value>
		/// The line number.
		/// </value>
		public long CurrentLineNumber { get; }

		/// <summary>
		/// Gets the warning message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public string Message { get; }
	}
}
