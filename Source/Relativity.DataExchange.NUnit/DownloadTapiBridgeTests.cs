﻿// -----------------------------------------------------------------------------------------------------
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
	using System.Threading;

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
							this.MockTransferLogger.Object,
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
							this.MockTransferLogger.Object,
							this.CancellationTokenSource.Token))
						{
						}
					});

			Assert.Throws<ArgumentNullException>(
				() =>
					{
						using (new DownloadTapiBridge2(
							this.MockTapiObjectService.Object,
							parameters,
							null,
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
							this.MockTransferLogger.Object,
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
							this.MockTransferLogger.Object,
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
			this.MockTransferLogger.Verify(
				log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Exactly(22));
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldThrowWhenTheAddPathArgsAreInvalid()
		{
			this.CreateTapiBridge(WellKnownTransferClient.Unassigned);
			Assert.Throws<ArgumentNullException>(() => this.TapiBridgeInstance.AddPath(null));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "The test teardown disposes the test object.")]
		protected override void CreateTapiBridge(WellKnownTransferClient client)
		{
			TapiBridgeParameters2 parameters = this.CreateTapiBridgeParameters(client);
			this.TestClientConfiguration.Client = client;
			this.TapiBridgeInstance = new DownloadTapiBridge2(
				this.MockTapiObjectService.Object,
				parameters,
				this.TestTransferContext,
				this.MockTransferLogger.Object,
				this.CancellationTokenSource.Token);
			this.OnTapiBridgeCreated();
		}
	}
}