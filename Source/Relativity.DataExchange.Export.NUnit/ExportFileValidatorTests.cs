// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
	using global::NUnit.Framework;

    using kCura.WinEDDS;

    using Moq;

	using Relativity.Export.VolumeManagerV2.Download;
	using Relativity.Export.VolumeManagerV2.Repository;
	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.Process;
	using Relativity.Logging;

    [TestFixture]
	public class ExportFileValidatorTests
	{
		private ExportFileValidator _instance;

		private ExportFile _exportSettings;

		private Mock<IStatus> _status;
		private Mock<IFile> _fileHelper;
		private Mock<IExportRequestRepository> _exportRequestRepository;

		[SetUp]
		public void SetUp()
		{
			_exportSettings = new ExportFile(1);

			_status = new Mock<IStatus>();
			_fileHelper = new Mock<IFile>();
			_exportRequestRepository = new Mock<IExportRequestRepository>();

			_instance = new ExportFileValidator(_exportSettings, _exportRequestRepository.Object, _status.Object, _fileHelper.Object, new NullLogger());
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

			// ACT & ASSERT
			return _instance.CanExport(filePath, string.Empty);
		}

		[Test]
		public void ItShouldWriteWarningWhenFileExistsAndNotOverwriting()
		{
			const string filePath = "file_path";

			_fileHelper.Setup(x => x.Exists(filePath)).Returns(true);
			_exportSettings.Overwrite = false;

			// ACT
			_instance.CanExport(filePath, string.Empty);

			// ASSERT
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()));
		}

		[Test]
		public void ItShouldWriteStatusAndDeleteFileWhenExistsAndOverwriting()
		{
			const string filePath = "file_path";

			_fileHelper.Setup(x => x.Exists(filePath)).Returns(true);
			_exportSettings.Overwrite = true;

			// ACT
			_instance.CanExport(filePath, string.Empty);

			// ASSERT
			_status.Verify(x => x.WriteStatusLine(EventType2.Status, It.IsAny<string>(), false));
			_fileHelper.Verify(x => x.Delete(filePath));
		}

		[Test]
		public void ItShouldWriteWarningWhenExportRequestExistsAndNotOverwriting()
		{
			const string filePath = "file_path";

			_exportRequestRepository.Setup(repository => repository.AnyRequestForLocation(filePath)).Returns(true);
			_exportSettings.Overwrite = false;

			// ACT
			bool canExport = _instance.CanExport(filePath, string.Empty);

			// ASSERT
			Assert.That(canExport, Is.False);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()));
		}

		[Test]
		public void ItShouldWriteStatusWhenExportRequestExistsAndOverwriting()
		{
			const string filePath = "file_path";

			_exportRequestRepository.Setup(repository => repository.AnyRequestForLocation(filePath)).Returns(true);
			_exportSettings.Overwrite = true;

			// ACT
			bool canExport = _instance.CanExport(filePath, string.Empty);

			// ASSERT
			Assert.That(canExport, Is.False);
			_status.Verify(x => x.WriteStatusLine(EventType2.Status, It.IsAny<string>(), false));
			_fileHelper.Verify(x => x.Delete(filePath), Times.Never);
		}
	}
}