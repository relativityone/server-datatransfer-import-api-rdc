using System.Data;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Core.Import.Tasks;
using Moq;
using NUnit.Framework;
using Relativity;

namespace kCura.WinEDDS.Core.NUnit.Import.Tasks
{
	[TestFixture]
	public class ImportRdoFoldersTaskTests
	{
		private readonly string _expectedFolderPath = string.Empty;

		private ImportRdoFoldersTask _instance;
		private ImportBatchContext _importBatchContext;
		private FileMetadata _fileMetadata;
		private ArtifactField _artifactField;

		private Mock<IImporterSettings> _settings;
		private Mock<IObjectManager> _objectManager;

		[SetUp]
		public void SetUp()
		{
			_objectManager = new Mock<IObjectManager>();

			_settings = new Mock<IImporterSettings>();
			ImportContext importContext = new ImportContext
			{
				Settings = _settings.Object
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

			_instance = new ImportRdoFoldersTask(_objectManager.Object);
		}

		[Test]
		public void ItShouldSetFolderInfoForCaseWhenNotCreatingFolderStructure()
		{
			SetUpCreateFolderStructure(false);
			var expectedFolderId = 429713;

			_settings.Setup(x => x.ParentArtifactTypeID).Returns(ArtifactType.Case);
			_settings.Setup(x => x.FolderId).Returns(expectedFolderId);

			// ACT
			_instance.Execute(_fileMetadata, _importBatchContext);

			// ASSERT
			Assert.That(_fileMetadata.FolderPath, Is.EqualTo(_expectedFolderPath));
			Assert.That(_fileMetadata.ParentFolderId, Is.EqualTo(expectedFolderId));
		}

		[Test]
		public void ItShouldSetFolderInfoForRdoWhenNotCreatingFolderStructure()
		{
			SetUpCreateFolderStructure(false);

			_settings.Setup(x => x.ParentArtifactTypeID).Returns(ArtifactType.Matter);

			// ACT
			_instance.Execute(_fileMetadata, _importBatchContext);

			// ASSERT
			Assert.That(_fileMetadata.FolderPath, Is.EqualTo(_expectedFolderPath));
			Assert.That(_fileMetadata.ParentFolderId, Is.EqualTo(-1));
		}

		[Test]
		public void ItShouldThrowExceptionForEmptyParentArtifact()
		{
			SetUpCreateFolderStructure(true);

			_artifactField.Value = "";

			// ACT
			Assert.That(() => _instance.Execute(_fileMetadata, _importBatchContext), Throws.TypeOf<LoadFileBase.ParentObjectReferenceRequiredException>());
		}

		[Test]
		public void ItShouldThrowExceptionForDuplicatedParentArtifact()
		{
			SetUpCreateFolderStructure(true);

			_artifactField.Value = "parent_artifact";

			int caseContextArtifactId = 704280;
			int artifactTypeId = (int) ArtifactType.Matter;

			_settings.Object.LoadFile.CaseInfo = new CaseInfo
			{
				ArtifactID = caseContextArtifactId
			};
			_settings.Object.LoadFile.ArtifactTypeID = artifactTypeId;

			var dataSet = MockDataSet(1, 2);
			_objectManager.Setup(x => x.RetrieveArtifactIdOfMappedParentObject(caseContextArtifactId, _artifactField.ValueAsString, artifactTypeId)).Returns(dataSet);

			// ACT
			Assert.That(() => _instance.Execute(_fileMetadata, _importBatchContext), Throws.TypeOf<LoadFileBase.DuplicateObjectReferenceException>());
		}

		[Test]
		public void ItShouldThrowExceptionForNonExistentParentArtifact()
		{
			SetUpCreateFolderStructure(true);

			_artifactField.Value = "parent_artifact";

			int caseContextArtifactId = 565283;
			int artifactTypeId = (int) ArtifactType.Matter;

			_settings.Object.LoadFile.CaseInfo = new CaseInfo
			{
				ArtifactID = caseContextArtifactId
			};
			_settings.Object.LoadFile.ArtifactTypeID = artifactTypeId;

			var dataSet = MockDataSet();
			_objectManager.Setup(x => x.RetrieveArtifactIdOfMappedParentObject(caseContextArtifactId, _artifactField.ValueAsString, artifactTypeId)).Returns(dataSet);

			// ACT
			Assert.That(() => _instance.Execute(_fileMetadata, _importBatchContext), Throws.TypeOf<LoadFileBase.NonExistentParentException>());
		}

		[Test]
		public void ItShouldSetValidParentArtifactId()
		{
			SetUpCreateFolderStructure(true);

			var expectedFolderId = 950752;

			_artifactField.Value = "parent_artifact";

			int caseContextArtifactId = 600338;
			int artifactTypeId = (int) ArtifactType.Matter;

			_settings.Object.LoadFile.CaseInfo = new CaseInfo
			{
				ArtifactID = caseContextArtifactId
			};
			_settings.Object.LoadFile.ArtifactTypeID = artifactTypeId;

			var dataSet = MockDataSet(expectedFolderId);
			_objectManager.Setup(x => x.RetrieveArtifactIdOfMappedParentObject(caseContextArtifactId, _artifactField.ValueAsString, artifactTypeId)).Returns(dataSet);

			// ACT
			_instance.Execute(_fileMetadata, _importBatchContext);

			// ASSERT
			Assert.That(_fileMetadata.FolderPath, Is.EqualTo(_expectedFolderPath));
			Assert.That(_fileMetadata.ParentFolderId, Is.EqualTo(expectedFolderId));
		}

		private void SetUpCreateFolderStructure(bool createFolderStructure)
		{
			_settings.Setup(x => x.LoadFile).Returns(new LoadFile
			{
				CreateFolderStructure = createFolderStructure
			});
		}

		private DataSet MockDataSet(params int[] artifactIds)
		{
			DataTable table = new DataTable();
			table.Columns.Add("ArtifactID", typeof(int));

			foreach (var artifactId in artifactIds)
			{
				var dataRow = table.NewRow();
				dataRow["ArtifactID"] = artifactId;
				table.Rows.Add(dataRow);
			}

			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(table);
			return dataSet;
		}
	}
}