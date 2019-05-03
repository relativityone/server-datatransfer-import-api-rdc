// -----------------------------------------------------------------------------------------------------
// <copyright file="MockDelimitedFileImport.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a mock <see cref="DelimitedFileImporter" /> implementation for testing purposes.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System.Collections.Generic;
	using System.IO;
	using System.Threading;

	using Relativity.Import.Export.Data;
	using Relativity.Import.Export.Io;
	using Relativity.Logging;

	/// <summary>
	/// Represents a mock <see cref="DelimitedFileImporter" /> implementation for testing purposes.
	/// </summary>
	internal class MockDelimitedFileImport : DelimitedFileImporter
	{
		public const char DefaultDelimiter = ',';

		public const char DefaultBound = '\"';

		public const char DefaultNewline = (char)10;

		public MockDelimitedFileImport()
			: this(DefaultDelimiter.ToString(), DefaultBound.ToString(), DefaultNewline.ToString())
		{
		}

		public MockDelimitedFileImport(string delimiter)
			: this(delimiter, DefaultBound.ToString())
		{
		}

		public MockDelimitedFileImport(string delimiter, string bound)
			: this(
				delimiter,
				bound,
				DefaultNewline.ToString(),
				new IoReporterContext(),
				new NullLogger(),
				CancellationToken.None)
		{
		}

		public MockDelimitedFileImport(string delimiter, string bound, string newlineProxy)
			: this(delimiter, bound, newlineProxy, new IoReporterContext(), new NullLogger(), CancellationToken.None)
		{
		}

		public MockDelimitedFileImport(
			string delimiter,
			string bound,
			string newlineProxy,
			IoReporterContext context,
			Relativity.Logging.ILog logger,
			CancellationToken token)
			: base(delimiter, bound, newlineProxy, context, logger, token)
		{
			this.TrimOption = TrimOption.Both;
		}

		public MockDelimitedFileImport(
			char delimiter,
			char bound,
			char newlineProxy,
			IoReporterContext context,
			Relativity.Logging.ILog logger,
			CancellationToken token)
			: base(delimiter, bound, newlineProxy, context, logger, token)
		{
			this.TrimOption = TrimOption.Both;
		}

		protected override bool UseConcordanceStyleBoundStart => false;

		public override object ReadFile(string path)
		{
			using (var stream = new System.IO.MemoryStream(System.Text.Encoding.Unicode.GetBytes(path)))
			{
				this.Reader = new StreamReader(stream, System.Text.Encoding.Unicode);

				try
				{
					var lines = new List<string[]>();
					while (!this.HasReachedEof)
					{
						string[] line = this.GetLine();
						lines.Add(line);
					}

					return lines;
				}
				finally
				{
					this.Reader.Close();
				}
			}
		}

		public void SetTrim(TrimOption trimOption)
		{
			this.TrimOption = trimOption;
		}
	}
}