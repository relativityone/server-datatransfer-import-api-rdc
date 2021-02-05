// ----------------------------------------------------------------------------
// <copyright file="MoveDocumentsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
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
	using Relativity.Services.User;
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
		private string testDestinationFolderName;

		private int testSourceFolderId;
		private string testSourceFolderName;

		private int testDeeperDestinationFolderId;
		private string testDeeperDestinationFolderName;

		private int workspaceFolderId;
		private int currentUserId;
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
			this.testDestinationFolderName = TestParameters.WorkspaceName + " | " + TestDestinationFolder;

			this.testSourceFolderId = folderIds.Skip(1).First();
			this.testSourceFolderName = TestParameters.WorkspaceName + " | " + TestSourceFolder;

			List<int> deeperFolderIds = await FolderHelper.CreateFolders(this.TestParameters, new List<string> { TestSourceFolder }, this.testDestinationFolderId).ConfigureAwait(false);

			this.testDeeperDestinationFolderId = deeperFolderIds.First();
			this.testDeeperDestinationFolderName = this.testDestinationFolderName + " | " + TestSourceFolder;

			using (var userManager = ServiceHelper.GetServiceProxy<IUserManager>(this.TestParameters))
			{
				User currentUser = await userManager.RetrieveCurrentAsync(this.TestParameters.WorkspaceId).ConfigureAwait(false);
				currentUserId = currentUser.ArtifactID;
			}
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
		public async Task ShouldMoveDocumentFromFolderToDifferentFolderWhenOverlayAsync()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = true;
			var executionStart = DateTime.Now;
			var expectedAuditDetails = new List<Dictionary<string, string>>
											{
												new Dictionary<string, string>
													{
														["@destinationFolderName"] = this.testDestinationFolderName,
														["@sourceFolderArtifactID"] = this.workspaceFolderId.ToString(),
														["@sourceFolderName"] = this.TestParameters.WorkspaceName,
														["@destinationFolderArtifactID"] = this.testDestinationFolderId.ToString(),
														["ArtifactID"] = this.importedDocuments[0].FieldValues[1].Value.ToString(),
													},
											};

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
					WellKnownFields.ControlNumber,
					this.importedDocuments[0].FieldValues.Select(x => x.Value).Take(1))
					.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 1);
			await this.ThenTheAuditIsCorrectAsync(this.currentUserId, executionStart, expectedAuditDetails, 1, AuditHelper.AuditAction.Move).ConfigureAwait(false);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("1C69567F-A763-4E5D-9AFC-BF15C3126437")]
		public async Task ShouldNotMoveDocumentFromFolderToDifferentFolderWhenOverlayAndMoveDisabledAsync()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = false;
			var executionStart = DateTime.Now;
			var expectedAuditDetails = new List<Dictionary<string, string>>();

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
					WellKnownFields.ControlNumber,
					this.importedDocuments[0].FieldValues.Select(x => x.Value).Take(1))
				.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 1);
			await this.ThenTheAuditIsCorrectAsync(this.currentUserId, executionStart, expectedAuditDetails, 1, AuditHelper.AuditAction.Move).ConfigureAwait(false);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("992985BD-50CA-4927-BC69-B5036FBF5739")]
		public async Task ShouldMoveDocumentFromFolderToDifferentFolderWhenAppendOverlayAsync()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = true;
			var executionStart = DateTime.Now;
			var expectedAuditDetails = new List<Dictionary<string, string>>
											{
												new Dictionary<string, string>
													{
														["@destinationFolderName"] = this.testDestinationFolderName,
														["@sourceFolderArtifactID"] = this.workspaceFolderId.ToString(),
														["@sourceFolderName"] = this.TestParameters.WorkspaceName,
														["@destinationFolderArtifactID"] = this.testDestinationFolderId.ToString(),
														["ArtifactID"] = this.importedDocuments[0].FieldValues[1].Value.ToString(),
													},
											};

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
					WellKnownFields.ControlNumber,
					this.importedDocuments[0].FieldValues.Select(x => x.Value).Take(1))
				.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 1);
			await this.ThenTheAuditIsCorrectAsync(this.currentUserId, executionStart, expectedAuditDetails, 1, AuditHelper.AuditAction.Move).ConfigureAwait(false);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("89788F06-C081-4D35-B175-71B64B642F32")]
		public async Task ShouldNotMoveDocumentFromFolderToDifferentFolderWhenAppendOverlayAndMoveDisabledAsync()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = false;
			var executionStart = DateTime.Now;
			var expectedAuditDetails = new List<Dictionary<string, string>>();

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
				WellKnownFields.ControlNumber,
				this.importedDocuments[0].FieldValues.Select(x => x.Value).Take(1))
				.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 1);
			await this.ThenTheAuditIsCorrectAsync(this.currentUserId, executionStart, expectedAuditDetails, 1, AuditHelper.AuditAction.Move).ConfigureAwait(false);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("D12C3757-A5FD-40D2-91E2-C435AB4AC4C2")]
		public async Task ShouldNotMoveDocumentFromFolderToSameFolderWhenOverlayAsync()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.DestinationFolderArtifactID = this.workspaceFolderId;
			settings.MoveDocumentsInAppendOverlayMode = true;
			var executionStart = DateTime.Now;
			var expectedAuditDetails = new List<Dictionary<string, string>>();

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSource<object[]> importData = ImportDataSourceBuilder.New().AddField(
					WellKnownFields.ControlNumber,
					this.importedDocuments[0].FieldValues.Select(x => x.Value).Take(1))
				.Build();

			// Act
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// Assert
			this.ThenTheImportJobIsSuccessful(results, 1);
			await this.ThenTheAuditIsCorrectAsync(this.currentUserId, executionStart, expectedAuditDetails, 1, AuditHelper.AuditAction.Move).ConfigureAwait(false);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("992985BD-50CA-4927-BC69-B5036FBF5739")]
		public async Task ShouldMoveTwoDocumentsFromFolderToFolderAsync()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;

			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = true;
			var executionStart = DateTime.Now;
			var expectedAuditDetails = new List<Dictionary<string, string>>
											{
												new Dictionary<string, string>
													{
														["@destinationFolderName"] = this.testDestinationFolderName,
														["@sourceFolderArtifactID"] = this.workspaceFolderId.ToString(),
														["@sourceFolderName"] = this.TestParameters.WorkspaceName,
														["@destinationFolderArtifactID"] = this.testDestinationFolderId.ToString(),
														["ArtifactID"] = this.importedDocuments[0].FieldValues[1].Value.ToString(),
													},
												new Dictionary<string, string>
													{
														["@destinationFolderName"] = this.testDestinationFolderName,
														["@sourceFolderArtifactID"] = this.workspaceFolderId.ToString(),
														["@sourceFolderName"] = this.TestParameters.WorkspaceName,
														["@destinationFolderArtifactID"] = this.testDestinationFolderId.ToString(),
														["ArtifactID"] = this.importedDocuments[1].FieldValues[1].Value.ToString(),
													},
											};

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
			await this.ThenTheAuditIsCorrectAsync(this.currentUserId, executionStart, expectedAuditDetails, 2, AuditHelper.AuditAction.Move).ConfigureAwait(false);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("C2B5860B-2082-497A-8A0F-68E7DDFE10F4")]
		public async Task ShouldMoveTwoDocumentsFromDifferentFoldersToDifferentFoldersAsync()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.DestinationFolderArtifactID = this.testDestinationFolderId;
			settings.MoveDocumentsInAppendOverlayMode = true;
			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			var executionStart = DateTime.Now;
			var expectedAuditDetails = new List<Dictionary<string, string>>
											{
												new Dictionary<string, string>
													{
														["@destinationFolderName"] = this.testDeeperDestinationFolderName,
														["@sourceFolderArtifactID"] = this.testSourceFolderId.ToString(),
														["@sourceFolderName"] = this.testSourceFolderName,
														["@destinationFolderArtifactID"] = this.testDeeperDestinationFolderId.ToString(),
														["ArtifactID"] = this.importedDocuments[2].FieldValues[1].Value.ToString(),
													},
												new Dictionary<string, string>
													{
														["@destinationFolderName"] = this.testDestinationFolderName,
														["@sourceFolderArtifactID"] = this.workspaceFolderId.ToString(),
														["@sourceFolderName"] = this.TestParameters.WorkspaceName,
														["@destinationFolderArtifactID"] = this.testDestinationFolderId.ToString(),
														["ArtifactID"] = this.importedDocuments[0].FieldValues[1].Value.ToString(),
													},
											};

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
			await this.ThenTheAuditIsCorrectAsync(this.currentUserId, executionStart, expectedAuditDetails, 2, AuditHelper.AuditAction.Move).ConfigureAwait(false);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testDeeperDestinationFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("3E03098E-54E2-475F-B448-843C3481F8B1")]
		public async Task ShouldMoveDocumentFromDifferentFolderEvenIfNoDestinationFolderAsync()
		{
			// Arrange
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.MoveDocumentsInAppendOverlayMode = true;
			var executionStart = DateTime.Now;
			var expectedAuditDetails = new List<Dictionary<string, string>>
				                           {
					                           new Dictionary<string, string>
						                           {
							                           ["@destinationFolderName"] = this.TestParameters.WorkspaceName,
							                           ["@sourceFolderArtifactID"] = this.testSourceFolderId.ToString(),
							                           ["@sourceFolderName"] = this.testSourceFolderName,
							                           ["@destinationFolderArtifactID"] = this.workspaceFolderId.ToString(),
							                           ["ArtifactID"] = this.importedDocuments[2].FieldValues[1].Value.ToString(),
						                           },
				                           };

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
			await this.ThenTheAuditIsCorrectAsync(this.currentUserId, executionStart, expectedAuditDetails, 1, AuditHelper.AuditAction.Move).ConfigureAwait(false);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fields: new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(4));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[0].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[1].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[2].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.workspaceFolderId));
			Assert.That(relativityObjects.First(x => x.ArtifactID == this.importedDocuments[3].ArtifactID).ParentObject.ArtifactID, Is.EqualTo(this.testSourceFolderId));
		}
	}
}
