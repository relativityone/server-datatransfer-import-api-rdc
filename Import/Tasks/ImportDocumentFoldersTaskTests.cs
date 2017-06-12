using System.IO;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Tasks;
using kCura.WinEDDS.Importers;
using Moq;
using NUnit.Framework;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Import.Tasks
{
	[TestFixture]
	public class ImportDocumentFoldersTaskTests
	{
		private const int _UNKNOWN_PARENT_FOLDER_ID = -9;

		private ImportDocumentFoldersTask _instance;

		private ImportBatchContext _importBatchContext;
		private FileMetadata _fileMetadata;
		private ArtifactField _artifactField;

		private Mock<IImporterSettings> _importerSettings;
		private Mock<ITransferConfig> _transferConfig;
		private Mock<IFolderCache> _folderCache;
		private readonly Mock<ILog> _logMock = new Mock<ILog>();

		[SetUp]
		public void SetUp()
		{
			_folderCache = new Mock<IFolderCache>();
			_transferConfig = new Mock<ITransferConfig>();

			_importerSettings = new Mock<IImporterSettings>();
			ImportContext importContext = new ImportContext
			{
				Settings = _importerSettings.Object
			};
			_importBatchContext = new ImportBatchContext(importContext, 1000);

			_artifactField = new ArtifactField("ParentFolder", 349, FieldTypeHelper.FieldType.Varchar, FieldCategory.ParentArtifact, 1, 1, null, false);
			_fileMetadata = new FileMetadata
			{
				ArtifactFieldCollection = new ArtifactFieldCollection
				{
					_artifactField
				}
			};

			_instance = new ImportDocumentFoldersTask(_folderCache.Object, _transferConfig.Object, _logMock.Object);
		}

		[Test]
		public void ItShouldSetFolderInfoWhenNotCreatingFolderStructure()
		{
			int expectedFolderId = 438133;
			string expectedFolderPath = string.Empty;

			SetUpCreatingFolders(false, false);

			_importerSettings.Setup(x => x.FolderId).Returns(expectedFolderId);

			// ACT

			_instance.Execute(_fileMetadata, _importBatchContext);

			// ASSERT
			Assert.That(_fileMetadata.FolderPath, Is.EqualTo(expectedFolderPath));
			Assert.That(_fileMetadata.ParentFolderId, Is.EqualTo(expectedFolderId));
		}

		[Test]
		public void ItShouldSetFolderInfoWhenNotCreatingFolderInWebApi()
		{
			int expectedFolderId = 894926;
			string expectedFolderPath = string.Empty;

			SetUpCreatingFolders(true, false);

			_folderCache.Setup(x => x.get_FolderId(It.IsAny<string>())).Returns(expectedFolderId);

			// ACT

			_instance.Execute(_fileMetadata, _importBatchContext);

			// ASSERT
			Assert.That(_fileMetadata.FolderPath, Is.EqualTo(expectedFolderPath));
			Assert.That(_fileMetadata.ParentFolderId, Is.EqualTo(expectedFolderId));
		}

		[Test]
		public void ItShouldThrowExceptionForTooLongPath()
		{
			_artifactField.Value = new string('x', 300);

			SetUpCreatingFolders(true, true);

			// ACT
			Assert.That(() => _instance.Execute(_fileMetadata, _importBatchContext), Throws.TypeOf<PathTooLongException>());
		}

		[Test]
		public void ItShouldSetFolderInfoWhenCreatingFolders()
		{
			var folderPath = "expected_path_227";
			var expectedFolderPath = "\\expected_path_227";

			_artifactField.Value = folderPath;

			SetUpCreatingFolders(true, true);

			// ACT
			_instance.Execute(_fileMetadata, _importBatchContext);

			// ASSERT
			Assert.That(_fileMetadata.ParentFolderId, Is.EqualTo(_UNKNOWN_PARENT_FOLDER_ID));
			Assert.That(_fileMetadata.FolderPath, Is.EqualTo(expectedFolderPath));
		}

		private void SetUpCreatingFolders(bool createFolderStructure, bool createFoldersInWebApi)
		{
			_importerSettings.Setup(x => x.LoadFile).Returns(new LoadFile
			{
				CreateFolderStructure = createFolderStructure
			});

			_transferConfig.Setup(x => x.CreateFoldersInWebAPI).Returns(createFoldersInWebApi);
		}
	}
}