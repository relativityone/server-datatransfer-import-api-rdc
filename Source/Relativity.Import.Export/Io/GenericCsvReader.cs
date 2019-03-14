// ----------------------------------------------------------------------------
// <copyright file="GenericCsvReader.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;
	using System.IO;
	using System.Text;
	using System.Threading;

	using Microsoft.VisualBasic.CompilerServices;

	using Relativity.Logging;

	/// <summary>
	/// Represents an exception that occured while attempting to import data.
	/// </summary>
	internal class GenericCsvReader : DelimitedFileImporter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericCsvReader"/> class.
		/// </summary>
		/// <param name="file">
		/// The full path to the CSV file.
		/// </param>
		/// <param name="retry">
		/// Specify whether retry behavior is required. This flag was added for backwards compatibility with legacy code.
		/// </param>
		public GenericCsvReader(string file, bool retry)
			: this(file, Encoding.Default, retry)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericCsvReader"/> class.
		/// </summary>
		/// <param name="file">
		/// The full path to the CSV file.
		/// </param>
		/// <param name="encoding">
		/// The encoding of the file.
		/// </param>
		/// <param name="retry">
		/// Specify whether retry behavior is required. This flag was added for backwards compatibility with legacy code.
		/// </param>
		public GenericCsvReader(string file, Encoding encoding, bool retry)
			: this(
				file,
				encoding,
				new IoReporterContext { RetryOptions = retry ? RetryOptions.Io : RetryOptions.None },
				new NullLogger(),
				CancellationToken.None)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericCsvReader"/> class.
		/// </summary>
		/// <param name="file">
		/// The full path to the CSV file.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		public GenericCsvReader(string file, ILog logger, CancellationToken token)
			: this(file, Encoding.Default, new IoReporterContext(), logger, token)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericCsvReader"/> class.
		/// </summary>
		/// <param name="file">
		/// The full path to the CSV file.
		/// </param>
		/// <param name="context">
		/// The I/O reporter context.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		public GenericCsvReader(string file, IoReporterContext context, ILog logger, CancellationToken token)
			: this(file, Encoding.Default, context, logger, token)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericCsvReader"/> class.
		/// </summary>
		/// <param name="file">
		/// The full path to the CSV file.
		/// </param>
		/// <param name="encoding">
		/// The encoding of the file.
		/// </param>
		/// <param name="context">
		/// The I/O reporter context.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		public GenericCsvReader(
			string file,
			System.Text.Encoding encoding,
			IoReporterContext context,
			ILog logger,
			CancellationToken token)
			: base(",", "\"", Conversions.ToString(Microsoft.VisualBasic.Strings.ChrW(10)), context, logger, token)
		{
			this.Reader = new StreamReader(file, encoding);
		}

		/// <summary>
		/// Gets a value indicating whether the end of the file has been reached.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the end of the file has been reached; otherwise, <see langword="false" />.
		/// </value>
		public bool Eof => this.Reader.Peek() == -1;

		/// <summary>
		/// Reads the next line of the file.
		/// </summary>
		/// <returns>
		/// The next line of the file.
		/// </returns>
		public string[] ReadLine()
		{
			if (this.Reader.Peek() == -1)
			{
				return null;
			}

			return this.GetLine();
		}

		/// <inheritdoc />
		public override object ReadFile(string path)
		{
			throw new NotSupportedException("This function is deprecated");
		}
	}
}