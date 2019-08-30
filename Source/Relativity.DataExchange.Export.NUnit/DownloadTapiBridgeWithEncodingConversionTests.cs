﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridgeWithEncodingConversionTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Linq;
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;
	using Relativity.Transfer;

	[TestFixture]
	public class DownloadTapiBridgeWithEncodingConversionTests : DownloadTapiBridgeAdapterTests
	{
		private Mock<ILog> _logger;

		[SetUp]
		public void SetUp()
		{
			this.SetUpMocks();

			this._logger = new Mock<ILog>();

			this.Instance = new DownloadTapiBridgeWithEncodingConversion(
				this.TapiBridge.Object,
				this.ProgressHandler.Object,
				this.MessagesHandler.Object,
				this.TransferStatistics.Object,
				this._logger.Object);
		}

		[Test]
		public void ItShouldAddRequestToTransferBridgeQueue()
		{
			// ACT
			this.Instance.QueueDownload(new TransferPath());
			this.Instance.QueueDownload(new TransferPath());
			this.Instance.QueueDownload(new TransferPath());

			// ASSERT
			this.TapiBridge.Verify(item => item.AddPath(It.IsAny<TransferPath>()), Times.Exactly(3));
		}

		[Test]
		public void ItShouldNotifySubscribersOnSuccessfulTransfer()
		{
			// ARRANGE
			const string firstFile = "firstFile";
			const string secondFile = "secondFile";

			global::System.Collections.Generic.List<string> subscriber = new global::System.Collections.Generic.List<string>();
			this.Instance.FileDownloaded.Subscribe(subscriber.Add);

			// ACT
			this.TapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(firstFile, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			// subscriber should not be notified on failed transfer
			this.TapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(secondFile, true, false, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			// ASSERT

			// that is needed as the events are send as
			Thread.Sleep(20);

			Assert.That(subscriber.Count, Is.EqualTo(1));
			Assert.That(subscriber.Contains(firstFile));
			Assert.That(!subscriber.Contains(secondFile));
		}

		[Test]
		public void ItShouldAlwaysStopConverterAfterDownloadFinished()
		{
			// ARRANGE
			bool currentStatus = true;

			this.Instance.FileDownloadCompleted.Subscribe(status => currentStatus = status);

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
			Assert.That(currentStatus, Is.False);
		}

		[Test]
		public void ItShouldNotWaitForTapiTransfer()
		{
			// ARRANGE
			bool currentStatus = true;

			this.Instance.FileDownloadCompleted.Subscribe(status => currentStatus = status);

			// ACT
			this.Instance.WaitForTransfers();

			// ASSERT
			Assert.That(currentStatus);
			this.TapiBridge.Verify(item => item.WaitForTransfers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
		}
	}
}