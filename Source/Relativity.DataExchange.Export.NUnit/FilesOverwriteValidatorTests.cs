// -----------------------------------------------------------------------------------------------------
// <copyright file="FilesOverwriteValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.DataExchange.Export.VolumeManagerV2.Validation;
	using Relativity.DataExchange.Io;
	using Relativity.Logging;

	[TestFixture]
	public class FilesOverwriteValidatorTests
	{
		private FilesOverwriteValidator _instance;

		private Mock<IFile> _fileHelperMock;
		private LoadFileDestinationPath _loadFileDestinationPath;
		private ImageLoadFileDestinationPath _imageLoadFileDestinationPath;

		[SetUp]
		public void SetUp()
		{
			Mock<IUserNotification> userNotificationMock = new Mock<IUserNotification>();
			this._fileHelperMock = new Mock<IFile>();

			var exportSettings = new ExportFile(1)
			{
				FolderPath = string.Empty,
				LoadFilesPrefix = "PREF_",
				LoadFileExtension = ".dat",
				LogFileFormat = LoadFileType.FileFormat.Opticon
			};

			this._loadFileDestinationPath = new LoadFileDestinationPath(exportSettings);
			this._imageLoadFileDestinationPath = new ImageLoadFileDestinationPath(exportSettings);

			this._instance = new FilesOverwriteValidator(userNotificationMock.Object, this._fileHelperMock.Object, new Mock<ILog>().Object);
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ItShouldPassWhenOverwritingFiles(FilesOverwriteTestData testData)
		{
			this._fileHelperMock.Setup(x => x.Exists(this._loadFileDestinationPath.Path)).Returns(testData.LoadFileExists);
			this._fileHelperMock.Setup(x => x.Exists(this._imageLoadFileDestinationPath.Path)).Returns(testData.ImageLoadFileExists);

			bool actualResult = this._instance.ValidateLoadFilesOverwriting(testData.Overwrite, testData.ExportImages, this._loadFileDestinationPath, this._imageLoadFileDestinationPath);

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