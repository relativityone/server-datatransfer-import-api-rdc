using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2
{
	[TestFixture]
	public class FilesOverwriteValidatorTests
	{
		private FilesOverwriteValidator _instance;

		private Mock<IFileHelper> _fileHelperMock;
		private LoadFileDestinationPath _loadFileDestinationPath;
		private ImageLoadFileDestinationPath _imageLoadFileDestinationPath;

		[SetUp]
		public void SetUp()
		{
			Mock<IUserNotification> userNotificationMock = new Mock<IUserNotification>();
			_fileHelperMock = new Mock<IFileHelper>();

			var exportSettings = new ExportFile(1)
			{
				FolderPath = "",
				LoadFilesPrefix = "PREF_",
				LoadFileExtension = ".dat",
				LogFileFormat = LoadFileType.FileFormat.Opticon
			};

			_loadFileDestinationPath = new LoadFileDestinationPath(exportSettings);
			_imageLoadFileDestinationPath = new ImageLoadFileDestinationPath(exportSettings);

			_instance = new FilesOverwriteValidator(userNotificationMock.Object, _fileHelperMock.Object, new Mock<ILog>().Object);
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ItShouldPassWhenOverwritingFiles(FilesOverwriteTestData testData)
		{
			_fileHelperMock.Setup(x => x.Exists(_loadFileDestinationPath.Path)).Returns(testData.LoadFileExists);
			_fileHelperMock.Setup(x => x.Exists(_imageLoadFileDestinationPath.Path)).Returns(testData.ImageLoadFileExists);

			bool actualResult = _instance.ValidateLoadFilesOverwriting(testData.Overwrite, testData.ExportImages, _loadFileDestinationPath, _imageLoadFileDestinationPath);

			Assert.That(actualResult, Is.EqualTo(testData.ExpectedResult));
		}

		private static FilesOverwriteTestData Overwrite_FilesExist()
		{
			return new FilesOverwriteTestData
			{
				Overwrite = true,
				ExportImages = true,
				LoadFileExists = true,
				ImageLoadFileExists = true,
				ExpectedResult = true
			};
		}

		private static FilesOverwriteTestData Overwrite_FilesDoNotExist()
		{
			return new FilesOverwriteTestData
			{
				Overwrite = true,
				ExportImages = true,
				LoadFileExists = false,
				ImageLoadFileExists = false,
				ExpectedResult = true
			};
		}

		private static FilesOverwriteTestData DoNotOverwrite_LoadFileExists()
		{
			return new FilesOverwriteTestData
			{
				Overwrite = false,
				ExportImages = true,
				LoadFileExists = true,
				ImageLoadFileExists = false,
				ExpectedResult = false
			};
		}

		private static FilesOverwriteTestData DoNotOverwrite_ImageLoadFileExists()
		{
			return new FilesOverwriteTestData
			{
				Overwrite = false,
				ExportImages = true,
				LoadFileExists = false,
				ImageLoadFileExists = true,
				ExpectedResult = false
			};
		}

		private static FilesOverwriteTestData DoNotOverwrite_ImageLoadFileExists_DoNotExportImages()
		{
			return new FilesOverwriteTestData
			{
				Overwrite = false,
				ExportImages = false,
				LoadFileExists = false,
				ImageLoadFileExists = true,
				ExpectedResult = true
			};
		}

		private static FilesOverwriteTestData DoNotOverwrite_FilesDoNotExist()
		{
			return new FilesOverwriteTestData
			{
				Overwrite = false,
				ExportImages = true,
				LoadFileExists = false,
				ImageLoadFileExists = false,
				ExpectedResult = true
			};
		}

		private static FilesOverwriteTestData[] TestCases()
		{
			return new[]
			{
				Overwrite_FilesDoNotExist(),
				Overwrite_FilesExist(),
				DoNotOverwrite_LoadFileExists(),
				DoNotOverwrite_ImageLoadFileExists(),
				DoNotOverwrite_ImageLoadFileExists_DoNotExportImages(),
				DoNotOverwrite_FilesDoNotExist()
			};
		}

		public class FilesOverwriteTestData
		{
			public bool Overwrite { get; set; }
			public bool ExportImages { get; set; }
			public bool LoadFileExists { get; set; }
			public bool ImageLoadFileExists { get; set; }
			public bool ExpectedResult { get; set; }
		}
	}
}