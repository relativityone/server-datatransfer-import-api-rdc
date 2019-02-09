// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridgeForFilesTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;

    using Moq;

    using global::NUnit.Framework;

    using Relativity.Logging;

    [TestFixture]
	public class DownloadTapiBridgeForFilesTests : DownloadTapiBridgeAdapterTests
	{
		private Mock<ITransferClientHandler> _transferClientHandler;

		[SetUp]
		public void SetUp()
		{
			SetUpMocks();

			_transferClientHandler = new Mock<ITransferClientHandler>();

			Instance = new DownloadTapiBridgeForFiles(TapiBridge.Object, ProgressHandler.Object, MessagesHandler.Object, TransferStatistics.Object, _transferClientHandler.Object,
				new NullLogger());
		}

		[Test]
		public void ItShouldDisposeTransferClientHandler()
		{
			Instance.Dispose();

			//ASSERT
			_transferClientHandler.Verify(x => x.Detach());
		}
	}
}