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
	using System.IO;
	using System.Threading;

	using Relativity.Import.Export.Io;
	using Relativity.Logging;

	/// <summary>
	/// Represents a mock <see cref="DelimitedFileImporter" /> implementation for testing purposes.
	/// </summary>
	public class MockDelimitedFileImport : DelimitedFileImporter
	{
		public MockDelimitedFileImport()
			: base(
				",",
				"\"",
				System.Convert.ToString((char)10),
				new IoReporterContext(),
				new NullLogger(),
				CancellationToken.None)
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
					var lines = new System.Collections.ArrayList();
					while (!this.HasReachedEof)
					{
						string[] line = this.GetLine();
						lines.AddRange(line);
					}

					return lines;
				}
				finally
				{
					this.Reader.Close();
				}
			}
		}
	}
}