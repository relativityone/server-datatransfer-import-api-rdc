// ----------------------------------------------------------------------------
// <copyright file="ImagePermissionTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[TestType.Error]
	public class ImagePermissionTests : ImportJobTestBase<ImageImportExecutionContext>
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
			testsSkipped = RelativityVersionChecker.VersionIsLowerThan(this.TestParameters, MinSupportedVersion);
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
				await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document)
					.ConfigureAwait(false);
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

		[IgnoreIfVersionLowerThan(RelativityVersion.Sundrop)]
		[IdentifiedTest("6920c59d-8ffc-43e1-9dc5-15909dcd8d8a")]
		[TestExecutionCategory.CI]
		public async Task ShouldPreventImageImportWithoutSufficientPermissionsAsync()
		{
			// ARRANGE
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

			const int NumberOfDocumentsToImport = 10;
			const int NumberOfImagesPerDocument = 20;

			long expectedFileBytes = 0L;
			IEnumerable<ImageImportWithFileNameDto> importData = new ImageImportWithFileNameDtoBuilder(
					this.TempDirectory.Directory,
					NumberOfDocumentsToImport,
					NumberOfImagesPerDocument,
					ImageFormat.Jpeg)
				.WithFileSizeBytesAggregator(fileSize => expectedFileBytes += fileSize)
				.Build();

			var imageSettingsBuilder = new ImageSettingsBuilder()
				.WithDefaultFieldNames()
				.WithOverlayMode(OverwriteModeEnum.AppendOverlay);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(importData);

			// ASSERT
			ThenTheErrorRowsHaveCorrectMessage(result.ErrorRows, " - Your account does not have rights to add a document or object to this case");
			Assert.That(result.JobReportErrorsCount, Is.EqualTo(NumberOfDocumentsToImport * NumberOfImagesPerDocument));
			Assert.That(result.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(result.JobReportTotalRows, Is.EqualTo(NumberOfDocumentsToImport * NumberOfImagesPerDocument));
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