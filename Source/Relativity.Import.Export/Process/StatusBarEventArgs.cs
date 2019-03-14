// ----------------------------------------------------------------------------
// <copyright file="StatusBarEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents the status bar event argument data.
	/// </summary>
	[Serializable]
	public sealed class StatusBarEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StatusBarEventArgs"/> class.
		/// </summary>
		/// <param name="message">
		/// The status bar message
		/// </param>
		/// <param name="popupText">
		/// The status bar popup text.
		/// </param>
		public StatusBarEventArgs(string message, string popupText)
		{
			this.Message = message;
			this.PopupText = popupText;
		}

		/// <summary>
		/// Gets the status bar message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public string Message { get; }

		/// <summary>
		/// Gets the status bar popup text.
		/// </summary>
		/// <value>
		/// The popup text.
		/// </value>
		public string PopupText { get; }
	}
}