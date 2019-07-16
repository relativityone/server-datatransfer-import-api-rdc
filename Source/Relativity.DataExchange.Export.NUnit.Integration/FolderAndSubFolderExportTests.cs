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
	using System;
	using System.Text;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents all folder and sub-folder export integration tests.
	/// </summary>
	[Feature.DataTransfer.RelativityDesktopClient.Export]
	public class FolderAndSubFolderExportTests : ExporterTestBase
	{
		[IdentifiedTest("5b20c6f1-1196-41ea-9326-0e875e2cabe9")]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public async Task ShouldExportAllSampleDocAndImagesAsync()
		{
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
		}

		[IdentifiedTest("14A8EB3C-5662-428C-B1E6-FA95E8C79259")]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public async Task ShouldExportWhenTheSettingsForFileShareIsNullAsync()
		{
			this.GivenTheMockFileShareSettingsServiceIsRegistered();
			this.GivenTheMockedSettingsForFileShareIsNull();
			await this.ExecuteFolderAndSubfoldersAndVerifyAsync().ConfigureAwait(false);
			this.ThenTheTransferModeShouldEqualDirectOrWebMode();
			this.ThenTheExportJobIsSuccessful(AllSampleFiles.Count);
		}

		private async Task ExecuteFolderAndSubfoldersAndVerifyAsync()
		{
			// Note: the sample dataset is imported only 1 time for all tests.
			this.GivenTheExportType(ExportFile.ExportType.ParentSearch);
			this.GivenTheFilesAreImported(AllSampleFiles);
			CaseInfo caseInfo = await this.WhenGettingTheWorkspaceInfoAsync().ConfigureAwait(false);
			this.GivenTheSelectedFolderId(caseInfo.RootFolderID);
			this.GivenTheIdentifierColumnName(WellKnownFields.ControlNumber);
			this.GivenTheEncoding(Encoding.Unicode);
			await this.WhenCreatingTheExportFileAsync(caseInfo).ConfigureAwait(false);
			this.WhenExportingTheDocs();
		}
	}
}