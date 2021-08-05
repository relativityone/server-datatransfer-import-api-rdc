// <copyright file="KeplerDistributedReplacementTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service.RelativityDistributed
{
	using System;
	using System.IO;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Credentials;
	using kCura.WinEDDS.Service.Kepler;

	using Moq;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.Service.RelativityDistributed;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Kepler.Transport;
	using Relativity.Logging;
	using Relativity.Services.FileSystem;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi]
	public class KeplerDistributedReplacementTests
	{
		private IntegrationTestParameters testParameters;
		private NetworkCredential validCredentials;
		private NetworkCredential invalidCredentials;

		[SetUp]
		public void SetUp()
		{
			this.testParameters = AssemblySetup.TestParameters.DeepCopy();
			RelativityWebApiCredentialsProvider.Instance().SetProvider(null);

			Assert.That(
				this.testParameters.WorkspaceId,
				Is.Positive,
				() => "The test workspace must be created or specified in order to run this integration test.");

			this.validCredentials = new NetworkCredential(
				this.testParameters.RelativityUserName,
				this.testParameters.RelativityPassword);
			this.invalidCredentials = new NetworkCredential(
				this.testParameters.RelativityUserName,
				"WRONG PASSWORD");
		}

		[IdentifiedTest("4bb77ae9-aa39-4d4c-8412-8fc012192e49")]
		[TestType.MainFlow]
		[IgnoreIfVersionLowerThan(RelativityVersion.Indigo)] // IFileSystemManager exists since Indigo release
		public Task ShouldDownloadFileAsync()
		{
			return this.DownloadFileAsync(this.validCredentials);
		}

		[IdentifiedTest("1471957f-7c70-46be-bbe8-25d94be8c573")]
		[TestType.Error]
		public void ShouldReturnNotFoundForFileWhichDoesNotExist()
		{
			// arrange
			string guid = Guid.NewGuid().ToString();
			var request = new FileDownloadRequest("localFile.txt", this.testParameters.WorkspaceId.ToString(), guid);

			var (sut, disposable) = this.CreateSut(this.validCredentials);
			FileDownloadResponse actualResponse;
			using (disposable)
			{
				// act
				actualResponse = sut.DownloadFile(request);
			}

			// assert
			Assert.That(actualResponse.IsSuccess, Is.False, "It should fail, because file does not exist");
			Assert.That(actualResponse.ErrorType, Is.EqualTo(RelativityDistributedErrorType.NotFound));
		}

		[IdentifiedTest("eefa8d95-b355-4e12-9a9c-5969e66590b6")]
		[TestType.Error]
		public void ShouldReturnNotAuthenticated()
		{
			// arrange
			string guid = Guid.NewGuid().ToString();
			var request = new FileDownloadRequest("localFile.txt", this.testParameters.WorkspaceId.ToString(), guid);

			var (sut, disposable) = this.CreateSut(this.invalidCredentials);
			FileDownloadResponse actualResponse;
			using (disposable)
			{
				// act
				actualResponse = sut.DownloadFile(request);
			}

			// assert
			Assert.That(actualResponse.IsSuccess, Is.False, "It should fail, because token is not correct");
			Assert.That(actualResponse.ErrorType, Is.EqualTo(RelativityDistributedErrorType.Authentication));
		}

		[IdentifiedTest("d603b18b-8cf8-437d-9aca-463ab7045393")]
		[TestType.EdgeCase]
		public Task ShouldUpdateInvalidCredentials()
		{
			RelativityWebApiCredentialsProvider.Instance().SetProvider(new TokenProvider(this.validCredentials));
			return this.DownloadFileAsync(this.invalidCredentials);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "It is disposed in tests")]
		private (KeplerDistributedReplacement, IDisposable) CreateSut(NetworkCredential credential)
		{
			var loggerMock = new Mock<ILog>();
			var connectionInfo = new KeplerServiceConnectionInfo(this.testParameters.RelativityWebApiUrl, credential);
			var serviceProxyFactory = new KeplerServiceProxyFactory(connectionInfo);
			var keplerProxy = new KeplerProxy(serviceProxyFactory, loggerMock.Object);

			var sut = new KeplerDistributedReplacement(
				keplerProxy,
				FileSystem.Instance.File,
				loggerMock.Object,
				() => string.Empty);

			return (sut, serviceProxyFactory);
		}

		private async Task DownloadFileAsync(NetworkCredential credentials)
		{
			// arrange
			string downloadedFilePath = "localFile.txt";
			string remoteFileName = Guid.NewGuid().ToString();
			string documentFolder = await WorkspaceHelper.GetDefaultFileRepositoryAsync(this.testParameters).ConfigureAwait(false);
			string remoteFilePath = Path.Combine(documentFolder, remoteFileName);
			var uploadedFilePath = ResourceFileHelper.GetResourceFilePath("Media", "AZIPPER_0011111.jpg");

			using (var fileSystemManager = ServiceHelper.GetServiceProxy<IFileSystemManager>(this.testParameters))
			{
				try
				{
					using (var fileStream = File.Open(uploadedFilePath, FileMode.Open, FileAccess.Read))
					using (var keplerStream = new KeplerStream(fileStream))
					{
						await fileSystemManager.UploadFileAsync(keplerStream, remoteFilePath).ConfigureAwait(false);
					}

					var downloadRequest = new FileDownloadRequest(
						downloadedFilePath,
						this.testParameters.WorkspaceId.ToString(),
						remoteFileName);

					var (sut, disposable) = this.CreateSut(credentials);
					FileDownloadResponse actualResponse;
					using (disposable)
					{
						// act
						actualResponse = sut.DownloadFile(downloadRequest);
					}

					// assert
					Assert.That(
						actualResponse.IsSuccess,
						Is.True,
						"It should succeed, because error file exist on the server");
					FileAssert.AreEqual(uploadedFilePath, downloadedFilePath);
				}
				finally
				{
					// clean
					await fileSystemManager.DeleteAsync(remoteFilePath, recursive: false).ConfigureAwait(false);
					File.Delete(downloadedFilePath);
				}
			}
		}

		private class TokenProvider : ICredentialsProvider
		{
			private readonly NetworkCredential credential;

			public TokenProvider(NetworkCredential credential)
			{
				this.credential = credential;
			}

			public NetworkCredential GetCredentials()
			{
				return this.credential;
			}

			public Task<NetworkCredential> GetCredentialsAsync(CancellationToken cancellationToken)
			{
				return Task.FromResult(this.GetCredentials());
			}
		}
	}
}
