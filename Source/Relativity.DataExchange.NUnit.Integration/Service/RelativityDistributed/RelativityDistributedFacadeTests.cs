﻿// <copyright file="RelativityDistributedFacadeTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service.RelativityDistributed
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Service;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Service.RelativityDistributed;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1;
	using Relativity.Kepler.Transport;
	using Relativity.Services.FileSystem;
	using Relativity.Testing.Identification;

	using File = System.IO.File;

	[TestFixture]
	[Feature.DataTransfer.ImportApi]
	[Feature.DeveloperPlatform.ExtensibilityPoints.Api.WebApi]
	public class RelativityDistributedFacadeTests : WebServiceTestsBase
	{
		private IRelativityDistributedFacade sut;

		private string authenticationToken;

		private int originalWebBasedFileDownloadChunkSize;

		[IdentifiedTest("2d7d0e40-72d3-48fc-a1bc-459f597ed99b")]
		[TestType.MainFlow]
		public async Task ShouldDownloadFileAsync()
		{
			// arrange
			string downloadedFilePath = "localFile.txt";
			string remoteFileName = Guid.NewGuid().ToString();
			string documentFolder = await WorkspaceHelper.GetDefaultFileRepositoryAsync(this.TestParameters).ConfigureAwait(false);
			var uploadedFilePath = ResourceFileHelper.GetResourceFilePath("Media", "AZIPPER_0011111.jpg");
			var contentBytes = File.ReadAllBytes(uploadedFilePath);

			using (var service = ServiceHelper.GetServiceProxy<IFileIOService>(this.TestParameters))
			{
				var correlationId = Guid.NewGuid().ToString();
				try
				{
					await service.BeginFillAsync(
						this.TestParameters.WorkspaceId,
						contentBytes.Concat(new byte[] { 0x49 }).ToArray(),
						documentFolder,
						remoteFileName,
						correlationId).ConfigureAwait(false);

					var downloadRequest = new FileDownloadRequest(
						downloadedFilePath,
						this.TestParameters.WorkspaceId.ToString(),
						remoteFileName);

					// act
					var actualResponse = this.sut.DownloadFile(downloadRequest);

					// assert
					Assert.That(
						actualResponse.IsSuccess,
						Is.True,
						"It should succeed, because error file exist on the server");
					FileAssert.AreEqual(uploadedFilePath, downloadedFilePath);
				}
				finally
				{
					await service.RemoveFillAsync(
						this.TestParameters.WorkspaceId,
						documentFolder,
						remoteFileName,
						correlationId).ConfigureAwait(false);
					File.Delete(downloadedFilePath);
				}
			}
		}

		[IdentifiedTest("dd861031-fed3-4287-bca6-e21c2f2d1f6a")]
		[TestType.Error]
		public void ShouldReturnNotFoundForFileWhichDoesNotExist()
		{
			// arrange
			string guid = Guid.NewGuid().ToString();
			var request = new FileDownloadRequest("localFile.txt", this.TestParameters.WorkspaceId.ToString(), guid);

			// act
			FileDownloadResponse actualResponse = this.sut.DownloadFile(request);

			// assert
			Assert.That(actualResponse.IsSuccess, Is.False, "It should fail, because file does not exist");
			Assert.That(actualResponse.ErrorType, Is.EqualTo(RelativityDistributedErrorType.NotFound));
		}

		[IdentifiedTest("bce2497b-dd6f-44da-a88f-02608cd7a9d7")]
		[TestType.Error]
		public void ShouldReturnNotAuthenticated()
		{
			// arrange
			this.authenticationToken = "WRONG_TOKEN";

			string guid = Guid.NewGuid().ToString();
			var request = new FileDownloadRequest("localFile.txt", this.TestParameters.WorkspaceId.ToString(), guid);

			// act
			FileDownloadResponse actualResponse = this.sut.DownloadFile(request);

			// assert
			Assert.That(actualResponse.IsSuccess, Is.False, "It should fail, because token is not correct");
			Assert.That(actualResponse.ErrorType, Is.EqualTo(RelativityDistributedErrorType.Authentication));
		}

		protected override void OnSetup()
		{
			// We want to test that download works when file size is bigger than this value.
			this.originalWebBasedFileDownloadChunkSize = this.AppSettings.WebBasedFileDownloadChunkSize;
			this.AppSettings.WebBasedFileDownloadChunkSize = 1000;

			// We need to set this to the correct value before each test.
			this.authenticationToken = Settings.AuthenticationToken;

			// Create SUT instance
			IFile fileHelper = FileSystem.Instance.File;
			var relativityDistributedUri = new Uri(this.RelativityInstance.Host, new Uri("Relativity.Distributed", UriKind.Relative));
			string downloadHandlerUrl = relativityDistributedUri.AbsoluteUri;
			this.sut = new RelativityDistributedFacade(
				this.Logger.Object,
				this.AppSettings,
				fileHelper,
				downloadHandlerUrl,
				this.RelativityInstance.Credentials as NetworkCredential,
				this.RelativityInstance.CookieContainer,
				() => this.authenticationToken);
		}

		protected override void OnTeardown()
		{
			this.AppSettings.WebBasedFileDownloadChunkSize = this.originalWebBasedFileDownloadChunkSize;
		}
	}
}
