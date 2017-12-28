using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
	[TestFixture]
	public class ExportFileValidatorTests
	{
		private ExportFileValidator _instance;

		private ExportFile _exportSettings;

		private Mock<IStatus> _status;
		private Mock<IFileHelper> _fileHelper;

		[SetUp]
		public void SetUp()
		{
			_exportSettings = new ExportFile(1);

			_status = new Mock<IStatus>();
			_fileHelper = new Mock<IFileHelper>();

			_instance = new ExportFileValidator(_exportSettings, _status.Object, _fileHelper.Object, new NullLogger());
		}

		[Test]
		[TestCase(true, true, ExpectedResult = true)]
		[TestCase(false, true, ExpectedResult = true)]
		[TestCase(false, false, ExpectedResult = true)]
		[TestCase(true, false, ExpectedResult = false)]
		public bool ItShouldReturnAppropriateResult(bool fileExists, bool overwrite)
		{
			const string filePath = "file_path";

			_fileHelper.Setup(x => x.Exists(filePath)).Returns(fileExists);
			_exportSettings.Overwrite = overwrite;

			//ACT & ASSERT
			return _instance.CanExport(filePath, string.Empty);
		}

		[Test]
		public void ItShouldWriteWarningWhenFileExistsAndNotOverwriting()
		{
			const string filePath = "file_path";

			_fileHelper.Setup(x => x.Exists(filePath)).Returns(true);
			_exportSettings.Overwrite = false;

			//ACT
			_instance.CanExport(filePath, string.Empty);

			//ASSERT
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()));
		}

		[Test]
		public void ItShouldWriteStatusAndDeleteFileWhenExistsAndOverwriting()
		{
			const string filePath = "file_path";

			_fileHelper.Setup(x => x.Exists(filePath)).Returns(true);
			_exportSettings.Overwrite = true;

			//ACT
			_instance.CanExport(filePath, string.Empty);

			//ASSERT
			_status.Verify(x => x.WriteStatusLine(EventType.Status, It.IsAny<string>(), false));
			_fileHelper.Verify(x => x.Delete(filePath));
		}
	}
}