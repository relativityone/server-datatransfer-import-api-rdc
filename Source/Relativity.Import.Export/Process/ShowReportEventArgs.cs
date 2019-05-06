// ----------------------------------------------------------------------------
// <copyright file="ShowReportEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;
	using System.Data;

	/// <summary>
	/// Represents the show report event argument data. This class cannot be inherited.
	/// </summary>
	public sealed class ShowReportEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ShowReportEventArgs"/> class.
		/// </summary>
		/// <param name="report">
		/// The report data source.
		/// </param>
		/// <param name="maxLengthExceeded">
		/// The value indicating whether the max length has been exceeded.
		/// </param>
		public ShowReportEventArgs(DataTable report, bool maxLengthExceeded)
		{
			if (report == null)
			{
				throw new ArgumentNullException(nameof(report));
			}

			this.Report = report;
			this.MaxLengthExceeded = maxLengthExceeded;
		}

		/// <summary>
		/// Gets the report data source.
		/// </summary>
		/// <value>
		/// The <see cref="DataTable"/> instance.
		/// </value>
		public DataTable Report
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the max length has been exceeded.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when the max length has been exceeded; otherwise, <see langword="false" />.
		/// </value>
		public bool MaxLengthExceeded
		{
			get;
		}
	}
}