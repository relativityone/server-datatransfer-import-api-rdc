﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextDownloaderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;
	using Relativity.Transfer;

	[TestFixture]
	public class LongTextDownloaderTests
	{
		private LongTextDownloader _instance;

		private Mock<ILongTextTapiBridgePool> _longTextTapiBridgePool;
		private Mock<IDownloadTapiBridge> _bridge;

		[SetUp]
		public void SetUp()
		{
			this._longTextTapiBridgePool = new Mock<ILongTextTapiBridgePool>();
			this._bridge = new Mock<IDownloadTapiBridge>();

			this._longTextTapiBridgePool.Setup(x => x.Request(CancellationToken.None)).Returns(this._bridge.Object);

			this._instance = new LongTextDownloader(new SafeIncrement(), this._longTextTapiBridgePool.Object, new NullLogger());
		}

		[Test]
		public async Task ItShouldRequestAndReleaseBridge()
		{
			// ACT
			await this._instance.DownloadAsync(new List<LongTextExportRequest>(), CancellationToken.None).ConfigureAwait(false);

			// ASSERT
			this._longTextTapiBridgePool.Verify(x => x.Request(CancellationToken.None), Times.Once);
			this._longTextTapiBridgePool.Verify(x => x.Release(this._bridge.Object), Times.Once);
		}

		[Test]
		public async Task ItShouldDownloadAllRequests()
		{
			ObjectExportInfo artifact = new ObjectExportInfo();
			List<LongTextExportRequest> longTextExportRequests = new List<LongTextExportRequest>
			{
				LongTextExportRequest.CreateRequestForFullText(artifact, 1, "a"),
				LongTextExportRequest.CreateRequestForFullText(artifact, 2, "b")
			};

			// ACT
			await this._instance.DownloadAsync(longTextExportRequests, CancellationToken.None).ConfigureAwait(false);

			// ASSERT
			this._bridge.Verify(x => x.WaitForTransferJob(), Times.Once);
			this._bridge.Verify(x => x.QueueDownload(It.Is<TransferPath>(t => t.Order == 1)));
			this._bridge.Verify(x => x.QueueDownload(It.Is<TransferPath>(t => t.Order == 2)));
		}

		[Test]
		public async Task ItShouldSetTextFileName()
		{
			const string expectedFileName = "file name";
			this._bridge.Setup(x => x.QueueDownload(It.IsAny<TransferPath>())).Returns(expectedFileName);

			ObjectExportInfo artifact = new ObjectExportInfo();
			List<LongTextExportRequest> longTextExportRequests = new List<LongTextExportRequest>
			{
				LongTextExportRequest.CreateRequestForFullText(artifact, 1, "a")
			};

			// ACT
			await this._instance.DownloadAsync(longTextExportRequests, CancellationToken.None).ConfigureAwait(false);

			// ASSERT
			Assert.That(longTextExportRequests[0].FileName, Is.EqualTo(expectedFileName));
		}
	}
}