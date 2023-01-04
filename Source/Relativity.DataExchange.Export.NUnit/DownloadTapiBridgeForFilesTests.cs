// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridgeForFilesTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class DownloadTapiBridgeForFilesTests : DownloadTapiBridgeAdapterTests
	{
		private Mock<ITransferClientHandler> _transferClientHandler;

		[SetUp]
		public void SetUp()
		{
			this.SetUpMocks();

			this._transferClientHandler = new Mock<ITransferClientHandler>();

			this.Instance = new DownloadTapiBridgeForFiles(
				this.TapiBridge.Object,
				this.ProgressHandler.Object,
				this.MessagesHandler.Object,
				this.TransferStatistics.Object,
				this._transferClientHandler.Object,
				new TestNullLogger());
		}

		[Test]
		public void ItShouldDisposeTransferClientHandler()
		{
			this.Instance.Dispose();

			// ASSERT
			this._transferClientHandler.Verify(x => x.Unsubscribe(this.TapiBridge.Object));
		}
	}
}