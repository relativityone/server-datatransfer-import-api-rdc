using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Batches
{
	[TestFixture]
	public class NativeFileBatchValidatorTests
	{
		protected IBatchValidator Instance { get; private set; }
		protected Mock<IErrorFileWriter> ErrorFileWriter { get; private set; }
		protected Mock<IFileHelper> FileHelper { get; private set; }
		protected Mock<IStatus> Status { get; private set; }

		private const int _FILE_SIZE = 550400;

		[SetUp]
		public void SetUp()
		{
			ErrorFileWriter = new Mock<IErrorFileWriter>();
			FileHelper = new Mock<IFileHelper>();
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

			//ACT
			Instance.ValidateExportedBatch(artifacts, new VolumePredictions[1], CancellationToken.None);

			//ASSERT
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
			VolumePredictions[] predictions = {prediction};

			FileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(true);
			FileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(_FILE_SIZE);

			//ACT
			Instance.ValidateExportedBatch(artifacts, predictions, CancellationToken.None);

			//ASSERT
			ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Never);
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[Ignore("Until File Size calculation is fixed by Production team REL-198994")]
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
			VolumePredictions[] predictions = {prediction};

			FileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(true);
			FileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(_FILE_SIZE - 1);

			//ACT
			Instance.ValidateExportedBatch(artifacts, predictions, CancellationToken.None);

			//ASSERT
			ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Never);
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Once);
		}

		[Test]
		[Ignore("Until File Size calculation is fixed by Production team REL-198994")]
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

			//ACT
			Instance.ValidateExportedBatch(artifacts, new VolumePredictions[1], CancellationToken.None);

			//ASSERT
			ErrorFileWriter.Verify(x => x.Write(Core.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType.Native, artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Once);
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}
	}
}