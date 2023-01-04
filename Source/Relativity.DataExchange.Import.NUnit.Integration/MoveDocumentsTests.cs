// ----------------------------------------------------------------------------
// <copyright file="MoveDocumentsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.LinkManager.Interfaces;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	using ArtifactType = Relativity.ArtifactType;
	using User = Relativity.Services.User.User;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[TestType.MainFlow]
	[TestExecutionCategory.CI]
	public class MoveDocumentsTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const RelativityVersion MinSupportedVersion = RelativityVersion.NinebarkFolderMove;
		private const string TestDestinationFolder = "TestDestinationFolder";
		private const string TestSourceFolder = "TestSourceFolder";

		private bool testsSkipped;

		private int testDestinationFolderId;

		private int testSourceFolderId;

		private int testDeeperDestinationFolderId;

		private int workspaceFolderId;
		private IList<RelativityObject> importedDocuments;

		[OneTimeSetUp]
		public async Task OneTimeSetUp()
		{
			testsSkipped = RelativityVersionChecker.VersionIsLowerThan(
				this.TestParameters,
				MinSupportedVersion);

			this.workspaceFolderId = await FolderHelper.GetWorkspaceRootArtifactIdAsync(this.TestParameters).ConfigureAwait(false);
			List<int> folderIds = await FolderHelper.CreateFolders(this.TestParameters, new List<string> { TestDestinationFolder, TestSourceFolder }, this.workspaceFolderId).ConfigureAwait(false);

			this.testDestinationFolderId = folderIds.First();
			this.testSourceFolderId = folderIds.Skip(1).First();

			List<int> deeperFolderIds = await FolderHelper.CreateFolders(this.TestParameters, new List<string> { TestSourceFolder }, this.testDestinationFolderId).ConfigureAwait(false);

			this.testDeeperDestinationFolderId = deeperFolderIds.First();
		}

		[SetUp]
		public void SetUp()
		{
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> documentsNames = Enumerable
				.Range(1, 4)
				.Select(i => $"{nameof(MoveDocumentsTests)}-{i}")
				.ToList();

			IEnumerable<string> documentsFolders = new List<string>
				                                           {
															   string.Empty,
															   string.Empty,
															   TestSourceFolder,
															   TestSourceFolder,
				                                           };

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, documentsNames)
				.AddField(WellKnownFields.FolderName, documentsFolders)
				.Build();

			this.JobExecutionContext.Execute(importData);

			string[] fieldsToValidate = { WellKnownFields.ControlNumber, WellKnownFields.ArtifactId };

			this.importedDocuments = RdoHelper.QueryRelativityObjects(
				this.TestParameters,
				(int)ArtifactTypeID.Document,
				fieldsToValidate);
		}

		[TearDown]
		public async Task TearDown()
		{
			if (!this.testsSkipped)
			{
				await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false);
			}
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("9AA8D3B6-4126-4AE5-A843-18A8565DF858")]
		public void ShouldMoveDocumentFromFolderToDifferentFolderWhenOverlay()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = true;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
					WellKnownFields.ControlNumber,
					this.importedDocuments[0].FieldValues.Select(x => x.Value).Take(1))
					.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 1);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("1C69567F-A763-4E5D-9AFC-BF15C3126437")]
		public void ShouldNotMoveDocumentFromFolderToDifferentFolderWhenOverlayAndMoveDisabled()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = false;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
					WellKnownFields.ControlNumber,
					this.importedDocuments[0].FieldValues.Select(x => x.Value).Take(1))
				.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 1);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("992985BD-50CA-4927-BC69-B5036FBF5739")]
		public void ShouldMoveDocumentFromFolderToDifferentFolderWhenAppendOverlay()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = true;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
					WellKnownFields.ControlNumber,
					this.importedDocuments[0].FieldValues.Select(x => x.Value).Take(1))
				.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 1);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("89788F06-C081-4D35-B175-71B64B642F32")]
		public void ShouldNotMoveDocumentFromFolderToDifferentFolderWhenAppendOverlayAndMoveDisabled()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = false;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
				WellKnownFields.ControlNumber,
				this.importedDocuments[0].FieldValues.Select(x => x.Value).Take(1))
				.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 1);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("D12C3757-A5FD-40D2-91E2-C435AB4AC4C2")]
		public void ShouldNotMoveDocumentFromFolderToSameFolderWhenOverlay()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.DestinationFolderArtifactID = this.workspaceFolderId;
			settings.MoveDocumentsInAppendOverlayMode = true;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
					WellKnownFields.ControlNumber,
					this.importedDocuments[0].FieldValues.Select(x => x.Value).Take(1))
				.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 1);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("992985BD-50CA-4927-BC69-B5036FBF5739")]
		public void ShouldMoveTwoDocumentsFromFolderToFolder()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;

			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = true;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumberSource = new List<string>
				                                          {
					                                          (string)this.importedDocuments[0].FieldValues[0].Value,
					                                          (string)this.importedDocuments[1].FieldValues[0].Value,
				                                          };

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
					WellKnownFields.ControlNumber,
					controlNumberSource)
				.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 2);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("C2B5860B-2082-497A-8A0F-68E7DDFE10F4")]
		public void ShouldMoveTwoDocumentsFromDifferentFoldersToDifferentFolders()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = true;
			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumberSource = new List<string>
				                                          {
					                                          (string)this.importedDocuments[0].FieldValues[0].Value,
					                                          (string)this.importedDocuments[2].FieldValues[0].Value,
				                                          };

			IEnumerable<string> folderSource = new List<string>
				                                          {
					                                          string.Empty,
					                                          TestSourceFolder,
				                                          };

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberSource)
				.AddField(WellKnownFields.FolderName, folderSource)
				.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 2);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDeeperDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("3E03098E-54E2-475F-B448-843C3481F8B1")]
		public void ShouldMoveDocumentFromDifferentFolderEvenIfNoDestinationFolder()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.MoveDocumentsInAppendOverlayMode = true;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumberSource = new List<string>
														  {
															  (string)this.importedDocuments[0].FieldValues[0].Value,
															  (string)this.importedDocuments[2].FieldValues[0].Value,
														  };

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
					WellKnownFields.ControlNumber,
					controlNumberSource)
				.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 2);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}
	}
}
