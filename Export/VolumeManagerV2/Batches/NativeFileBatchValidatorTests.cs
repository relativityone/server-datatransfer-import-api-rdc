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
		private NativeFileBatchValidator _instance;
		private Mock<IErrorFileWriter> _errorFileWriter;
		private Mock<IFileHelper> _fileHelper;
		private Mock<IStatus> _status;

		private const int _FILE_SIZE = 550400;

		[SetUp]
		public void SetUp()
		{
			_errorFileWriter = new Mock<IErrorFileWriter>();
			_fileHelper = new Mock<IFileHelper>();
			_status = new Mock<IStatus>();

			_instance = new NativeFileBatchValidator(_errorFileWriter.Object, _fileHelper.Object, _status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldSkipArtifactWithoutNative()
		{
			var artifact = new ObjectExportInfo
			{
				NativeTempLocation = string.Empty,
				IdentifierValue = "id"
			};
			ObjectExportInfo[] aritfacts =
			{
				artifact
			};

			//ACT
			_instance.ValidateExportedBatch(aritfacts, new VolumePredictions[1], CancellationToken.None);

			//ASSERT
			_errorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Never);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldPassForNativeWithValidSize()
		{
			var artifact = new ObjectExportInfo
			{
				NativeTempLocation = "file_path"
			};
			ObjectExportInfo[] aritfacts =
			{
				artifact
			};

			var prediction = new VolumePredictions
			{
				NativeFilesSize = _FILE_SIZE
			};
			VolumePredictions[] predictions = {prediction};

			_fileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(true);
			_fileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(_FILE_SIZE);

			//ACT
			_instance.ValidateExportedBatch(aritfacts, predictions, CancellationToken.None);

			//ASSERT
			_errorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Never);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteWarningForFileWithInvalidSize()
		{
			var artifact = new ObjectExportInfo
			{
				NativeTempLocation = "file_path"
			};
			ObjectExportInfo[] aritfacts =
			{
				artifact
			};

			var prediction = new VolumePredictions
			{
				NativeFilesSize = _FILE_SIZE
			};
			VolumePredictions[] predictions = { prediction };

			_fileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(true);
			_fileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(_FILE_SIZE - 1);

			//ACT
			_instance.ValidateExportedBatch(aritfacts, predictions, CancellationToken.None);

			//ASSERT
			_errorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Never);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Once);
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
			ObjectExportInfo[] aritfacts =
			{
				artifact
			};

			_fileHelper.Setup(x => x.Exists(artifact.NativeTempLocation)).Returns(exists);
			_fileHelper.Setup(x => x.GetFileSize(artifact.NativeTempLocation)).Returns(size);

			//ACT
			_instance.ValidateExportedBatch(aritfacts, new VolumePredictions[1], CancellationToken.None);

			//ASSERT
			_errorFileWriter.Verify(x => x.Write(ErrorFileWriter.ExportFileType.Native, artifact.IdentifierValue, artifact.NativeTempLocation, It.IsAny<string>()), Times.Once);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}
	}
}