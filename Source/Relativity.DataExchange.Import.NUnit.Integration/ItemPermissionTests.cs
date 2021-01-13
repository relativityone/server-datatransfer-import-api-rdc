// ----------------------------------------------------------------------------
// <copyright file="ItemPermissionTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Threading.Tasks;
	using global::NUnit.Framework;
	using kCura.Relativity.DataReaderClient;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.LinkManager.Interfaces;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[TestType.Error]
	[TestExecutionCategory.CI]
	public class ItemPermissionTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const RelativityVersion MinSupportedVersion = RelativityVersion.Juniper;
		private bool testsSkipped;

		private int groupId;
		private int userId;
		private int folderId;
		private string newUsername;
		private string newPassword;
		private string oldUsername;
		private string oldPassword;

		private DocumentWithArtifactFieldDto[] importedDocuments;

		[OneTimeSetUp]
		public async Task SetupObjectAsync()
		{
			testsSkipped = RelativityVersionChecker.VersionIsLowerThan(
				this.TestParameters,
				MinSupportedVersion);

			if (!testsSkipped)
			{
				this.oldUsername = this.TestParameters.RelativityUserName;
				this.oldPassword = this.TestParameters.RelativityPassword;

				const int EveryoneGroupId = 1015005;
				string lastName = Guid.NewGuid().ToString();
				this.newUsername = $"ImportAPI.{lastName}@relativity.com";
				this.newPassword = "Test1234!";
				this.userId = await UsersHelper.CreateNewUserAsync(
								  this.TestParameters,
								  this.newUsername,
								  this.newPassword,
								  new List<int> { EveryoneGroupId }).ConfigureAwait(false);

				var rootArtifactId = await FolderHelper.GetWorkspaceRootArtifactIdAsync(this.TestParameters).ConfigureAwait(false);
				List<int> folderIds = await FolderHelper.CreateFolders(this.TestParameters, new List<string> { "TestFolder" }, rootArtifactId).ConfigureAwait(false);

				this.folderId = folderIds.First();

				ImportHelper.ImportDefaultTestDataToFolderByArtifactId(this.TestParameters, this.folderId);

				string[] fieldsToValidate = { WellKnownFields.ControlNumber, WellKnownFields.ArtifactId };
				this.importedDocuments = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.Document, fieldsToValidate)
					.Select(ro => new DocumentWithArtifactFieldDto((string)ro.FieldValues[0].Value, ro.ArtifactID))
					.ToArray();

				this.WithOriginalUser();
				this.groupId = await GroupHelper.CreateNewGroupAsync(this.TestParameters, Guid.NewGuid().ToString()).ConfigureAwait(false);
				await GroupHelper.AddMemberAsync(this.TestParameters, this.groupId, this.userId).ConfigureAwait(false);
				await PermissionsHelper.AddGroupToWorkspaceAsync(this.TestParameters, this.groupId).ConfigureAwait(false);

				await PermissionsHelper.SetAllWorkspaceOtherSettingsAsync(
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

				await PermissionsHelper.ApplyItemLevelSecurityAsync(TestParameters, this.folderId, this.groupId, false).ConfigureAwait(false);
				await PermissionsHelper.ApplyItemLevelSecurityAsync(TestParameters, this.importedDocuments[0].ArtifactId, this.groupId, false).ConfigureAwait(false);
			}
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDown()
		{
			if (!this.testsSkipped)
			{
				this.WithOriginalUser();
				await UsersHelper.RemoveUserAsync(this.TestParameters, this.userId).ConfigureAwait(false);
				await GroupHelper.RemoveGroupAsync(this.TestParameters, this.groupId).ConfigureAwait(false);
				await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false);
			}
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("D27147B2-608C-42CA-8A4C-00AC932F04B6")]
		public async Task ShouldPreventAppendWhenFolderIsSecured()
		{
			// ARRANGE
			await PermissionsHelper.ApplyItemLevelSecurityAsync(TestParameters, this.folderId, this.groupId, false).ConfigureAwait(false);

			this.WithNewUser();
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.Append;
			settings.DestinationFolderArtifactID = this.folderId;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			DocumentDto[] importData =
				{
					new DocumentDto("10001"),
				};

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobCompletedWithErrors(results, 1, 1);
			this.ValidateJobMessagesContainsText(results, "[Record Info: 0] Error - [Line 1] - Your account does not have rights to add a document or object to this case");
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("E87C404D-EA4B-47C4-806B-C5F04ACBE8D8")]
		public void ShouldPreventOverlayWhenFolderIsSecured([Values(OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			this.WithNewUser();
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = overwriteMode;
			settings.DestinationFolderArtifactID = this.folderId;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			DocumentDto[] importData =
				{
					new DocumentDto(this.importedDocuments[0].ControlNumber),
				};

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobCompletedWithErrors(results, 1, 1);
			this.ValidateJobMessagesContainsText(results, "[Record Info: 0] Error - [Line 1] - The document specified has been secured for editing");
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("480C870C-18E4-4220-A7B8-E669E3EF61C2")]
		public void ShouldPreventOverlayWhenDocumentIsSecured([Values(OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			this.WithNewUser();
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = overwriteMode;
			settings.DestinationFolderArtifactID = this.folderId;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			DocumentDto[] importData =
				{
					new DocumentDto(this.importedDocuments[0].ControlNumber),
				};

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobCompletedWithErrors(results, 1, 1);
			this.ValidateJobMessagesContainsText(results, "[Record Info: 0] Error - [Line 1] - The document specified has been secured for editing");
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

		private class DocumentDto
		{
			public DocumentDto(string controlNumber)
			{
				this.ControlNumber = controlNumber;
			}

			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Must be public for DataReader")]
			[System.ComponentModel.DisplayName(WellKnownFields.ControlNumber)]
			public string ControlNumber { get; }

			public override string ToString()
			{
				return $"{ControlNumber}";
			}
		}

		private class DocumentWithArtifactFieldDto
		{
			public DocumentWithArtifactFieldDto(string controlNumber, int artifactId)
			{
				this.ControlNumber = controlNumber;
				this.ArtifactId = artifactId;
			}

			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Must be public for DataReader")]
			[System.ComponentModel.DisplayName(WellKnownFields.ControlNumber)]
			public string ControlNumber { get; }

			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Must be public for DataReader")]
			[System.ComponentModel.DisplayName(WellKnownFields.ArtifactId)]
			public int ArtifactId { get; }

			public override string ToString()
			{
				return $"{ControlNumber}_{ArtifactId}";
			}
		}
	}
}
