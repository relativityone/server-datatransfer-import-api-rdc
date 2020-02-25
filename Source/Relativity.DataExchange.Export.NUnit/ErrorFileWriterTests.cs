// -----------------------------------------------------------------------------------------------------
// <copyright file="ErrorFileWriterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.IO;
	using System.Text;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class ErrorFileWriterTests
	{
		private const string _HEADER = "\"File Type\",\"Document Identifier\",\"File Guid\",\"Error Description\"";
		private const string _ERROR_LINE = "\"{0}\",\"record_identifier\",\"file_location\",\"error_text\"";
		private ErrorFileWriter _instance;
		private MemoryStream _memoryStream;
		private StreamWriter _streamWriter;
		private Mock<IStatus> _status;

		[SetUp]
		public void SetUp()
		{
			const string errorFilePath = "error_file_path";
			this._memoryStream = new MemoryStream();
			this._streamWriter = new StreamWriter(this._memoryStream, Encoding.Default);

			Mock<IStreamFactory> streamFactory = new Mock<IStreamFactory>();
			streamFactory.Setup(x => x.Create(It.IsAny<StreamWriter>(), It.IsAny<long>(), errorFilePath, Encoding.Default, It.IsAny<bool>())).Returns(this._streamWriter);

			Mock<IDestinationPath> destinationPath = new Mock<IDestinationPath>();
			destinationPath.Setup(x => x.Path).Returns(errorFilePath);
			destinationPath.Setup(x => x.Encoding).Returns(Encoding.Default);

			this._status = new Mock<IStatus>();
			this._instance = new ErrorFileWriter(streamFactory.Object, destinationPath.Object, this._status.Object, new TestNullLogger());
		}

		[Test]
		[TestCase(ErrorFileWriter.ExportFileType.Generic, true)]
		[TestCase(ErrorFileWriter.ExportFileType.Image, true)]
		[TestCase(ErrorFileWriter.ExportFileType.Native, true)]
		[TestCase(ErrorFileWriter.ExportFileType.Generic, false)]
		[TestCase(ErrorFileWriter.ExportFileType.Image, false)]
		[TestCase(ErrorFileWriter.ExportFileType.Native, false)]
		public void ItShouldWriteError(ErrorFileWriter.ExportFileType type, bool errorStatusAlreadySent)
		{
			const string recordIdentifier = "record_identifier";
			const string fileLocation = "file_location";
			const string errorText = "error_text";

			// ACT
			this._instance.Write(type, new ObjectExportInfo() { IdentifierValue = recordIdentifier, DocumentError = errorStatusAlreadySent }, fileLocation, errorText);
			this._instance.Write(type, new ObjectExportInfo() { IdentifierValue = recordIdentifier, DocumentError = errorStatusAlreadySent }, fileLocation, errorText);

			// ASSERT
			string expectedText = $"{_HEADER}{Environment.NewLine}{string.Format(_ERROR_LINE, type)}{Environment.NewLine}{string.Format(_ERROR_LINE, type)}{Environment.NewLine}";
			string writtenText = this.GetWrittenText();
			Assert.That(writtenText, Is.EqualTo(expectedText));

			string message =
				$"{type} - Document [{recordIdentifier}] - File [{fileLocation}] - Error: {Environment.NewLine}{errorText}";
			if (errorStatusAlreadySent)
			{
				this._status.Verify(x => x.WriteWarning(message), Times.Exactly(2));
			}
			else
			{
				this._status.Verify(x => x.WriteError(message), Times.Exactly(2));
			}
		}

		private string GetWrittenText()
		{
			this._streamWriter.Flush();
			return Encoding.Default.GetString(this._memoryStream.ToArray());
		}
	}
}