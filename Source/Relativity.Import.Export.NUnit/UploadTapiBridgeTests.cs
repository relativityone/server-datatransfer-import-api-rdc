// -----------------------------------------------------------------------------------------------------
// <copyright file="UploadTapiBridgeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="UploadTapiBridge"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
    using System;
    using System.Net;
    using System.Threading;

    using global::NUnit.Framework;

    using Moq;

    using Relativity.Import.Export.TestFramework;
	using Relativity.Import.Export.Transfer;
    using Relativity.Transfer;

    /// <summary>
	/// Represents <see cref="UploadTapiBridge"/> tests.
	/// </summary>
	[TestFixture]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "The test class handles the disposal.")]
    public class UploadTapiBridgeTests : TapiBridgeTestsBase<UploadTapiBridge>
	{
		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldThrowWhenTheConstructorArgsAreInvalid()
		{
			UploadTapiBridgeParameters parameters =
				this.CreateUploadTapiBridgeParameters(WellKnownTransferClient.Unassigned);
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						using (new UploadTapiBridge(
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
						using (new UploadTapiBridge(
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
						using (new UploadTapiBridge(
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
						using (new UploadTapiBridge(
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
						using (new UploadTapiBridge(
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
				Times.Exactly(23));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "The test teardown disposes the test object.")]
		protected override void CreateTapiBridge(WellKnownTransferClient client)
		{
			UploadTapiBridgeParameters parameters = this.CreateUploadTapiBridgeParameters(client);
			this.TapiBridgeInstance = new UploadTapiBridge(
				this.MockTapiObjectService.Object,
				parameters,
				this.MockTransferLogger.Object,
				CancellationToken.None);
		}

		private UploadTapiBridgeParameters CreateUploadTapiBridgeParameters(WellKnownTransferClient client)
		{
			UploadTapiBridgeParameters parameters = new UploadTapiBridgeParameters
				                                        {
					                                        Credentials = new NetworkCredential(),
					                                        WebServiceUrl = "https://relativity.one.com",
					                                        WorkspaceId = this.TestWorkspaceId
				                                        };
			this.ConfigureTapiBridgeParameters(parameters, client);
			return parameters;
		}
	}
}