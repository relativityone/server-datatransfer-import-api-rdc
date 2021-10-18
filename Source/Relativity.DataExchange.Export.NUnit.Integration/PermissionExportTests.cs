// <copyright file="PermissionExportTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[TestType.MainFlow]
	[Feature.DataTransfer.DocumentExportApi.Operations.ExportFolderAndSubfolders]
	public class PermissionExportTests : ExportTestBase
	{
		private const RelativityVersion MinSupportedVersion = RelativityVersion.Foxglove;
		private bool testsSkipped;
		private int groupId;
		private int userId;
		private string newUsername;
		private string newPassword;
		private string oldUsername;
		private string oldPassword;
		private IntegrationTestParameters testParameters;

		public PermissionExportTests(bool useKepler)
			: base(useKepler)
		{
		}

		protected override IntegrationTestParameters TestParameters => testParameters;

		[OneTimeSetUp]
		public async Task OneTimeSetupAsync()
		{
			testParameters = IntegrationTestHelper.Create();

			testsSkipped = RelativityVersionChecker.VersionIsLowerThan(
				this.TestParameters,
				MinSupportedVersion);
			if (!testsSkipped)
			{
				this.oldUsername = this.TestParameters.RelativityUserName;
				this.oldPassword = this.TestParameters.RelativityPassword;

				this.newUsername = $"ImportAPI.{Guid.NewGuid()}@relativity.com";
				this.newPassword = "Test1234!";
				this.userId = await UsersHelper.CreateNewUserAsync(
					              this.TestParameters,
					              this.newUsername,
					              this.newPassword,
					              new List<int> { GroupHelper.EveryoneGroupId })
					              .ConfigureAwait(false);

				ImportDocumentsAndImages();
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

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA1801: Review unused parameters",
			Justification = "We are using TestExecutionContext.CurrentContext.CurrentTest.Arguments to retrieve value of client parameter.")]
		[IdentifiedTestCase("2013DF1B-46DE-4755-B96B-AF454CA5D2CE", TapiClient.Direct)]
		[IdentifiedTestCase("2CE35FD2-C6E4-46F0-A3DD-EA11C6C64E36", TapiClient.Web)]
		public async Task ExportShouldExecuteWithMinimalSetOfPermissions(TapiClient client)
		{
			// ARRANGE
			ExtendedExportFileSetup.SetupDocumentExport(ExtendedExportFile);
			ExtendedExportFileSetup.SetupImageExport(ExtendedExportFile);
			ExtendedExportFileSetup.SetupPaddings(ExtendedExportFile);
			ExtendedExportFileSetup.SetupDelimiters(ExtendedExportFile);

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
					new List<string> { "Allow Export" },
					true)
				.ConfigureAwait(false);

			this.WithNewUser();

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			int expectedDocumentsCount = TestData.SampleDocFiles.Count();
			int imagesPerDocumentCount = TestData.SampleImageFiles.Count();
			int metadataFilesCount = 2;
			int expectedExportedFilesCount = metadataFilesCount + expectedDocumentsCount + (expectedDocumentsCount * imagesPerDocumentCount);

			// ASSERT
			this.ThenTheExportJobIsSuccessful(TestData.SampleDocFiles.Count());

			ThenTheFilesAreExported(expectedExportedFilesCount);

			this.ThenTheCorrelationIdWasRetrieved();
		}

		private void ImportDocumentsAndImages()
		{
			var documents = ImportHelper.ImportDocuments(TestParameters);
			ImportHelper.ImportImagesForDocuments(TestParameters, documents);
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