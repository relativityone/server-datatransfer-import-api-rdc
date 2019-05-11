// -----------------------------------------------------------------------------------------------------
// <copyright file="FromFileToLoadFileWriterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.IO;
	using System.Text;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.Logging;

	[TestFixture]
	public class FromFileToLoadFileWriterTests
	{
		private const char _QUOTE_DELIMITER = 'Q';
		private const char _NEWLINE_DELIMITER = 'N';
		private FromFileToLoadFileWriter _instance;
		private MemoryStream _memoryStream;
		private StreamWriter _streamWriter;
		private string _filePath;

		[SetUp]
		public void SetUp()
		{
			this._filePath = Path.GetTempFileName();

			this._memoryStream = new MemoryStream();
			this._streamWriter = new StreamWriter(this._memoryStream, Encoding.Default);

			ExportFile exportSettings = new ExportFile(1)
			{
				QuoteDelimiter = _QUOTE_DELIMITER,
				NewlineDelimiter = _NEWLINE_DELIMITER
			};
			this._instance = new FromFileToLoadFileWriter(new NullLogger(), new DelimitedFileLongTextStreamFormatterFactory(exportSettings));
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(this._filePath))
			{
				File.Delete(this._filePath);
			}
		}

		[Test]
		[TestCase("text_to_write", "text_to_write")]
		[TestCase("text_with_Q_quote", "text_with_QQ_quote")]
		[TestCase("text_with_\n_new_line", "text_with_N_new_line")]
		[TestCase("", "")]
		public void ItShouldWriteFormattedText(string text, string expectedResult)
		{
			File.WriteAllText(this._filePath, text, Encoding.Default);

			// ACT
			this._instance.WriteLongTextFileToDatFile(this._streamWriter, this._filePath, Encoding.Default);

			// ASSERT
			string actualResult = this.GetWrittenText();
			Assert.That(actualResult, Is.EqualTo(expectedResult));
		}

		private string GetWrittenText()
		{
			this._streamWriter.Flush();
			return Encoding.Default.GetString(this._memoryStream.ToArray());
		}
	}
}