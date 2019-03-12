// ----------------------------------------------------------------------------
// <copyright file="ProcessErrorReader.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System.Globalization;
	using System.Threading;

	using Microsoft.VisualBasic.CompilerServices;

	using Relativity.Import.Export.Io;
	using Relativity.Logging;

	/// <summary>
	/// Represents a class object that can read error information from a delimited file.
	/// </summary>
	internal class ProcessErrorReader : DelimitedFileImporter
	{
		/// <summary>
		/// The key column name.
		/// </summary>
		public const string ColumnKey = "Key";

		/// <summary>
		/// The status column name.
		/// </summary>
		public const string ColumnStatus = "Status";

		/// <summary>
		/// The description column name.
		/// </summary>
		public const string ColumnDescription = "Description";

		/// <summary>
		/// The timestamp column name.
		/// </summary>
		public const string ColumnTimestamp = "Timestamp";

		/// <summary>
		/// The maximum number of rows.
		/// </summary>
		public const int MaxRows = 1000;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessErrorReader"/> class.
		/// </summary>
		/// <param name="context">
		/// The I/O reporter context.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="cancellationToken">
		/// The Cancel Token used to stop the process a any requested time.</param>
		public ProcessErrorReader(IoReporterContext context, ILog logger, CancellationToken cancellationToken)
			: base(
				",",
				"\"",
				Conversions.ToString(Microsoft.VisualBasic.Strings.ChrW(20)),
				context,
				logger,
				cancellationToken)
		{
		}

		/// <inheritdoc />
		public override object ReadFile(string path)
		{
			this.Reader = new System.IO.StreamReader(path);
			System.Data.DataTable table = new System.Data.DataTable { Locale = CultureInfo.CurrentCulture };
			table.Columns.Add(ColumnKey);
			table.Columns.Add(ColumnStatus);
			table.Columns.Add(ColumnDescription);
			table.Columns.Add(ColumnTimestamp);
			int totalRows = 0;
			while (!this.HasReachedEof && totalRows < MaxRows)
			{
				table.Rows.Add(this.GetLine());
				totalRows++;
			}

			this.Close();
			return new ProcessErrorReport { Report = table, MaxLengthExceeded = totalRows >= 1000 };
		}
	}
}