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
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.DocumentExportApi.Operations.ExportFolderAndSubfolders]
	public class FolderAndSubFolderExportTests : ExportTestBase
	{
		protected override IntegrationTestParameters TestParameters => AssemblySetup.TestParameters;

		[IdentifiedTestCase("3B50E3A9-0A28-4FA4-9ACD-5FB878DEF97A", false)]
		[IdentifiedTestCase("12EC9F92-DE5E-4731-A423-96E0ABE84BD2", true)]
		[TestType.Error]
		public async Task ShouldExportWhenTheFileStorageSearchResultsAreEmptyAsync(bool cloudInstance)
		{
			// ARRANGE
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedSearchResultsAreEmpty(cloudInstance);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(TestData.SampleDocFiles.Count());
			this.ThenTheMockSearchFileStorageAsyncIsVerified();

			await this.ThenTheExportedDocumentLoadFileIsAsExpectedAsync().ConfigureAwait(false);
			await this.ThenTheExportedImageLoadFileIsAsExpectedAsync().ConfigureAwait(false);

			this.ThenTheCorrelationIdWasRetrieved();
		}

		[IdentifiedTestCase("F8F28759-EC5A-4C03-95A3-70ACB005BCCE", false)]
		[IdentifiedTestCase("4B7B8DA4-7875-4C7B-B65B-4CB9FF94F8BB", true)]
		[TestType.Error]
		public async Task ShouldExportWhenTheFileStorageSearchResultsAreInvalidAsync(bool cloudInstance)
		{
			// ARRANGE
			this.GivenTheMockTapiObjectServiceIsRegistered();
			this.GivenTheMockedSearchResultsAreInvalid(cloudInstance);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(TestData.SampleDocFiles.Count());
			this.ThenTheMockSearchFileStorageAsyncIsVerified();

			await this.ThenTheExportedDocumentLoadFileIsAsExpectedAsync().ConfigureAwait(false);
			await this.ThenTheExportedImageLoadFileIsAsExpectedAsync().ConfigureAwait(false);

			this.ThenTheCorrelationIdWasRetrieved();
		}

		[IdentifiedTest("77A786A1-58E5-45E3-B0BF-CB70D3FFCE62")]
		[TestType.Failure]
		public async Task ShouldExportWhenTheFileStorageSearchThrowsNonFatalExceptionAsync()
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

			await this.ThenTheExportedDocumentLoadFileIsAsExpectedAsync().ConfigureAwait(false);
			await this.ThenTheExportedImageLoadFileIsAsExpectedAsync().ConfigureAwait(false);

			this.ThenTheCorrelationIdWasRetrieved();
		}

		[IdentifiedTest("8DFA89C0-EB36-446B-92BC-2A0D8314ECD8")]
		[TestType.Failure]
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

			this.ThenTheCorrelationIdWasRetrieved();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA1801: Review unused parameters",
			Justification = "We are using TestExecutionContext.CurrentContext.CurrentTest.Arguments to retrieve value of client parameter.")]
		[IdentifiedTestCase("14A8EB3C-5662-428C-B1E6-FA95E8C79259", TapiClient.None)]
		[IdentifiedTestCase("76895B54-BD41-4C02-8C53-EC18E68BA98D", TapiClient.Aspera)]
		[TestType.Error]
		public async Task ShouldExportWhenTheSettingsForFileShareIsNullAsync(TapiClient tapiClient)
		{
			// ARRANGE
			this.GivenTheMockedSettingsForFileShareIsNull();

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(TestData.SampleDocFiles.Count());
			this.ThenTheMockFileShareSettingsServiceIsVerified();

			await this.ThenTheExportedDocumentLoadFileIsAsExpectedAsync().ConfigureAwait(false);
			await this.ThenTheExportedImageLoadFileIsAsExpectedAsync().ConfigureAwait(false);

			this.ThenTheCorrelationIdWasRetrieved();
		}

		/// <summary>
		/// This test should not verify any load file content and physical download export logic
		/// It is used only to validate the total number of record processed is correct and the given exception is thrown on invalid source location path.
		/// </summary>
		[IdentifiedTest("1AB462A0-AF45-4D4E-99DB-43FF74D44131")]
		[TestType.Error]
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
			// This is the only way how can we currently combine specific download issue validation with total records processed count implementation in DownloadProgressManager class
			ValidateNativeFileRequestIssue(fileExportRequests);
			this.ThenTheExportJobIsNotSuccessful(TestData.SampleDocFiles.Count() + fileExportRequests.Count);

			this.ThenTheCorrelationIdWasRetrieved();
		}

		/// <summary>
		/// This test should not verify any load file content and physical download export logic
		/// It is used only to validate the total number of record processed is correct and the given exception is thrown on invalid destination location path.
		/// </summary>
		[IdentifiedTest("76FB096D-7948-4BFE-8CED-7E509505CA95")]
		[TestType.Error]
		public void ShouldExportWhenTheDestinationLocationIsInvalid()
		{
			// ARRANGE
			const string MsgTextFileDownloadIssueMsg = "There was a problem with the request details that prevented retrieving the long text data from artifact {ArtifactId}";

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
						var request = LongTextExportRequest.CreateRequestForLongText(
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
			// This is the only way how can we currently combine specific download issue validation with total records processed count implementation in DownloadProgressManager class
			this.ValidateNativeFileRequestIssue(fileExportRequests);
			this.Logger.NullLoggerMock.Verify(
				x => x.LogWarning(It.IsAny<ArgumentException>(), It.Is<string>(msg => msg.Contains(MsgTextFileDownloadIssueMsg)), longTextExportRequests[0].ArtifactId), Times.Once);

			this.ThenTheExportJobIsNotSuccessful(TestData.SampleDocFiles.Count() + fileExportRequests.Count + longTextExportRequests.Count);

			this.ThenTheCorrelationIdWasRetrieved();
		}

		private void ValidateNativeFileRequestIssue(List<ExportRequest> fileExportRequests)
		{
			const string MsgNativeFileDownloadIssueMsg = "There was a problem downloading artifact {ArtifactId}";

			foreach (var fileExportRequest in fileExportRequests)
			{
				this.Logger.NullLoggerMock.Verify(
					x => x.LogWarning(It.IsAny<ArgumentException>(), It.Is<string>(msg => msg.Contains(MsgNativeFileDownloadIssueMsg)), fileExportRequest.ArtifactId), Times.Once);
			}
		}
	}
}