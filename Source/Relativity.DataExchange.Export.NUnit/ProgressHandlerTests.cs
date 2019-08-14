// -----------------------------------------------------------------------------------------------------
// <copyright file="ProgressHandlerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	[TestFixture]
	public abstract class ProgressHandlerTests
	{
		private ProgressHandler _instance;

		private Mock<IDownloadProgressManager> _downloadProgressManager;
		private Mock<ITapiBridge> _tapiBridge;

		[SetUp]
		public void SetUp()
		{
			this._downloadProgressManager = new Mock<IDownloadProgressManager>();
			this._tapiBridge = new Mock<ITapiBridge>();

			this._instance = this.CreateInstance(this._downloadProgressManager.Object);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldMarkFileAsCompleted(bool transferStatus)
		{
			const string id = "812216";
			const bool TransferCompleted = true;

			// ACT
			this._instance.Attach(this._tapiBridge.Object);
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id, TransferCompleted, transferStatus, 1, 1, DateTime.Now, DateTime.Now));

			// ASSERT
			this.VerifyFileMarkedAsDownloaded(this._downloadProgressManager, id, 1);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldNotMarkFileAsCompletedWhenWhenTransferNotFinished(bool transferStatus)
		{
			const string id = "812216";
			const bool TransferNotCompleted = false;

			// ACT
			this._instance.Attach(this._tapiBridge.Object);
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id, TransferNotCompleted, transferStatus, 1, 1, DateTime.Now, DateTime.Now));

			// ASSERT
			this.VerifyFileNotMarkedAsDownloaded(this._downloadProgressManager, id, 1);
		}

		[Test]
		public void ItShouldDetachFromTapiBridge()
		{
			const string id1 = "812216";
			const string id2 = "267641";

			// ACT
			this._instance.Attach(this._tapiBridge.Object);
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id1, true, true, 1, 1, DateTime.Now, DateTime.Now));

			this._instance.Detach(this._tapiBridge.Object);
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id2, true, true, 1, 1, DateTime.Now, DateTime.Now));

			// ASSERT
			this.VerifyFileMarkedAsDownloaded(this._downloadProgressManager, id1, 1);
			this.VerifyFileNotMarkedAsDownloaded(this._downloadProgressManager, id2, 2);
		}

		protected abstract ProgressHandler CreateInstance(IDownloadProgressManager downloadProgressManager);

		protected abstract void VerifyFileMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id, int lineNumber);

		protected abstract void VerifyFileNotMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id, int lineNumber);
	}
}