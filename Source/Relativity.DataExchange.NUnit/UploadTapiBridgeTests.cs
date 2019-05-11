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
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	/// <summary>
	/// Represents <see cref="UploadTapiBridge2"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class UploadTapiBridgeTests : TapiBridgeTestsBase<UploadTapiBridge2>
	{
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
							this.MockTransferLogger.Object,
							CancellationToken.None))
						{
						}
					});

			Assert.Throws<ArgumentNullException>(
				() =>
					{
						using (new UploadTapiBridge2(
							this.MockTapiObjectService.Object,
							null,
							this.MockTransferLogger.Object,
							CancellationToken.None))
						{
						}
					});

			Assert.Throws<ArgumentNullException>(
				() =>
					{
						using (new UploadTapiBridge2(
							this.MockTapiObjectService.Object,
							parameters,
							null,
							CancellationToken.None))
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
							this.MockTransferLogger.Object,
							CancellationToken.None))
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
							this.MockTransferLogger.Object,
							CancellationToken.None))
						{
						}
					});
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldDumpTheTapiBridgeParameters()
		{
			this.CreateTapiBridge(WellKnownTransferClient.Unassigned);
			this.TapiBridgeInstance.DumpInfo();

			// Force somebody to review this test should the number of logged entries change.
			this.MockTransferLogger.Verify(
				log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Exactly(25));
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
				this.MockTransferLogger.Object,
				CancellationToken.None);
		}

		private UploadTapiBridgeParameters2 CreateUploadTapiBridgeParameters(WellKnownTransferClient client)
		{
			UploadTapiBridgeParameters2 parameters = new UploadTapiBridgeParameters2
			{
				Credentials = new NetworkCredential(),
				WebServiceUrl = "https://relativity.one.com",
				WorkspaceId = this.TestWorkspaceId,
			};
			this.ConfigureTapiBridgeParameters(parameters, client);
			return parameters;
		}
	}
}