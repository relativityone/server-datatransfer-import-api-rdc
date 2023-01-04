// -----------------------------------------------------------------------------------------------------
// <copyright file="UploadTapiBridgeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="UploadTapiBridge2"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Net;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	/// <summary>
	/// Represents <see cref="UploadTapiBridge2"/> tests.
	/// </summary>
	[TestFixture(false)]
	[TestFixture(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class UploadTapiBridgeTests : TapiBridgeTestsBase<UploadTapiBridge2>
	{
		public UploadTapiBridgeTests(bool useLegacyWebApi)
			: base(useLegacyWebApi)
		{
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldThrowWhenTheConstructorArgsAreInvalid()
		{
			UploadTapiBridgeParameters2 parameters =
				this.CreateUploadTapiBridgeParameters(WellKnownTransferClient.Unassigned);
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						using (new UploadTapiBridge2(
							null,
							parameters,
							this.MockLogger.Object,
							this.UseLegacyWebApi,
							this.CancellationTokenSource.Token))
						{
						}
					});

			Assert.Throws<ArgumentNullException>(
				() =>
					{
						using (new UploadTapiBridge2(
							this.MockTapiObjectService.Object,
							null,
							this.MockLogger.Object,
							this.UseLegacyWebApi,
							this.CancellationTokenSource.Token))
						{
						}
					});

			parameters.WorkspaceId = 0;
			Assert.Throws<ArgumentOutOfRangeException>(
				() =>
					{
						using (new UploadTapiBridge2(
							this.MockTapiObjectService.Object,
							parameters,
							this.MockLogger.Object,
							this.UseLegacyWebApi,
							this.CancellationTokenSource.Token))
						{
						}
					});

			parameters.Credentials = null;
			Assert.Throws<ArgumentException>(
				() =>
					{
						using (new UploadTapiBridge2(
							this.MockTapiObjectService.Object,
							parameters,
							this.MockLogger.Object,
							this.UseLegacyWebApi,
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
				Times.Exactly(27));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "The test teardown disposes the test object.")]
		protected override void CreateTapiBridge(WellKnownTransferClient client)
		{
			UploadTapiBridgeParameters2 parameters = this.CreateUploadTapiBridgeParameters(client);
			this.TapiBridgeInstance = new UploadTapiBridge2(
				this.MockTapiObjectService.Object,
				parameters,
				this.TestTransferContext,
				this.MockLogger.Object,
				this.UseLegacyWebApi,
				this.CancellationTokenSource.Token);
			this.OnTapiBridgeCreated();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "The test teardown disposes the test object.")]
		protected override void CreateTapiBridge(WellKnownTransferClient client, TapiBridgeParameters2 parameters)
		{
			UploadTapiBridgeParameters2 uploadParameters = new UploadTapiBridgeParameters2(parameters);
			this.TapiBridgeInstance = new UploadTapiBridge2(
				this.MockTapiObjectService.Object,
				uploadParameters,
				this.TestTransferContext,
				this.MockLogger.Object,
				this.UseLegacyWebApi,
				this.CancellationTokenSource.Token);
			this.OnTapiBridgeCreated();
		}

		private UploadTapiBridgeParameters2 CreateUploadTapiBridgeParameters(WellKnownTransferClient client)
		{
			UploadTapiBridgeParameters2 parameters = new UploadTapiBridgeParameters2
			{
				Credentials = new NetworkCredential(),
				WebServiceUrl = "https://relativity.one.com",
				WorkspaceId = this.TestWorkspaceId,
				MaxInactivitySeconds = this.TestMaxInactivitySeconds,
			};

			this.ConfigureTapiBridgeParameters(parameters, client);
			return parameters;
		}
	}
}