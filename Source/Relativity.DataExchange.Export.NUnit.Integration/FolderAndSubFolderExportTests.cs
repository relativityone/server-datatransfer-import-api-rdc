// -----------------------------------------------------------------------------------------------------
// <copyright file="FolderAndSubFolderExportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents all folder and sub-folder export integration tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents all folder and sub-folder export integration tests.
	/// </summary>
	[TestFixture("utf-8", "utf-8")]
	[TestFixture("utf-8", "utf-16")]
	[TestFixture("utf-16", "utf-16")]
	[TestFixture("utf-16", "utf-8")]
	[Feature.DataTransfer.RelativityDesktopClient.Export]
	public class FolderAndSubFolderExportTests : ExporterTestBase
	{
		public FolderAndSubFolderExportTests(string loadFileEncoding, string textFileEncoding)
			: base(loadFileEncoding, textFileEncoding)
		{
		}

		[IdentifiedTest("5b20c6f1-1196-41ea-9326-0e875e2cabe9")]
		[TestCase(TapiClient.Aspera)]
		[TestCase(TapiClient.Direct)]
		[TestCase(TapiClient.Web)]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public async Task ShouldExportAllSampleDocAndImagesAsync(TapiClient client)
		{
			if ((client == TapiClient.Aspera && this.TestParameters.SkipAsperaModeTests) ||
			    (client == TapiClient.Direct && this.TestParameters.SkipDirectModeTests))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"{client}");
			}

			this.GivenTheTapiForceClientAppSettings(client);
			await this.ExecuteFolderAndSubfoldersAndVerifyAsync().ConfigureAwait(false);
			this.ThenTheExportJobIsSuccessful(AllSampleFiles.Count);
		}

		[IdentifiedTest("3B50E3A9-0A28-4FA4-9ACD-5FB878DEF97A")]
		[TestCase(false)]
		[TestCase(true)]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public async Task ShouldExportWhenTheFileStorageSearchResultsAreEmptyAsync(bool cloudInstance)
		{
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedSearchResultsAreEmpty(cloudInstance);
			await this.ExecuteFolderAndSubfoldersAndVerifyAsync().ConfigureAwait(false);
			this.ThenTheExportJobIsSuccessful(AllSampleFiles.Count);
			this.ThenTheTransferModeShouldEqualDirectOrWebMode();
			this.ThenTheMockSearchFileStorageAsyncIsVerified();
		}

		[IdentifiedTest("F8F28759-EC5A-4C03-95A3-70ACB005BCCE")]
		[TestCase(false)]
		[TestCase(true)]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public async Task ShouldExportWhenTheFileStorageSearchResultsAreInvalidAsync(bool cloudInstance)
		{
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedSearchResultsAreInvalid(cloudInstance);
			await this.ExecuteFolderAndSubfoldersAndVerifyAsync().ConfigureAwait(false);
			this.ThenTheExportJobIsSuccessful(AllSampleFiles.Count);
			this.ThenTheTransferModeShouldEqualDirectOrWebMode();
			this.ThenTheMockSearchFileStorageAsyncIsVerified();
		}

		[IdentifiedTest("77A786A1-58E5-45E3-B0BF-CB70D3FFCE62")]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public async Task ShouldExportWhenTheFileStorageSearchThrowsNonFatalExceptionAsync()
		{
			const bool Fatal = false;
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedFileStorageSearchThrows(Fatal);
			await this.ExecuteFolderAndSubfoldersAndVerifyAsync().ConfigureAwait(false);
			this.ThenTheExportJobIsSuccessful(AllSampleFiles.Count);
			this.ThenTheTransferModeShouldEqualDirectOrWebMode();
			this.ThenTheMockSearchFileStorageAsyncIsVerified();
		}

		[IdentifiedTest("8DFA89C0-EB36-446B-92BC-2A0D8314ECD8")]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public async Task ShouldNotExportWhenTheFileStorageSearchThrowsFatalExceptionAsync()
		{
			const bool Fatal = true;
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedFileStorageSearchThrows(Fatal);
			await this.ExecuteFolderAndSubfoldersAndVerifyAsync().ConfigureAwait(false);
			this.ThenTheExportJobIsFatal();
			this.ThenTheMockSearchFileStorageAsyncIsVerified();
		}

		[IdentifiedTest("14A8EB3C-5662-428C-B1E6-FA95E8C79259")]
		[TestCase(false)]
		[TestCase(true)]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public async Task ShouldExportWhenTheSettingsForFileShareIsNullAsync(bool forceAsperaClient)
		{
			this.GivenTheTapiForceClientAppSettings(forceAsperaClient ? TapiClient.Aspera : TapiClient.None);
			this.GivenTheMockFileShareSettingsServiceIsRegistered();
			this.GivenTheMockedSettingsForFileShareIsNull();
			await this.ExecuteFolderAndSubfoldersAndVerifyAsync().ConfigureAwait(false);
			this.ThenTheTransferModeShouldEqualDirectOrWebMode();
			this.ThenTheExportJobIsSuccessful(AllSampleFiles.Count);
			this.ThenTheMockFileShareSettingsServiceIsVerified();
		}

		private async Task ExecuteFolderAndSubfoldersAndVerifyAsync()
		{
			// Note: the sample dataset is imported only 1 time for all tests.
			this.GivenTheExportType(ExportFile.ExportType.ParentSearch);
			this.GivenTheFilesAreImported(AllSampleFiles);
			CaseInfo caseInfo = await this.WhenGettingTheWorkspaceInfoAsync().ConfigureAwait(false);
			this.GivenTheSelectedFolderId(caseInfo.RootFolderID);
			this.GivenTheIdentifierColumnName(WellKnownFields.ControlNumber);
			await this.WhenCreatingTheExportFileAsync(caseInfo).ConfigureAwait(false);
			this.WhenExportingTheDocs();
		}
	}
}