// ----------------------------------------------------------------------------
// <copyright file="ProcessErrorReport.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System.Data;

	/// <summary>
	/// Represents the show report event argument data.
	/// </summary>
	public sealed class ProcessErrorReport
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessErrorReport"/> class.
		/// </summary>
		public ProcessErrorReport()
		{
			this.Report = null;
			this.MaxLengthExceeded = false;
		}

		/// <summary>
		/// Gets or sets the report data source.
		/// </summary>
		/// <value>
		/// The <see cref="DataTable"/> instance.
		/// </value>
		public DataTable Report
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the max length has been exceeded.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when the max length has been exceeded; otherwise, <see langword="false" />.
		/// </value>
		public bool MaxLengthExceeded
		{
			get;
			set;
		}
	}
}