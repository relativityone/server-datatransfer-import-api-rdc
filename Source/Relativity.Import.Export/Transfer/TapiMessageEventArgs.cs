// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiMessageEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines Transfer API message event arguments data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
	using System;

	/// <summary>
	/// Represents Transfer API message event arguments data.
	/// </summary>
	/// <seealso cref="System.EventArgs" />
	public sealed class TapiMessageEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiMessageEventArgs"/> class.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="lineNumber">
		/// The line number.
		/// </param>
		public TapiMessageEventArgs(string message, int lineNumber)
		{
			this.Message = message;
			this.LineNumber = lineNumber;
		}

		/// <summary>
		/// Gets the line number.
		/// </summary>
		/// <value>
		/// The line number.
		/// </value>
		public int LineNumber
		{
			get;
		}

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public string Message
		{
			get;
		}
	}
}