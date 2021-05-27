// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridgeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="DownloadTapiBridge2"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Net;
	using System.Reflection;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Services.Client;
	using Relativity.Transfer;

	/// <summary>
	/// Represents <see cref="DownloadTapiBridge2"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class DownloadTapiBridgeTests : TapiBridgeTestsBase<DownloadTapiBridge2>
	{
		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldThrowWhenTheConstructorArgsAreInvalid()
		{
			DownloadTapiBridgeParameters2 parameters = this.CreateDownloadTapiBridgeParameters(WellKnownTransferClient.Unassigned);
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						using (new DownloadTapiBridge2(
							null,
							parameters,
							this.MockLogger.Object,
							this.CancellationTokenSource.Token))
						{
						}
					});

			Assert.Throws<ArgumentNullException>(
				() =>
					{
						using (new DownloadTapiBridge2(
							this.MockTapiObjectService.Object,
							null,
							this.MockLogger.Object,
							this.CancellationTokenSource.Token))
						{
						}
					});

			parameters.WorkspaceId = 0;
			Assert.Throws<ArgumentOutOfRangeException>(
				() =>
					{
						using (new DownloadTapiBridge2(
							this.MockTapiObjectService.Object,
							parameters,
							this.MockLogger.Object,
							this.CancellationTokenSource.Token))
						{
						}
					});

			parameters.Credentials = null;
			Assert.Throws<ArgumentException>(
				() =>
					{
						using (new DownloadTapiBridge2(
							this.MockTapiObjectService.Object,
							parameters,
							this.MockLogger.Object,
							this.CancellationTokenSource.Token))
						{
						}
					});
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldLogTheTapiBridgeParameters()
		{
			this.CreateTapiBridge(WellKnownTransferClient.Unassigned);
			this.TapiBridgeInstance.LogTransferParameters();

			// Force somebody to review this test should the number of logged entries change.
			this.MockLogger.Verify(
				log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Exactly(23));
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldLogTheTapiBridgeParametersBeforeCreatingClient()
		{
			this.CreateTapiBridge(WellKnownTransferClient.Unassigned);
			this.TapiBridgeInstance.CreateTransferClient();

			// Force somebody to review this test should the number of logged entries change.
			this.MockLogger.Verify(
				log => log.LogInformation("TAPI client configuration {Configuration}", It.IsAny<object[]>()),
				Times.Once);
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldThrowWhenTheAddPathArgsAreInvalid()
		{
			this.CreateTapiBridge(WellKnownTransferClient.Unassigned);
			Assert.Throws<ArgumentNullException>(() => this.TapiBridgeInstance.AddPath(null));
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldNotModifyTheBridgeParameters()
		{
			DownloadTapiBridgeParameters2 expectedParameters = this.CreateDownloadTapiBridgeParameters(WellKnownTransferClient.FileShare);
			DownloadTapiBridgeParameters2 parameters = this.CreateDownloadTapiBridgeParameters(WellKnownTransferClient.FileShare);
			this.CreateTapiBridge(WellKnownTransferClient.FileShare, parameters);
			this.TapiBridgeInstance.AddPath(TestTransferPath);

			Assert.That(this.TapiBridgeInstance.Parameters.Application, Is.EqualTo(expectedParameters.Application));
			Assert.That(this.TapiBridgeInstance.Parameters.AsperaBcpRootFolder, Is.EqualTo(expectedParameters.AsperaBcpRootFolder));
			Assert.That(this.TapiBridgeInstance.Parameters.AsperaDatagramSize, Is.EqualTo(expectedParameters.AsperaDatagramSize));
			Assert.That(this.TapiBridgeInstance.Parameters.AsperaDocRootLevels, Is.EqualTo(expectedParameters.AsperaDocRootLevels));
			Assert.That(this.TapiBridgeInstance.Parameters.BadPathErrorsRetry, Is.EqualTo(expectedParameters.BadPathErrorsRetry));
			Assert.That(this.TapiBridgeInstance.Parameters.FileNotFoundErrorsDisabled, Is.EqualTo(expectedParameters.FileNotFoundErrorsDisabled));
			Assert.That(this.TapiBridgeInstance.Parameters.FileNotFoundErrorsRetry, Is.EqualTo(expectedParameters.FileNotFoundErrorsRetry));
			Assert.That(this.TapiBridgeInstance.Parameters.FileShare, Is.EqualTo(expectedParameters.FileShare));
			Assert.That(this.TapiBridgeInstance.Parameters.ForceAsperaClient, Is.EqualTo(expectedParameters.ForceAsperaClient));
			Assert.That(this.TapiBridgeInstance.Parameters.ForceClientCandidates, Is.EqualTo(expectedParameters.ForceClientCandidates));
			Assert.That(this.TapiBridgeInstance.Parameters.ForceFileShareClient, Is.EqualTo(expectedParameters.ForceFileShareClient));
			Assert.That(this.TapiBridgeInstance.Parameters.ForceHttpClient, Is.EqualTo(expectedParameters.ForceHttpClient));
			Assert.That(this.TapiBridgeInstance.Parameters.LargeFileProgressEnabled, Is.EqualTo(expectedParameters.LargeFileProgressEnabled));
			Assert.That(this.TapiBridgeInstance.Parameters.LogConfigFile, Is.EqualTo(expectedParameters.LogConfigFile));
			Assert.That(this.TapiBridgeInstance.Parameters.MaxInactivitySeconds, Is.EqualTo(expectedParameters.MaxInactivitySeconds));
			Assert.That(this.TapiBridgeInstance.Parameters.MaxJobParallelism, Is.EqualTo(expectedParameters.MaxJobParallelism));
			Assert.That(this.TapiBridgeInstance.Parameters.MinDataRateMbps, Is.EqualTo(expectedParameters.MinDataRateMbps));
			Assert.That(this.TapiBridgeInstance.Parameters.PermissionErrorsRetry, Is.EqualTo(expectedParameters.PermissionErrorsRetry));
			Assert.That(this.TapiBridgeInstance.Parameters.PreserveFileTimestamps, Is.EqualTo(expectedParameters.PreserveFileTimestamps));
			Assert.That(this.TapiBridgeInstance.Parameters.SubmitApmMetrics, Is.EqualTo(expectedParameters.SubmitApmMetrics));
			Assert.That(this.TapiBridgeInstance.Parameters.SupportCheckPath, Is.EqualTo(expectedParameters.SupportCheckPath));
			Assert.That(this.TapiBridgeInstance.Parameters.TargetDataRateMbps, Is.EqualTo(expectedParameters.TargetDataRateMbps));
			Assert.That(this.TapiBridgeInstance.Parameters.TargetPath, Is.EqualTo(expectedParameters.TargetPath));
			Assert.That(this.TapiBridgeInstance.Parameters.TimeoutSeconds, Is.EqualTo(expectedParameters.TimeoutSeconds));
			Assert.That(this.TapiBridgeInstance.Parameters.TransferCredential, Is.EqualTo(expectedParameters.TransferCredential));
			Assert.That(this.TapiBridgeInstance.Parameters.TransferLogDirectory, Is.EqualTo(expectedParameters.TransferLogDirectory));
			Assert.That(this.TapiBridgeInstance.Parameters.WaitTimeBetweenRetryAttempts, Is.EqualTo(expectedParameters.WaitTimeBetweenRetryAttempts));
			Assert.That(this.TapiBridgeInstance.Parameters.WebServiceUrl, Is.EqualTo(expectedParameters.WebServiceUrl));
			Assert.That(this.TapiBridgeInstance.Parameters.WorkspaceId, Is.EqualTo(expectedParameters.WorkspaceId));
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldGenerateNewClientRequestId()
		{
			DownloadTapiBridgeParameters2 expectedParameters = this.CreateDownloadTapiBridgeParameters(WellKnownTransferClient.FileShare);
			DownloadTapiBridgeParameters2 parameters = this.CreateDownloadTapiBridgeParameters(WellKnownTransferClient.FileShare);
			this.CreateTapiBridge(WellKnownTransferClient.FileShare, parameters);
			this.TapiBridgeInstance.AddPath(TestTransferPath);

			Assert.That(this.TapiBridgeInstance.Parameters.ClientRequestId, Is.Not.EqualTo(expectedParameters.ClientRequestId));
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldSetHttpErrorNumberOfRetriesTo1()
		{
			DownloadTapiBridgeParameters2 expectedParameters = this.CreateDownloadTapiBridgeParameters(WellKnownTransferClient.FileShare);
			DownloadTapiBridgeParameters2 parameters = this.CreateDownloadTapiBridgeParameters(WellKnownTransferClient.FileShare);
			this.CreateTapiBridge(WellKnownTransferClient.FileShare, parameters);
			this.TapiBridgeInstance.AddPath(TestTransferPath);

			Assert.That(this.TapiBridgeInstance.Parameters.HttpErrorNumberOfRetries, Is.Not.EqualTo(expectedParameters.HttpErrorNumberOfRetries));
			Assert.That(this.TapiBridgeInstance.Parameters.HttpErrorNumberOfRetries, Is.EqualTo(1));
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldSetWebCookieContainer()
		{
			DownloadTapiBridgeParameters2 parameters = this.CreateDownloadTapiBridgeParameters(WellKnownTransferClient.FileShare);

			this.CreateTapiBridge(WellKnownTransferClient.FileShare, parameters);
			this.TapiBridgeInstance.AddPath(TestTransferPath);

			Assert.That(this.TapiBridgeInstance.Parameters.WebCookieContainer, Is.Not.EqualTo(null));
		}

		protected override void CreateTapiBridge(WellKnownTransferClient client)
		{
			this.CreateTapiBridge(client, this.CreateDownloadTapiBridgeParameters(client));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "The test teardown disposes the test object.")]
		protected override void CreateTapiBridge(WellKnownTransferClient client, TapiBridgeParameters2 parameters)
		{
			DownloadTapiBridgeParameters2 downloadParameters = new DownloadTapiBridgeParameters2(parameters);

			this.TestClientConfiguration.Client = client;
			this.TapiBridgeInstance = new DownloadTapiBridge2(
				this.MockTapiObjectService.Object,
				downloadParameters,
				this.TestTransferContext,
				this.MockLogger.Object,
				this.CancellationTokenSource.Token);
			this.OnTapiBridgeCreated();
		}
	}
}