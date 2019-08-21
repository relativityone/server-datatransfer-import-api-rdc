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
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;
	using Relativity.Transfer;

	[TestFixture]
	public class DownloadTapiBridgeWithEncodingConversionTests : DownloadTapiBridgeAdapterTests
	{
		private Mock<ILongTextEncodingConverter> _longTextEncodingConverter;
		private Mock<ILog> _logger;

		[SetUp]
		public void SetUp()
		{
			this.SetUpMocks();

			this._logger = new Mock<ILog>();
			this._longTextEncodingConverter = new Mock<ILongTextEncodingConverter>();

			this.Instance = new DownloadTapiBridgeWithEncodingConversion(
				this.TapiBridge.Object,
				this.ProgressHandler.Object,
				this.MessagesHandler.Object,
				this.TransferStatistics.Object,
				this._longTextEncodingConverter.Object,
				this._logger.Object);
		}

		[Test]
		public void ItShouldStartConverterAfterAddingFirstTransferPath()
		{
			// ACT
			this.Instance.QueueDownload(new TransferPath());
			this.Instance.QueueDownload(new TransferPath());
			this.Instance.QueueDownload(new TransferPath());

			// ASSERT
			this._longTextEncodingConverter.Verify(x => x.NotifyStartConversion(), Times.Once);
		}

		[Test]
		public void ItShouldAlwaysStopConverterAfterDownloadFinished()
		{
			this.TapiBridge
				.Setup(
					x => x.WaitForTransfers(
						It.IsAny<string>(),
						It.IsAny<string>(),
						It.IsAny<string>(),
						It.IsAny<bool>())).Throws<Exception>();

			// ACT
			this.Instance.QueueDownload(new TransferPath());
			Assert.Throws<Exception>(() => this.Instance.WaitForTransfers());

			// ASSERT
			this._longTextEncodingConverter.Verify(x => x.NotifyStopConversion(), Times.Once);
		}

		[Test]
		public void ItShouldWaitForConversionToComplete()
		{
			// ARRANGE
			const string firstFile = "firstFile";
			const string secondFile = "secondFile";

			// ACT
			this.Instance.QueueDownload(new TransferPath());

			this.TapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(firstFile, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this.TapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(secondFile, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this.Instance.WaitForTransfers();

			// ASSERT
			this._longTextEncodingConverter.Verify(x => x.AddForConversion(firstFile), Times.Once);
			this._longTextEncodingConverter.Verify(x => x.AddForConversion(secondFile), Times.Once);

			this._longTextEncodingConverter.Verify(x => x.WaitForConversionCompletion(), Times.Once);
		}

		[Test]
		public void ItShouldLogErrorOnConverterQueueMarkAsCompleted()
		{
			// ARRANGE
			this._longTextEncodingConverter.Setup(x => x.AddForConversion(It.IsAny<string>()))
				.Throws<InvalidOperationException>();

			// ACT
			this.Instance.QueueDownload(new TransferPath());

			Assert.Throws<InvalidOperationException>(
				() => this.TapiBridge.Raise(
					x => x.TapiProgress += null,
					new TapiProgressEventArgs("fileName", true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue)));

			// ASSERT
			this._logger.Verify(x => x.LogError(It.IsAny<InvalidOperationException>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
		}

		[Test]
		public void ItShouldNotConvertFileWhenDownloadFailed()
		{
			// Simulate an inability to find the LongText object within the repository.
			const string fileName = "fileName";

			// ACT
			this.Instance.QueueDownload(new TransferPath());

			// Mark download as not completed
			this.TapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(fileName, true, false, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			// ASSERT
			_longTextEncodingConverter.Verify(x => x.AddForConversion(fileName), Times.Never);
		}
	}
}