﻿// -----------------------------------------------------------------------------------------------------
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

	[Feature.DataTransfer.RelativityDesktopClient.Export]
	[Category(TestCategories.Export)]
	[Category(TestCategories.Integration)]
	public class FolderAndSubFolderExportTests : ExportTestBase
	{
		protected override IntegrationTestParameters TestParameters => AssemblySetup.TestParameters;

		[IdentifiedTest("3B50E3A9-0A28-4FA4-9ACD-5FB878DEF97A")]
		[TestCase(false)]
		[TestCase(true)]
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
		}

		[IdentifiedTest("F8F28759-EC5A-4C03-95A3-70ACB005BCCE")]
		[TestCase(false)]
		[TestCase(true)]
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
		}

		[IdentifiedTest("77A786A1-58E5-45E3-B0BF-CB70D3FFCE62")]
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA1801: Review unused parameters",
			Justification = "We are using TestExecutionContext.CurrentContext.CurrentTest.Arguments to retrieve value of client parameter.")]
		[IdentifiedTest("14A8EB3C-5662-428C-B1E6-FA95E8C79259")]
		[TestCase(TapiClient.None)]
		[TestCase(TapiClient.Aspera)]
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
		}

		/// <summary>
		/// This test should not verify any load file content and physical download export logic
		/// It is used only to validate the total number of record processed is correct and the given exception is thrown on invalid source location path.
		/// </summary>
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
			// This is the only way how can we currently combine specific download issue validation with total records processed count implementation in DownloadProgressManager class
			ValidateNativeFileRequestIssue(fileExportRequests);
			this.ThenTheExportJobIsNotSuccessful(TestData.SampleDocFiles.Count() + fileExportRequests.Count);
		}

		/// <summary>
		/// This test should not verify any load file content and physical download export logic
		/// It is used only to validate the total number of record processed is correct and the given exception is thrown on invalid destination location path.
		/// </summary>
		[IdentifiedTest("76FB096D-7948-4BFE-8CED-7E509505CA95")]
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
			// This is the only way how can we currently combine specific download issue validation with total records processed count implementation in DownloadProgressManager class
			ValidateNativeFileRequestIssue(fileExportRequests);
			this.Logger.NullLoggerMock.Verify(
				x => x.LogWarning(It.IsAny<ArgumentException>(), It.Is<string>(msg => msg.Contains(MsgTextFileDownloadIssueMsg)), longTextExportRequests[0].ArtifactId), Times.Once);

			this.ThenTheExportJobIsNotSuccessful(TestData.SampleDocFiles.Count() + fileExportRequests.Count + longTextExportRequests.Count);
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