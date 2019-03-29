// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridgeWithEncodingConversionTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System;

    using global::NUnit.Framework;

    using Moq;

	using Relativity.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;
    using Relativity.Transfer;

    [TestFixture]
	public class DownloadTapiBridgeWithEncodingConversionTests : DownloadTapiBridgeAdapterTests
	{
		private Mock<ILongTextEncodingConverter> _longTextEncodingConverter;

		[SetUp]
		public void SetUp()
		{
			SetUpMocks();

			_longTextEncodingConverter = new Mock<ILongTextEncodingConverter>();

			Instance = new DownloadTapiBridgeWithEncodingConversion(
				TapiBridge.Object,
				ProgressHandler.Object,
				MessagesHandler.Object,
				TransferStatistics.Object,
				_longTextEncodingConverter.Object,
				new NullLogger());
		}

		[Test]
		public void ItShouldStartConverterAfterAddingFirstTransferPath()
		{
			// ACT
			Instance.QueueDownload(new TransferPath());
			Instance.QueueDownload(new TransferPath());
			Instance.QueueDownload(new TransferPath());

			// ASSERT
			_longTextEncodingConverter.Verify(x => x.StartListening(TapiBridge.Object), Times.Once);
		}

		[Test]
		public void ItShouldAlwaysStopConverterAfterDownloadFinished()
		{
			TapiBridge.Setup(x => x.WaitForTransferJob()).Throws<Exception>();

			// ACT
			Instance.QueueDownload(new TransferPath());
			Assert.Throws<Exception>(() => Instance.WaitForTransferJob());

			// ASSERT
			_longTextEncodingConverter.Verify(x => x.StopListening(TapiBridge.Object), Times.Once);
		}

		[Test]
		public void ItShouldWaitForConversionToComplete()
		{
			// ACT
			Instance.QueueDownload(new TransferPath());
			Instance.WaitForTransferJob();

			// ASSERT
			_longTextEncodingConverter.Verify(x => x.WaitForConversionCompletion(), Times.Once);
		}
	}
}