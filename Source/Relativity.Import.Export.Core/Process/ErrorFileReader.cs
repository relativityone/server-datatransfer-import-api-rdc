// ----------------------------------------------------------------------------
// <copyright file="ErrorFileReader.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System.Globalization;
	using System.Threading;

	using Microsoft.VisualBasic.CompilerServices;

	using Relativity.Import.Export.Importer;
	using Relativity.Import.Export.Io;
	using Relativity.Logging;

	/// <summary>
	/// Represents a class object that can read error information from a delimited file.
	/// </summary>
	public class ErrorFileReader : DelimitedFileImporter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorFileReader"/> class.
		/// </summary>
		/// <param name="context">
		/// The I/O reporter context.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="cancellationToken">
		/// The Cancel Token used to stop the process a any requested time.</param>
		public ErrorFileReader(IoReporterContext context, ILog logger, CancellationToken cancellationToken)
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
			table.Columns.Add("Key");
			table.Columns.Add("Status");
			table.Columns.Add("Description");
			table.Columns.Add("Timestamp");
			int totalRows = 0;
			const int MaxRows = 1000;
			while (!this.HasReachedEof && totalRows < MaxRows)
			{
				table.Rows.Add(this.GetLine());
				totalRows++;
			}

			this.Close();
			return new object[2] { table, totalRows >= 1000 };
		}
	}
}