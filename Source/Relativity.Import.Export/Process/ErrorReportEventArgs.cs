// ----------------------------------------------------------------------------
// <copyright file="ErrorReportEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents the error report event argument data. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class ErrorReportEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorReportEventArgs"/> class.
		/// </summary>
		/// <param name="error">
		/// The dictionary containing the error information.
		/// </param>
		public ErrorReportEventArgs(IDictionary error)
		{
			this.Error = error;
		}

		/// <summary>
		/// Gets the dictionary containing the error information.
		/// </summary>
		/// <value>
		/// The <see cref="IDictionary"/> instance.
		/// </value>
		public IDictionary Error
		{
			get;
		}
	}
}