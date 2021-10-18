// ----------------------------------------------------------------------------
// <copyright file="PermissionImportTests.cs" company="Relativity ODA LLC">
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
	public class PermissionImportTests : ImportJobTestBase<NativeImportExecutionContext>
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

		[TestType.MainFlow]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[TestExecutionCategory.CI]
		[IdentifiedTestCase("E2DBA887-1CB2-4D2B-9D4F-5F27BCC71833", OverwriteModeEnum.Append)]
		[IdentifiedTestCase("10BD74E4-F43F-43AA-B38E-8D3EBE75ED43", OverwriteModeEnum.Overlay)]
		public async Task ShouldImportWithMinimalSetOfPermissions(OverwriteModeEnum mode)
		{
			// ARRANGE
			const int NumberOfDocuments = 10;
			const string ControlNumberSuffix = "MinimalPermissionsTest";

			await PermissionsHelper
				.SetAllWorkspaceOtherSettingsAsync(
					this.TestParameters,
					this.groupId,
					false)
				.ConfigureAwait(false);

			await PermissionsHelper
				.SetWorkspaceOtherSettingsAsync(
					this.TestParameters,
					this.groupId,
					new List<string> { "Allow Import" },
					true)
				.ConfigureAwait(false);

			await PermissionsHelper
				.SetWorkspaceObjectSecurityAsync(
					this.TestParameters,
					this.groupId,
					new List<string> { "Document" },
					mode == OverwriteModeEnum.Append,
					mode == OverwriteModeEnum.Overlay,
					false)
				.ConfigureAwait(false);

			this.WithNewUser();
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = mode;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumberSource = GetControlNumberEnumerable(
				mode,
				NumberOfDocuments,
				ControlNumberSuffix);

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberSource)
				.Build();

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(result, NumberOfDocuments);
			Assert.That(result.NumberOfJobMessages, Is.Positive);
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(NumberOfDocuments));
		}

		[TestType.Error]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("aec57885-a9b9-40f3-ac30-f0538b6cb6cf")]
		[TestExecutionCategory.CI]
		[IdentifiedTestCase("1375A1EA-E1AD-499F-AC19-AC49332CB661", OverwriteModeEnum.Append, false, false)]
		[IdentifiedTestCase("91C78113-9CAA-4CFE-8BC2-E51615487790", OverwriteModeEnum.Append, false, true)]
		[IdentifiedTestCase("A02ED44C-389D-4E9F-A64B-E5FD542E10CC", OverwriteModeEnum.Overlay, false, false)]
		[IdentifiedTestCase("79EE4F98-1B46-4C71-9188-66882C4CE8A7", OverwriteModeEnum.Overlay, true, false)]
		public async Task ShouldPreventImportDocumentsWithoutPermissions(OverwriteModeEnum mode, bool canAdd, bool canEdit)
		{
			// ARRANGE
			const int NumberOfDocuments = 10;
			const string ControlNumberSuffix = "PermissionsTestDocument";
			const bool EnablePermissions = false;

			await PermissionsHelper.SetAllWorkspaceOtherSettingsAsync(
					this.TestParameters,
					this.groupId,
					EnablePermissions)
				.ConfigureAwait(false);

			await PermissionsHelper
				.SetWorkspaceOtherSettingsAsync(
					this.TestParameters,
					this.groupId,
					new List<string> { "Allow Import" },
					true)
				.ConfigureAwait(false);

			if (canAdd || canEdit)
			{
				await PermissionsHelper.SetWorkspaceObjectSecurityAsync(
						this.TestParameters,
						this.groupId,
						new List<string> { "Document" },
						canAdd,
						canEdit,
						false)
					.ConfigureAwait(false);
			}

			this.WithNewUser();
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = mode;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumberSource = GetControlNumberEnumerable(
				mode,
				NumberOfDocuments,
				ControlNumberSuffix);

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberSource)
				.Build();

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			string expectedErrorMessage = mode == OverwriteModeEnum.Append
				                              ? " - Your account does not have rights to add a document or object to this case"
				                              : " - The document specified has been secured for editing";
			ThenTheErrorRowsHaveCorrectMessage(result.ErrorRows, expectedErrorMessage);
			Assert.That(result.JobReportTotalRows, Is.EqualTo(NumberOfDocuments), () => "Wrong number of total job reports.");
			Assert.That(result.JobReportErrorsCount, Is.EqualTo(NumberOfDocuments), () => "Wrong number of error rows.");
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(NumberOfDocuments), () => "Wrong number of number of completed rows.");
			Assert.That(result.NumberOfJobMessages, Is.Positive);
		}

		[TestType.Error]
		[Ignore("We don't check permissions for associate objects")]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("97286c32-733f-4e5f-9a1f-82f165a1b3bd")]
		public async Task ShouldPreventAddingAssociatedObject()
		{
			// ARRANGE
			const int NumberOfDocumentsToImport = 10;
			const bool EnablePermissions = false;
			string referenceToObjectFieldName = $"ReferenceToObject-{nameof(PermissionImportTests)}";
			string controlNumberSuffix = $"{nameof(this.ShouldPreventAddingAssociatedObject)}";
			string objectTypeName = $"{nameof(PermissionImportTests)}-Object";

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
				true,
				true,
				true).ConfigureAwait(false);

			await PermissionsHelper.SetWorkspaceObjectSecurityAsync(
				this.TestParameters,
				this.groupId,
				new List<string> { objectTypeName },
				false,
				false,
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
