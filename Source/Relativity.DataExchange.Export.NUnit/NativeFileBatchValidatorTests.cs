// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeFileBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
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

		protected Mock<IStatus> Status { get; private set; }

		[SetUp]
		public void SetUp()
		{
			ErrorFileWriter = new Mock<IErrorFileWriter>();
			FileHelper = new Mock<IFile>();
			Status = new Mock<IStatus>();

			Instance = CreateValidator();
		}

		protected virtual IBatchValidator CreateValidator()
		{
			return new NativeFileBatchValidator(ErrorFileWriter.Object, FileHelper.Object, Status.Object, new NullLogger());
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
			Instance.ValidateExportedBatch(artifacts, new VolumePredictions[1], CancellationToken.None);

			// ASSERT
			ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Never);
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
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

			var prediction = new VolumePredictions
			{
				NativeFilesSize = _FILE_SIZE
			};

			VolumePredictions[] predictions = { prediction };

			FileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(true);
			FileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(_FILE_SIZE);

			// ACT
			Instance.ValidateExportedBatch(artifacts, predictions, CancellationToken.None);

			// ASSERT
			ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Never);
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
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

			var prediction = new VolumePredictions
			{
				NativeFilesSize = _FILE_SIZE
			};

			VolumePredictions[] predictions = { prediction };

			FileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(true);
			FileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(_FILE_SIZE - 1);

			// ACT
			Instance.ValidateExportedBatch(artifacts, predictions, CancellationToken.None);

			// ASSERT
			ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Never);
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(true, 0)]
		[TestCase(false, 1)]
		public void ItShouldWriteErrorForMissingOrEmptyFile(bool exists, long size)
		{
			var artifact = new ObjectExportInfo
			{
				NativeTempLocation = "file_path"
			};
			ObjectExportInfo[] artifacts =
			{
				artifact
			};

			FileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(exists);
			FileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(size);

			// ACT
			Instance.ValidateExportedBatch(artifacts, new VolumePredictions[1], CancellationToken.None);

			// ASSERT
			ErrorFileWriter.Verify(x => x.Write(Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType.Native, artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Once);
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}
	}
}