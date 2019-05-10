// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridgeForFilesTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
	using global::NUnit.Framework;

    using Moq;

	using Relativity.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Export.VolumeManagerV2.Statistics;
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

			Instance = new DownloadTapiBridgeForFiles(
				TapiBridge.Object,
				ProgressHandler.Object,
				MessagesHandler.Object,
				TransferStatistics.Object,
				_transferClientHandler.Object,
				new NullLogger());
		}

		[Test]
		public void ItShouldDisposeTransferClientHandler()
		{
			Instance.Dispose();

			// ASSERT
			_transferClientHandler.Verify(x => x.Detach());
		}
	}
}