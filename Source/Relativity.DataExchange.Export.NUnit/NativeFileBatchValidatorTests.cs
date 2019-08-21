// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeFileBatchValidatorTests.cs" company="Relativity ODA LLC">
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
	using Relativity.Logging;

	[TestFixture]
	public class NativeFileBatchValidatorTests
	{
		private const int _FILE_SIZE = 550400;

		protected IBatchValidator Instance { get; private set; }

		protected Mock<IErrorFileWriter> ErrorFileWriter { get; private set; }

		protected Mock<IFile> FileHelper { get; private set; }

		protected Mock<IAppSettings> AppSettings { get; private set; }

		protected Mock<IStatus> Status { get; private set; }

		protected Mock<ILog> Logger { get; private set; }

		[SetUp]
		public void SetUp()
		{
			this.AppSettings = new Mock<IAppSettings>();
			this.ErrorFileWriter = new Mock<IErrorFileWriter>();
			this.FileHelper = new Mock<IFile>();
			this.Status = new Mock<IStatus>();
			this.Logger = new Mock<ILog>();
			this.Instance = this.CreateValidator();
		}

		protected virtual IBatchValidator CreateValidator()
		{
			return new NativeFileBatchValidator(
				this.ErrorFileWriter.Object,
				this.FileHelper.Object,
				this.AppSettings.Object,
				this.Logger.Object);
		}

		[Test]
		public void ItShouldSkipArtifactWithoutNative()
		{
			var artifact = new ObjectExportInfo
			{
				NativeTempLocation = string.Empty,
				IdentifierValue = "id"
			};
			ObjectExportInfo[] artifacts =
			{
				artifact
			};

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Never);
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldPassForNativeWithValidSize()
		{
			var artifact = new ObjectExportInfo
			{
				NativeTempLocation = "file_path"
			};
			ObjectExportInfo[] artifacts =
			{
				artifact
			};

			this.FileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(_FILE_SIZE);

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Never);
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteWarningForFileWithInvalidSize()
		{
			var artifact = new ObjectExportInfo
			{
				NativeTempLocation = "file_path"
			};
			ObjectExportInfo[] artifacts =
			{
				artifact
			};

			this.FileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(_FILE_SIZE - 1);

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Never);
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteErrorForMissingFile()
		{
			var artifact = new ObjectExportInfo
			{
				NativeTempLocation = "file_path"
			};
			ObjectExportInfo[] artifacts =
			{
				artifact
			};

			this.AppSettings.SetupGet(x => x.CreateErrorForEmptyNativeFile).Returns(true);
			this.FileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(false);
			this.FileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(1);

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType
						.Native,
					artifact.IdentifierValue,
					artifact.NativeTempLocation,
					It.IsAny<string>()),
				Times.Once);
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldNotWriteErrorForEmptyFileWhenConfigured(bool createErrorForEmptyNativeFile)
		{
			var artifact = new ObjectExportInfo
				               {
					               NativeTempLocation = "file_path"
				               };
			ObjectExportInfo[] artifacts =
				{
					artifact
				};

			this.AppSettings.SetupGet(x => x.CreateErrorForEmptyNativeFile).Returns(createErrorForEmptyNativeFile);
			this.FileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(0);

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType
						.Native,
					artifact.IdentifierValue,
					artifact.NativeTempLocation,
					It.IsAny<string>()),
				createErrorForEmptyNativeFile ? Times.Once() : Times.Never());
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void ItShouldWriteErrorForInvalidNativeSourceLocation(string location)
		{
			// ARRANGE
			ObjectExportInfo artifact =
				new ObjectExportInfo { NativeTempLocation = "file_path", NativeSourceLocation = location };
			ObjectExportInfo[] artifacts = { artifact };

			this.FileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(false);
			this.FileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(0);

			// ACT
			this.Instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType
						.Native,
					artifact.IdentifierValue,
					artifact.NativeTempLocation,
					It.IsAny<string>()),
				Times.Once);
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}
	}
}