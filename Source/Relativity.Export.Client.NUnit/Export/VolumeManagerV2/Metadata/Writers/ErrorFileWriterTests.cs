using System;
using System.IO;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Writers
{
	[TestFixture]
	public class ErrorFileWriterTests
	{
		private ErrorFileWriter _instance;

		private MemoryStream _memoryStream;
		private StreamWriter _streamWriter;

		private Mock<IStatus> _status;

		private const string _HEADER = "\"File Type\",\"Document Identifier\",\"File Guid\",\"Error Description\"";
		private const string _ERROR_LINE = "\"{0}\",\"record_identifier\",\"file_location\",\"error_text\"";

		[SetUp]
		public void SetUp()
		{
			const string errorFilePath = "error_file_path";
			_memoryStream = new MemoryStream();
			_streamWriter = new StreamWriter(_memoryStream, Encoding.Default);

			Mock<IStreamFactory> streamFactory = new Mock<IStreamFactory>();
			streamFactory.Setup(x => x.Create(It.IsAny<StreamWriter>(), It.IsAny<long>(), errorFilePath, Encoding.Default, It.IsAny<bool>())).Returns(_streamWriter);

			Mock<IDestinationPath> destinationPath = new Mock<IDestinationPath>();
			destinationPath.Setup(x => x.Path).Returns(errorFilePath);
			destinationPath.Setup(x => x.Encoding).Returns(Encoding.Default);

			_status = new Mock<IStatus>();
			_instance = new ErrorFileWriter(streamFactory.Object, destinationPath.Object, _status.Object, new NullLogger());
		}

		[Test]
		[TestCase(ErrorFileWriter.ExportFileType.Generic)]
		[TestCase(ErrorFileWriter.ExportFileType.Image)]
		[TestCase(ErrorFileWriter.ExportFileType.Native)]
		public void ItShouldWriteError(ErrorFileWriter.ExportFileType type)
		{
			const string recordIdentifier = "record_identifier";
			const string fileLocation = "file_location";
			const string errorText = "error_text";

			//ACT
			_instance.Write(type, recordIdentifier, fileLocation, errorText);
			_instance.Write(type, recordIdentifier, fileLocation, errorText);

			//ASSERT
			string expectedText = $"{_HEADER}{Environment.NewLine}{string.Format(_ERROR_LINE, type)}{Environment.NewLine}{string.Format(_ERROR_LINE, type)}{Environment.NewLine}";
			string writtenText = GetWrittenText();
			Assert.That(writtenText, Is.EqualTo(expectedText));

			_status.Verify(x => x.WriteError($"{type} - Document [{recordIdentifier}] - File [{fileLocation}] - Error: {Environment.NewLine}{errorText}"), Times.Exactly(2));
		}

		private string GetWrittenText()
		{
			_streamWriter.Flush();
			return Encoding.Default.GetString(_memoryStream.ToArray());
		}
	}
}