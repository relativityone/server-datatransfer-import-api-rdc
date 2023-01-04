// -----------------------------------------------------------------------------------------------------
// <copyright file="ProgressHandlerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.IO;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Transfer;

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
		public void ItShouldMarkFileAsCompleted(bool transferResult)
		{
			const string id = "812216";
			const bool TransferCompleted = true;

			// ACT
			this._instance.Subscribe(this._tapiBridge.Object);
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id, Path.Combine(@"C:\temp", id), TransferCompleted, transferResult, 1, 1, DateTime.Now, DateTime.Now));

			// ASSERT
			this.VerifyFileMarkedAsCompleted(this._downloadProgressManager, Path.Combine(@"C:\temp", id), 1, transferResult);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldNotMarkFileAsCompletedWhenWhenTransferNotFinished(bool transferResult)
		{
			const string id = "812216";
			const bool TransferNotCompleted = false;

			// ACT
			this._instance.Subscribe(this._tapiBridge.Object);
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id, string.Empty, TransferNotCompleted, transferResult, 1, 1, DateTime.Now, DateTime.Now));

			// ASSERT
			this.VerifyFileNotMarkedAsCompleted(this._downloadProgressManager, id, 1, transferResult);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldDetachFromTapiBridge(bool transferResult)
		{
			const string id1 = "812216";
			const string id2 = "267641";

			// ACT
			this._instance.Subscribe(this._tapiBridge.Object);
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id1, Path.Combine(@"C:\temp", id1), true, transferResult, 1, 1, DateTime.Now, DateTime.Now));

			this._instance.Unsubscribe(this._tapiBridge.Object);
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id2, Path.Combine(@"C:\temp", id2), true, transferResult, 1, 1, DateTime.Now, DateTime.Now));

			// ASSERT
			this.VerifyFileMarkedAsCompleted(this._downloadProgressManager, Path.Combine(@"C:\temp", id1), 1, transferResult);
			this.VerifyFileNotMarkedAsCompleted(this._downloadProgressManager, Path.Combine(@"C:\temp", id2), 2, transferResult);
		}

		protected abstract ProgressHandler CreateInstance(IDownloadProgressManager downloadProgressManager);

		protected abstract void VerifyFileMarkedAsCompleted(Mock<IDownloadProgressManager> downloadProgressManager, string targetFile, int lineNumber, bool transferResult);

		protected abstract void VerifyFileNotMarkedAsCompleted(Mock<IDownloadProgressManager> downloadProgressManager, string targetFile, int lineNumber, bool transferResult);
	}
}