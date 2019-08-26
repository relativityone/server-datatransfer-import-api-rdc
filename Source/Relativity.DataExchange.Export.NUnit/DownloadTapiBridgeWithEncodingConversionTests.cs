// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridgeWithEncodingConversionTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;
	using Relativity.Transfer;

	[TestFixture]
	public class DownloadTapiBridgeWithEncodingConversionTests : DownloadTapiBridgeAdapterTests
	{
		private Mock<ILongTextEncodingConverter> _longTextEncodingConverter;

		[SetUp]
		public void SetUp()
		{
			this.SetUpMocks();

			this._longTextEncodingConverter = new Mock<ILongTextEncodingConverter>();

			this.Instance = new DownloadTapiBridgeWithEncodingConversion(
				this.TapiBridge.Object,
				this.ProgressHandler.Object,
				this.MessagesHandler.Object,
				this.TransferStatistics.Object,
				this._longTextEncodingConverter.Object,
				new NullLogger());
		}

		[Test]
		public void ItShouldStartConverterAfterAddingFirstTransferPath()
		{
			// ACT
			this.Instance.QueueDownload(new TransferPath());
			this.Instance.QueueDownload(new TransferPath());
			this.Instance.QueueDownload(new TransferPath());

			// ASSERT
			this._longTextEncodingConverter.Verify(x => x.Subscribe(this.TapiBridge.Object), Times.Once);
		}

		[Test]
		public void ItShouldAlwaysStopConverterAfterDownloadFinished()
		{
			this.TapiBridge.Setup(x => x.WaitForTransfers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws<Exception>();

			// ACT
			this.Instance.QueueDownload(new TransferPath());
			Assert.Throws<Exception>(() => this.Instance.WaitForTransfers());

			// ASSERT
			this._longTextEncodingConverter.Verify(x => x.Unsubscribe(this.TapiBridge.Object), Times.Once);
		}

		[Test]
		public void ItShouldWaitForConversionToComplete()
		{
			// ACT
			this.Instance.QueueDownload(new TransferPath());
			this.Instance.WaitForTransfers();

			// ASSERT
			this._longTextEncodingConverter.Verify(x => x.WaitForConversionCompletion(), Times.Once);
		}
	}
}