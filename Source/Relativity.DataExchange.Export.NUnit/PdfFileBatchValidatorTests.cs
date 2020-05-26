// -----------------------------------------------------------------------------------------------------
// <copyright file="PdfFileBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class PdfFileBatchValidatorTests
	{
		private const int _FILE_SIZE = 550400;

		protected IBatchValidator Instance { get; private set; }

		protected Mock<IErrorFileWriter> ErrorFileWriter { get; private set; }

		protected Mock<IFile> FileHelper { get; private set; }

		protected Mock<IAppSettings> AppSettings { get; private set; }

		protected Mock<IStatus> Status { get; private set; }

		protected TestNullLogger Logger { get; private set; }

		[SetUp]
		public void SetUp()
		{
			this.AppSettings = new Mock<IAppSettings>();
			this.ErrorFileWriter = new Mock<IErrorFileWriter>();
			this.FileHelper = new Mock<IFile>();
			this.Status = new Mock<IStatus>();
			this.Logger = new TestNullLogger();
			this.Instance = this.CreateValidator();
		}

		protected virtual IBatchValidator CreateValidator()
		{
			return new PdfFileBatchValidator(
				this.ErrorFileWriter.Object,
				this.FileHelper.Object,
				this.AppSettings.Object,
				this.Logger);
		}

		[Test]
		public void ItShouldSkipArtifactWithoutPdf()
		{
			var artifact = new ObjectExportInfo
			{
				PdfDestinationLocation = string.Empty,
				IdentifierValue = "id"
			};
			ObjectExportInfo[] artifacts =
			{
				artifact
			};

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact, artifact.PdfDestinationLocation, It.IsAny<string>()), Times.Never);
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldPassForPdfWithValidSize()
		{
			var artifact = new ObjectExportInfo
			{
				PdfDestinationLocation = "file_path"
			};
			ObjectExportInfo[] artifacts =
			{
				artifact
			};

			this.FileHelper.Setup(x => x.Exists(artifact.PdfDestinationLocation)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(artifact.PdfDestinationLocation)).Returns(_FILE_SIZE);

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact, artifact.PdfDestinationLocation, It.IsAny<string>()), Times.Never);
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteWarningForFileWithInvalidSize()
		{
			var artifact = new ObjectExportInfo
			{
				PdfDestinationLocation = "file_path"
			};
			ObjectExportInfo[] artifacts =
			{
				artifact
			};

			this.FileHelper.Setup(x => x.Exists(artifact.PdfDestinationLocation)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(artifact.PdfDestinationLocation)).Returns(_FILE_SIZE - 1);

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact, artifact.PdfDestinationLocation, It.IsAny<string>()), Times.Never);
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteErrorForMissingFile()
		{
			var artifact = new ObjectExportInfo
			{
				PdfDestinationLocation = "file_path"
			};
			ObjectExportInfo[] artifacts =
			{
				artifact
			};

			this.AppSettings.SetupGet(x => x.CreateErrorForEmptyPdfFile).Returns(true);
			this.FileHelper.Setup(x => x.Exists(artifact.PdfDestinationLocation)).Returns(false);
			this.FileHelper.Setup(x => x.GetFileSize(artifact.PdfDestinationLocation)).Returns(1);

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType
						.Pdf,
					artifact,
					artifact.PdfDestinationLocation,
					It.IsAny<string>()),
				Times.Once);
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldNotWriteErrorForEmptyFileWhenConfigured(bool createErrorForEmptyPdfFile)
		{
			var artifact = new ObjectExportInfo
			{
				PdfDestinationLocation = "file_path"
			};
			ObjectExportInfo[] artifacts =
				{
					artifact
				};

			this.AppSettings.SetupGet(x => x.CreateErrorForEmptyPdfFile).Returns(createErrorForEmptyPdfFile);
			this.FileHelper.Setup(x => x.Exists(artifact.PdfDestinationLocation)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(artifact.PdfDestinationLocation)).Returns(0);

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType
						.Pdf,
					artifact,
					artifact.PdfDestinationLocation,
					It.IsAny<string>()),
				createErrorForEmptyPdfFile ? Times.Once() : Times.Never());
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void ItShouldWriteErrorForInvalidPdfSourceLocation(string location)
		{
			// ARRANGE
			ObjectExportInfo artifact =
				new ObjectExportInfo { PdfDestinationLocation = "file_path", PdfSourceLocation = location };
			ObjectExportInfo[] artifacts = { artifact };

			this.FileHelper.Setup(x => x.Exists(artifact.PdfDestinationLocation)).Returns(false);
			this.FileHelper.Setup(x => x.GetFileSize(artifact.PdfDestinationLocation)).Returns(0);

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType
						.Pdf,
					artifact,
					artifact.PdfDestinationLocation,
					It.IsAny<string>()),
				Times.Once);
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}
	}
}
