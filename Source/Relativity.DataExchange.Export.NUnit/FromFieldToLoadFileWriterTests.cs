// -----------------------------------------------------------------------------------------------------
// <copyright file="FromFieldToLoadFileWriterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System.IO;
    using System.Text;

    using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.Logging;

    [TestFixture]
	public class FromFieldToLoadFileWriterTests
	{
		private const char _QUOTE_DELIMITER = 'Q';
		private const char _NEWLINE_DELIMITER = 'N';
		private FromFieldToLoadFileWriter _instance;
		private MemoryStream _memoryStream;
		private StreamWriter _streamWriter;

		[SetUp]
		public void SetUp()
		{
			_memoryStream = new MemoryStream();
			_streamWriter = new StreamWriter(_memoryStream, Encoding.Default);

			ExportFile exportSettings = new ExportFile(1)
			{
				QuoteDelimiter = _QUOTE_DELIMITER,
				NewlineDelimiter = _NEWLINE_DELIMITER
			};
			_instance = new FromFieldToLoadFileWriter(new NullLogger(), new DelimitedFileLongTextStreamFormatterFactory(exportSettings));
		}

		[Test]
		[TestCase("text_to_write", "text_to_write")]
		[TestCase("text_with_Q_quote", "text_with_QQ_quote")]
		[TestCase("text_with_\n_new_line", "text_with_N_new_line")]
		[TestCase("", "")]
		public void ItShouldWriteFormattedText(string text, string expectedResult)
		{
			// ACT
			_instance.WriteLongTextFileToDatFile(_streamWriter, text, Encoding.Default);

			// ASSERT
			string actualResult = GetWrittenText();
			Assert.That(actualResult, Is.EqualTo(expectedResult));
		}

		private string GetWrittenText()
		{
			_streamWriter.Flush();
			return Encoding.Default.GetString(_memoryStream.ToArray());
		}
	}
}