// ----------------------------------------------------------------------------
// <copyright file="ProcessCompleteEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents the process complete event argument data.
	/// </summary>
	[Serializable]
	public sealed class ProcessCompleteEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessCompleteEventArgs"/> class.
		/// </summary>
		/// <param name="closeForm">
		/// Specify whether to close any form that started the runnable process.
		/// </param>
		/// <param name="exportFilePath">
		/// The full path to the exported process file.
		/// </param>
		/// <param name="exportLog">
		/// Specify whether logs were exported.
		/// </param>
		public ProcessCompleteEventArgs(bool closeForm, string exportFilePath, bool exportLog)
		{
			this.CloseForm = closeForm;
			this.ExportFilePath = exportFilePath;
			this.ExportLog = exportLog;
		}

		/// <summary>
		/// Gets a value indicating whether to close any form that started the runnable process.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to close the form; otherwise, <see langword="false" />.
		/// </value>
		public bool CloseForm { get; }

		/// <summary>
		/// Gets the full path to the exported process file.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string ExportFilePath { get; }

		/// <summary>
		/// Gets a value indicating whether logs were exported.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when logs were exported; otherwise, <see langword="false" />.
		/// </value>
		public bool ExportLog { get; }
	}
}