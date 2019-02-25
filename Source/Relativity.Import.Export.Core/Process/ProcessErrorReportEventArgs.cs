// ----------------------------------------------------------------------------
// <copyright file="ProcessErrorReportEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents the process error report event argument data.
	/// </summary>
	[Serializable]
	public sealed class ProcessErrorReportEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessErrorReportEventArgs"/> class.
		/// </summary>
		/// <param name="error">
		/// The dictionary containing the error information.
		/// </param>
		public ProcessErrorReportEventArgs(IDictionary error)
		{
			this.Error = error;
		}

		/// <summary>
		/// Gets the dictionary containing the error information.
		/// </summary>
		/// <value>
		/// The <see cref="IDictionary"/> instance.
		/// </value>
		public IDictionary Error { get; }
	}
}