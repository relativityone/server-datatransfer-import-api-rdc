// ----------------------------------------------------------------------------
// <copyright file="ProcessErrorWriter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Threading;

	using Relativity.Import.Export.Io;

	/// <summary>
	/// Represents a class object that writes all process errors to a CSV file.
	/// </summary>
	internal sealed class ProcessErrorWriter : IProcessErrorWriter
	{
		private readonly Relativity.Logging.ILog logger;
		private string errorsFile;
		private System.IO.StreamWriter streamWriter;
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessErrorWriter"/> class.
		/// </summary>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="logger"/> is <see langword="null" />.
		/// </exception>
		public ProcessErrorWriter(Relativity.Logging.ILog logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			this.logger = logger;
		}

		/// <inheritdoc />
		public bool HasErrors => !string.IsNullOrEmpty(this.errorsFile);

		/// <inheritdoc />
		public ProcessErrorReport BuildErrorReport(CancellationToken token)
		{
			this.Close();

			// Preserve the original implementation - no retry!
			IoReporterContext context = new IoReporterContext { RetryOptions = RetryOptions.None };
			ProcessErrorReader reader = new ProcessErrorReader(context, this.logger, token);
			return reader.ReadFile(this.errorsFile) as ProcessErrorReport;
		}

		/// <inheritdoc />
		public void Close()
		{
			this.streamWriter?.Close();
			this.streamWriter = null;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		public void Write(string key, string description)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (string.IsNullOrEmpty(description))
			{
				throw new ArgumentNullException(nameof(description));
			}

			if (string.IsNullOrEmpty(this.errorsFile))
			{
				this.errorsFile = System.IO.Path.Combine(
					System.IO.Path.GetTempPath(),
					$"RDC_Errors_{Guid.NewGuid()}.tmp");
			}

			if (this.streamWriter?.BaseStream == null)
			{
				this.streamWriter = new StreamWriter(this.errorsFile, false);
			}

			key = key.Replace("\"", "\"\"");
			description = description.Replace("\"", "\"\"");
			this.streamWriter.WriteLine(
				"\"{1}{0}Error{0}{2}{0}{3}\"",
				"\",\"",
				key,
				description,
				DateTime.Now.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing">
		/// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.
		/// </param>
		private void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}

			if (disposing && this.streamWriter != null)
			{
				// Duplicated to avoid CA warning.
				this.streamWriter.Close();
				this.streamWriter = null;
			}

			this.disposed = true;
		}
	}
}