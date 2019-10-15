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
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
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
		public void ShouldExportAllSampleDocAndImages(TapiClient client)
		{
			if ((client == TapiClient.Aspera && this.TestParameters.SkipAsperaModeTests) ||
				(client == TapiClient.Direct && this.TestParameters.SkipDirectModeTests))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"{client}");
			}

			// ARRANGE
			GivenTheTapiForceClientAppSettings(client);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(ExporterTestData.AllSampleFiles.Count());
		}

		[IdentifiedTest("3B50E3A9-0A28-4FA4-9ACD-5FB878DEF97A")]
		[TestCase(false)]
		[TestCase(true)]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public void ShouldExportWhenTheFileStorageSearchResultsAreEmpty(bool cloudInstance)
		{
			// ARRANGE
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedSearchResultsAreEmpty(cloudInstance);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(ExporterTestData.AllSampleFiles.Count());
			this.ThenTheMockSearchFileStorageAsyncIsVerified();
		}

		[IdentifiedTest("F8F28759-EC5A-4C03-95A3-70ACB005BCCE")]
		[TestCase(false)]
		[TestCase(true)]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public void ShouldExportWhenTheFileStorageSearchResultsAreInvalid(bool cloudInstance)
		{
			// ARRANGE
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedSearchResultsAreInvalid(cloudInstance);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(ExporterTestData.AllSampleFiles.Count());
			this.ThenTheMockSearchFileStorageAsyncIsVerified();
		}

		[IdentifiedTest("77A786A1-58E5-45E3-B0BF-CB70D3FFCE62")]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public void ShouldExportWhenTheFileStorageSearchThrowsNonFatalException()
		{
			// ARRANGE
			const bool Fatal = false;
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedFileStorageSearchThrows(Fatal);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(ExporterTestData.AllSampleFiles.Count());
			this.ThenTheMockSearchFileStorageAsyncIsVerified();
		}

		[IdentifiedTest("8DFA89C0-EB36-446B-92BC-2A0D8314ECD8")]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public void ShouldNotExportWhenTheFileStorageSearchThrowsFatalException()
		{
			// ARRANGE
			const bool Fatal = true;
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedFileStorageSearchThrows(Fatal);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			const bool ExpectedSearchResult = true;
			this.ThenTheExportJobIsFatal(ExpectedSearchResult);
			this.ThenTheMockSearchFileStorageAsyncIsVerified();
		}

		[IdentifiedTest("14A8EB3C-5662-428C-B1E6-FA95E8C79259")]
		[TestCase(false)]
		[TestCase(true)]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public void ShouldExportWhenTheSettingsForFileShareIsNull(bool forceAsperaClient)
		{
			// ARRANGE
			GivenTheTapiForceClientAppSettings(forceAsperaClient ? TapiClient.Aspera : TapiClient.None);
			this.GivenTheMockFileShareSettingsServiceIsRegistered();
			this.GivenTheMockedSettingsForFileShareIsNull();

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(ExporterTestData.AllSampleFiles.Count());
			this.ThenTheMockFileShareSettingsServiceIsVerified();
		}

		[IdentifiedTest("1AB462A0-AF45-4D4E-99DB-43FF74D44131")]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public void ShouldExportWhenTheNativeSourceLocationIsInvalid()
		{
			// ARRANGE
			const bool ValidNativeGuid = true;
			const bool ValidNativeSourceLocation = false;
			const bool ValidDestinationLocation = true;
			var fileExportRequests = new List<ExportRequest>
			{
				this.CreateTestPhysicalFileExportRequest(
					1,
					1,
					ValidNativeGuid,
					ValidNativeSourceLocation,
					ValidDestinationLocation),
				this.CreateTestFieldFileExportRequest(
					2,
					2,
					2,
					ValidNativeGuid,
					ValidNativeSourceLocation,
					ValidDestinationLocation),
			};

			this.GivenTheMockExportRequestRetrieverIsRegistered();
			this.GivenTheMockedExportRequestRetrieverReturns(fileExportRequests, new List<LongTextExportRequest>());

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			const int ExpectedProcessedCount = 2;
			this.ThenTheExportJobIsNotSuccessful(ExpectedProcessedCount);
		}

		[IdentifiedTest("76FB096D-7948-4BFE-8CED-7E509505CA95")]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public void ShouldExportWhenTheDestinationLocationIsInvalid()
		{
			// ARRANGE
			const bool ValidNativeGuid = true;
			const bool ValidNativeSourceLocation = true;
			const bool ValidDestinationLocation = false;
			var fileExportRequests = new List<ExportRequest>
			{
				this.CreateTestPhysicalFileExportRequest(
					1,
					1,
					ValidNativeGuid,
					ValidNativeSourceLocation,
					ValidDestinationLocation),
				this.CreateTestFieldFileExportRequest(
					2,
					2,
					2,
					ValidNativeGuid,
					ValidNativeSourceLocation,
					ValidDestinationLocation),
			};

			var longTextExportRequests = new List<LongTextExportRequest>
			{
				this.CreateTestLongTextExportRequest(
					3,
					3,
					3,
					ValidNativeSourceLocation,
					ValidDestinationLocation),
			};

			this.GivenTheMockExportRequestRetrieverIsRegistered();
			this.GivenTheMockedExportRequestRetrieverReturns(fileExportRequests, longTextExportRequests);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			const int ExpectedProcessedCount = 3;
			this.ThenTheExportJobIsNotSuccessful(ExpectedProcessedCount);
		}

		private void ExecuteFolderAndSubfoldersAndVerify()
		{
			// Note: the sample dataset is imported only 1 time for all tests.
			this.GivenTheExportType(ExportFile.ExportType.ParentSearch);
			this.GivenTheFilesAreImported(null, ExporterTestData.AllSampleFiles);
			CaseInfo caseInfo = this.WhenGettingTheWorkspaceInfo();
			this.GivenTheSelectedFolderId(caseInfo.RootFolderID);
			this.GivenTheIdentifierColumnName(WellKnownFields.ControlNumber);
			this.WhenCreatingTheExportFile(caseInfo);
			this.WhenExportingTheDocs();
		}
	}
}