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

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
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
			TapiBridgeParameters2 parameters = this.CreateTapiBridgeParameters(WellKnownTransferClient.Unassigned);
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
		public void ShouldThrowWhenTheAddPathArgsAreInvalid()
		{
			this.CreateTapiBridge(WellKnownTransferClient.Unassigned);
			Assert.Throws<ArgumentNullException>(() => this.TapiBridgeInstance.AddPath(null));
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldModifyTheBridgeParameters()
		{
			TapiBridgeParameters2 parameters = this.CreateTapiBridgeParameters(WellKnownTransferClient.FileShare);
			parameters.FileNotFoundErrorsDisabled = false;
			parameters.FileNotFoundErrorsRetry = true;
			parameters.PermissionErrorsRetry = true;
			this.CreateTapiBridge(WellKnownTransferClient.FileShare, parameters);
			Assert.That(this.TapiBridgeInstance.Parameters.FileNotFoundErrorsDisabled, Is.False);
			Assert.That(this.TapiBridgeInstance.Parameters.FileNotFoundErrorsRetry, Is.True);
			Assert.That(this.TapiBridgeInstance.Parameters.PermissionErrorsRetry, Is.True);
			this.TapiBridgeInstance.AddPath(TestTransferPath);

			// All expected values were inverted above to ensure that all expected behavior is honored.
			Assert.That(this.TapiBridgeInstance.Parameters.FileNotFoundErrorsDisabled, Is.True);
			Assert.That(this.TapiBridgeInstance.Parameters.FileNotFoundErrorsRetry, Is.False);
			Assert.That(this.TapiBridgeInstance.Parameters.PermissionErrorsRetry, Is.False);
		}

		protected override void CreateTapiBridge(WellKnownTransferClient client)
		{
			this.CreateTapiBridge(client, this.CreateTapiBridgeParameters(client));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "The test teardown disposes the test object.")]
		private void CreateTapiBridge(WellKnownTransferClient client, TapiBridgeParameters2 parameters)
		{
			this.TestClientConfiguration.Client = client;
			this.TapiBridgeInstance = new DownloadTapiBridge2(
				this.MockTapiObjectService.Object,
				parameters,
				this.TestTransferContext,
				this.MockLogger.Object,
				this.CancellationTokenSource.Token);
			this.OnTapiBridgeCreated();
		}
	}
}