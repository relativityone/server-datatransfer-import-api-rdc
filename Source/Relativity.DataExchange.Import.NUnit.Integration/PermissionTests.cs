// ----------------------------------------------------------------------------
// <copyright file="PermissionTests.cs" company="Relativity ODA LLC">
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
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[TestType.Error]
	public class PermissionTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const RelativityVersion MinSupportedVersion = RelativityVersion.Foxglove;
		private bool testsSkipped;

		private int groupId;
		private int userId;
		private string newUsername;
		private string newPassword;
		private string oldUsername;
		private string oldPassword;

		[OneTimeSetUp]
		public async Task OneTimeSetupAsync()
		{
			testsSkipped = RelativityVersionChecker.VersionIsLowerThan(
				               this.TestParameters,
				               MinSupportedVersion);
			if (!testsSkipped)
			{
				this.oldUsername = this.TestParameters.RelativityUserName;
				this.oldPassword = this.TestParameters.RelativityPassword;

				this.newUsername = $"ImportAPI.{Guid.NewGuid().ToString()}@relativity.com";
				this.newPassword = "Test1234!";
				this.userId = await UsersHelper.CreateNewUserAsync(
					              this.TestParameters,
					              this.newUsername,
					              this.newPassword,
					              new List<int> { GroupHelper.EveryoneGroupId }).ConfigureAwait(false);

				ImportHelper.ImportDefaultTestData(this.TestParameters);
			}
		}

		[OneTimeTearDown]
		public async Task OneTimeTeardown()
		{
			if (!testsSkipped)
			{
				this.WithOriginalUser();
				await UsersHelper.RemoveUserAsync(this.TestParameters, this.userId).ConfigureAwait(false);
				await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false);
			}
		}

		[SetUp]
		public async Task SetUpAsync()
		{
			if (!testsSkipped)
			{
				this.WithOriginalUser();
				this.groupId = await GroupHelper.CreateNewGroupAsync(this.TestParameters, Guid.NewGuid().ToString()).ConfigureAwait(false);
				await GroupHelper.AddMemberAsync(this.TestParameters, this.groupId, this.userId).ConfigureAwait(false);
				await PermissionsHelper.AddGroupToWorkspaceAsync(this.TestParameters, this.groupId).ConfigureAwait(false);
				await PermissionsHelper.AddGroupToAdminAsync(this.TestParameters, this.groupId).ConfigureAwait(false);
				await PermissionsHelper
					.SetAdminObjectSecurityAsync(
						this.TestParameters,
						this.groupId,
						new List<string> { "InstanceSetting" },
						true)
					.ConfigureAwait(false);
			}
		}

		[TearDown]
		public async Task TearDownAsync()
		{
			if (!testsSkipped)
			{
				this.WithOriginalUser();
				await GroupHelper.RemoveGroupAsync(this.TestParameters, this.groupId).ConfigureAwait(false);
			}
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("6920c59d-8ffc-43e1-9dc5-15909dcd8d8a")]
		[TestExecutionCategory.CI]
		public async Task ShouldPreventSecurityAdd()
		{
			// ARRANGE
			const int TotalRows = 10;
			const bool EnableAllPermissions = false;

			await PermissionsHelper.SetAllWorkspaceOtherSettingsAsync(
					this.TestParameters,
					this.groupId,
					EnableAllPermissions)
				.ConfigureAwait(false);

			await PermissionsHelper
				.SetWorkspaceOtherSettingsAsync(
					this.TestParameters,
					this.groupId,
					new List<string> { "Allow Import" },
					true)
				.ConfigureAwait(false);

			this.WithNewUser();
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);
			IEnumerable<string> controlNumberSource = Enumerable.Range(1, TotalRows).Select(i => $"doc-{nameof(this.ShouldPreventSecurityAdd)}-{i}");

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberSource)
				.Build();

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			ThenTheErrorRowsHaveCorrectMessage(result.ErrorRows, " - Your account does not have rights to add a document or object to this case");
			Assert.That(result.JobReportErrorsCount, Is.EqualTo(TotalRows));
			Assert.That(result.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(TotalRows));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("aec57885-a9b9-40f3-ac30-f0538b6cb6cf")]
		[TestExecutionCategory.CI]
		public async Task ShouldPreventSecuredDocumentsEdit()
		{
			// ARRANGE
			const int NumberOfDocumentsToOverlay = 10;
			const string ControlNumberSuffix = "EditWOPermissionsTest";
			const bool EnablePermissions = false;

			await PermissionsHelper.SetAllWorkspaceOtherSettingsAsync(this.TestParameters, this.groupId, EnablePermissions)
				.ConfigureAwait(false);
			await PermissionsHelper
				.SetWorkspaceOtherSettingsAsync(this.TestParameters, this.groupId, new List<string> { "Allow Import" }, true)
				.ConfigureAwait(false);

			this.WithNewUser();
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Overlay;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumber = GetControlNumberEnumerable(
				OverwriteModeEnum.Overlay,
				NumberOfDocumentsToOverlay,
				ControlNumberSuffix);

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber).Build();

			// ACT
			ImportTestJobResult resultsOverlay = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			ThenTheErrorRowsHaveCorrectMessage(resultsOverlay.ErrorRows, " - The document specified has been secured for editing");
			Assert.That(resultsOverlay.ErrorRows.Count, Is.EqualTo(NumberOfDocumentsToOverlay), () => "Wrong number of error rows.");
			Assert.That(resultsOverlay.JobReportTotalRows, Is.EqualTo(NumberOfDocumentsToOverlay), () => "Wrong number of total job reports.");
		}

		[Ignore("We don't check permissions for associate objects")]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("97286c32-733f-4e5f-9a1f-82f165a1b3bd")]
		public async Task ShouldPreventAddingAssociatedObject()
		{
			// ARRANGE
			const int NumberOfDocumentsToImport = 10;
			const bool EnablePermissions = false;
			string referenceToObjectFieldName = $"ReferenceToObject-{nameof(PermissionTests)}";
			string controlNumberSuffix = $"{nameof(this.ShouldPreventAddingAssociatedObject)}";
			string objectTypeName = $"{nameof(PermissionTests)}-Object";

			int objectArtifactTypeId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, objectTypeName)
											.ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(
					this.TestParameters,
					referenceToObjectFieldName,
					objectArtifactTypeId: (int)ArtifactType.Document,
					associativeObjectArtifactTypeId: objectArtifactTypeId)
				.ConfigureAwait(false);

			await PermissionsHelper.SetAllWorkspaceOtherSettingsAsync(this.TestParameters, this.groupId, EnablePermissions)
				.ConfigureAwait(false);
			await PermissionsHelper
				.SetWorkspaceOtherSettingsAsync(this.TestParameters, this.groupId, new List<string> { "Allow Import" }, true)
				.ConfigureAwait(false);

			await PermissionsHelper.SetWorkspaceObjectSecurityAsync(
				this.TestParameters,
				this.groupId,
				new List<string> { "Document" },
				true).ConfigureAwait(false);

			await PermissionsHelper.SetWorkspaceObjectSecurityAsync(
				this.TestParameters,
				this.groupId,
				new List<string> { objectTypeName },
				false).ConfigureAwait(false);

			this.WithNewUser();
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumber = GetControlNumberEnumerable(
				OverwriteModeEnum.Append,
				NumberOfDocumentsToImport,
				controlNumberSuffix);

			IEnumerable<string> referenceToMyObjectSource = Enumerable.Range(1, NumberOfDocumentsToImport)
				.Select(i => $"{nameof(this.ShouldPreventAddingAssociatedObject)}-Object-{i}");

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber)
				.AddField(referenceToObjectFieldName, referenceToMyObjectSource)
				.Build();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			ThenTheErrorRowsHaveCorrectMessage(results.ErrorRows, " - Your account does not have rights to add an associated object to the current object");
			Assert.That(results.ErrorRows.Count, Is.EqualTo(NumberOfDocumentsToImport), () => "Wrong number of error rows.");
			Assert.That(results.JobReportTotalRows, Is.EqualTo(NumberOfDocumentsToImport), () => "Wrong number of total job reports.");
		}

		private void WithNewUser()
		{
			this.TestParameters.RelativityUserName = this.newUsername;
			this.TestParameters.RelativityPassword = this.newPassword;
		}

		private void WithOriginalUser()
		{
			this.TestParameters.RelativityUserName = this.oldUsername;
			this.TestParameters.RelativityPassword = this.oldPassword;
		}
	}
}
