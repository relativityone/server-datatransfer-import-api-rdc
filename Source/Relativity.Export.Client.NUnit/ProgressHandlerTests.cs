﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="ProgressHandlerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System;

    using global::NUnit.Framework;

	using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;

    using Moq;

    using Relativity.Import.Export.Transfer;
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
			_downloadProgressManager = new Mock<IDownloadProgressManager>();
			_tapiBridge = new Mock<ITapiBridge>();

			_instance = CreateInstance(_downloadProgressManager.Object);
		}

		[Test]
		public void ItShouldMarkFileAsDownloadedWhenFileIsDownloaded()
		{
			const string id = "812216";

			// ACT
			_instance.Attach(_tapiBridge.Object);
			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id, true, TransferPathStatus.Successful, 1, 1, DateTime.Now, DateTime.Now));

			// ASSERT
			VerifyFileMarkedAsDownloaded(_downloadProgressManager, id, 1);
		}

		[Test]
		public void ItShouldNotMarkFileAsDownloadedWhenFileIsNotDownloaded()
		{
			const string id = "812216";

			// ACT
			_instance.Attach(_tapiBridge.Object);
			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id, false, TransferPathStatus.Failed, 1, 1, DateTime.Now, DateTime.Now));

			// ASSERT
			VerifyFileNotMarkedAsDownloaded(_downloadProgressManager, id, 1);
		}

		[Test]
		public void ItShouldDetachFromTapiBridge()
		{
			const string id1 = "812216";
			const string id2 = "267641";

			// ACT
			_instance.Attach(_tapiBridge.Object);
			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id1, true, TransferPathStatus.Successful, 1, 1, DateTime.Now, DateTime.Now));

			_instance.Detach();
			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(id2, true, TransferPathStatus.Successful, 1, 1, DateTime.Now, DateTime.Now));

			// ASSERT
			VerifyFileMarkedAsDownloaded(_downloadProgressManager, id1, 1);
			VerifyFileNotMarkedAsDownloaded(_downloadProgressManager, id2, 2);
		}

		protected abstract ProgressHandler CreateInstance(IDownloadProgressManager downloadProgressManager);

		protected abstract void VerifyFileMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id, int lineNumber);

		protected abstract void VerifyFileNotMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id, int lineNumber);
	}
}