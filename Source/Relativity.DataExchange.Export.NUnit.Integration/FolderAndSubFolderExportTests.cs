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
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[Feature.DataTransfer.RelativityDesktopClient.Export]
	[Category(TestCategories.Export)]
	[Category(TestCategories.Integration)]
	public class FolderAndSubFolderExportTests : ExportTestBase
	{
		protected override IntegrationTestParameters TestParameters => AssemblySetup.TestParameters;

		[IdentifiedTest("3B50E3A9-0A28-4FA4-9ACD-5FB878DEF97A")]
		[TestCase(false)]
		[TestCase(true)]
		public void ShouldExportWhenTheFileStorageSearchResultsAreEmpty(bool cloudInstance)
		{
			// ARRANGE
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedSearchResultsAreEmpty(cloudInstance);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(TestData.SampleDocFiles.Count());
			this.ThenTheMockSearchFileStorageAsyncIsVerified();
		}

		[IdentifiedTest("F8F28759-EC5A-4C03-95A3-70ACB005BCCE")]
		[TestCase(false)]
		[TestCase(true)]
		public void ShouldExportWhenTheFileStorageSearchResultsAreInvalid(bool cloudInstance)
		{
			// ARRANGE
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedSearchResultsAreInvalid(cloudInstance);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(TestData.SampleDocFiles.Count());
			this.ThenTheMockSearchFileStorageAsyncIsVerified();
		}

		[IdentifiedTest("77A786A1-58E5-45E3-B0BF-CB70D3FFCE62")]
		public void ShouldExportWhenTheFileStorageSearchThrowsNonFatalException()
		{
			// ARRANGE
			const bool Fatal = false;
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedFileStorageSearchThrows(Fatal);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(TestData.SampleDocFiles.Count());
			this.ThenTheMockSearchFileStorageAsyncIsVerified();
		}

		[IdentifiedTest("8DFA89C0-EB36-446B-92BC-2A0D8314ECD8")]
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
		[TestCase(TapiClient.None)]
		[TestCase(TapiClient.Aspera)]
		public void ShouldExportWhenTheSettingsForFileShareIsNull(TapiClient tapiClient)
		{
			// ARRANGE
			GivenTheTapiForceClientAppSettings(tapiClient);
			this.GivenTheMockedSettingsForFileShareIsNull();

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(TestData.SampleDocFiles.Count());
			this.ThenTheMockFileShareSettingsServiceIsVerified();
		}

		[IdentifiedTest("1AB462A0-AF45-4D4E-99DB-43FF74D44131")]
		public void ShouldExportWhenTheNativeSourceLocationIsInvalid()
		{
			// ARRANGE
			var nativeExportRequest = PhysicalFileExportRequest.CreateRequestForNative(
				new ObjectExportInfo
				{
					ArtifactID = 1,
					NativeSourceLocation = null,
					NativeFileGuid = System.Guid.NewGuid().ToString(),
				},
				System.IO.Path.Combine(this.TempDirectory.Directory, $"{Guid.NewGuid()}.doc"));
			nativeExportRequest.Order = 1;

			var fileExportRequests = new List<ExportRequest>
			{
				nativeExportRequest,
				new FieldFileExportRequest(
					new ObjectExportInfo
						{
							ArtifactID = 2,
							NativeSourceLocation = null,
							NativeFileGuid = System.Guid.NewGuid().ToString(),
						},
					fileFieldArtifactId: 2,
					destinationLocation: System.IO.Path.Combine(this.TempDirectory.Directory, $"{Guid.NewGuid()}.msg"))
					{
						Order = 2,
					},
			};

			this.GivenTheMockedExportRequestRetrieverReturns(fileExportRequests, new List<LongTextExportRequest>());

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			// Note: the Exporter artifact search doesn't support dependency injection and the DownloadProgressManage now performs a finalization that counts all search/DI artifacts.
			this.ThenTheExportJobIsNotSuccessful(TestData.SampleDocFiles.Count() + fileExportRequests.Count);
		}

		[IdentifiedTest("76FB096D-7948-4BFE-8CED-7E509505CA95")]
		public void ShouldExportWhenTheDestinationLocationIsInvalid()
		{
			// ARRANGE
			var nativeExportRequest = PhysicalFileExportRequest.CreateRequestForNative(
				new ObjectExportInfo
				{
					ArtifactID = 1,
					NativeSourceLocation = $"{TestData.DummyUncPath}{Guid.NewGuid()}.doc",
					NativeFileGuid = System.Guid.NewGuid().ToString(),
				},
				destinationLocation: null);
			nativeExportRequest.Order = 1;

			var fileExportRequests = new List<ExportRequest>
			{
				nativeExportRequest,
				new FieldFileExportRequest(
					new ObjectExportInfo
						{
							ArtifactID = 2,
							NativeSourceLocation = $"{TestData.DummyUncPath}{Guid.NewGuid()}.msg",
							NativeFileGuid = System.Guid.NewGuid().ToString(),
						},
					fileFieldArtifactId: 2,
					destinationLocation: null)
					{
						Order = 2,
					},
			};

			var longTextExportRequests = new List<LongTextExportRequest>
			{
				((Func<LongTextExportRequest>)(() =>
					{
						LongTextExportRequest request = LongTextExportRequest.CreateRequestForLongText(
							new ObjectExportInfo
								{
									ArtifactID = 3,
									NativeSourceLocation = $"{TestData.DummyUncPath}{Guid.NewGuid()}.txt",
									NativeFileGuid = null,
								},
							fieldArtifactId: 3,
							destinationLocation: null);
						request.Order = 3;
						return request;
					}))(),
			};

			this.GivenTheMockedExportRequestRetrieverReturns(fileExportRequests, longTextExportRequests);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			// Note: the Exporter artifact search doesn't support dependency injection and the DownloadProgressManage now performs a finalization that counts all search/DI artifacts.
			this.ThenTheExportJobIsNotSuccessful(TestData.SampleDocFiles.Count() + fileExportRequests.Count + longTextExportRequests.Count);
		}
	}
}