

using System;

namespace kCura.WinEDDS.TApi
{
	/// <summary>
	/// Class for IO warning event arguments
	/// </summary>
	public class IoWarningEventArgs : EventArgs
	{
		/// <summary>
		/// Contructor for IoWarningEventArgs
		/// </summary>
		public IoWarningEventArgs(string message, long currentLineNumber)
		{
			Message = message;
			CurrentLineNumber = currentLineNumber;
		}

		/// <summary>
		/// Current line number
		/// </summary>
		public long CurrentLineNumber { get; }

		/// <summary>
		/// Message
		/// </summary>
		public string Message { get; }
	}
}
