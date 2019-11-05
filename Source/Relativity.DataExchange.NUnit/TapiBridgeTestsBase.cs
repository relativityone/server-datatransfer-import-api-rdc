// -----------------------------------------------------------------------------------------------------
// <copyright file="TapiBridgeTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a base class for <see cref="TapiBridgeBase2"/> derived classes.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
    using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	/// <summary>
	/// Represents a base class for <see cref="TapiBridgeBase2"/> derived classes.
	/// </summary>
	/// <typeparam name="TTapiBridge">
	/// The TAPI bridge type under test.
	/// </typeparam>
	public abstract class TapiBridgeTestsBase<TTapiBridge>
		where TTapiBridge : TapiBridgeBase2
	{
		/// <summary>
		/// The real object service backing.
		/// </summary>
		private readonly TapiObjectService realObjectService = new TapiObjectService();

		private readonly object syncRoot = new object();

		private readonly List<TapiProgressEventArgs> progressEvents = new List<TapiProgressEventArgs>();

		private readonly List<TapiLargeFileProgressEventArgs> largeFileProgressEvents = new List<TapiLargeFileProgressEventArgs>();

		private readonly List<JobTransferPath> queuedJobTransferPaths = new List<JobTransferPath>();

		protected static TransferPath TestTransferPath =>
			new TransferPath(
				"C:\\out.txt",
				TransferPathAttributes.File,
				@"\\files\File\EDDS1234",
				TransferDirection.Upload,
				"out.txt") { Bytes = RandomHelper.NextInt64(1000, 100000), Order = 1 };

		protected static string TestWaitMessage => "Waiting...";

		protected static string TestSuccessMessage => "Success...";

		protected static string TestErrorMessage => "Error...";

		protected Mock<ITapiObjectService> MockTapiObjectService { get; private set; }

		protected Mock<ITransferJob> MockTransferJob { get; private set; }

		protected Mock<ITransferJobService> MockTransferJobService { get; private set; }

		protected Mock<ITransferLog> MockTransferLogger { get; private set; }

		protected Mock<IRelativityTransferHost> MockRelativityTransferHost { get; private set; }

		protected Mock<ITransferClient> MockTransferClient { get; private set; }

		protected TTapiBridge TapiBridgeInstance { get; set; }

		protected ClientConfiguration TestClientConfiguration { get; private set; }

		protected TransferContext TestTransferContext { get; private set; }

		protected int TestWorkspaceId { get; private set; }

		protected int TestMaxInactivitySeconds { get; private set; }

		protected CancellationTokenSource CancellationTokenSource { get; private set; }

		protected TapiClient ChangedTapiClient { get; set; }

		protected IList<TapiLargeFileProgressEventArgs> LargeFileProgressEvents => this.largeFileProgressEvents;

		protected IList<TapiProgressEventArgs> ProgressEvents => this.progressEvents;

		[SetUp]
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "The test teardown disposes the test object.")]
		public void Setup()
		{
			this.CancellationTokenSource = new CancellationTokenSource();
			this.ChangedTapiClient = TapiClient.None;
			this.largeFileProgressEvents.Clear();
			this.MockTransferLogger = new Mock<ITransferLog>();
			this.progressEvents.Clear();
			this.TestClientConfiguration = new ClientConfiguration();
			this.TestTransferContext = new TransferContext();
			this.TestWorkspaceId = RandomHelper.NextInt32(1111111, 9999999);
			this.TestMaxInactivitySeconds = 10;
			this.MockTransferJobService = new Mock<ITransferJobService>();
			this.queuedJobTransferPaths.Clear();
			this.queuedJobTransferPaths.Add(new JobTransferPath { Path = TestTransferPath, Status = TransferPathStatus.Started });
			this.MockTransferJobService.Setup(x => x.GetJobTransferPaths()).Returns(this.queuedJobTransferPaths);
				this.MockTransferJobService.Setup(x => x.Issues).Returns(new List<ITransferIssue>());
			this.MockTransferJobService.Setup(x => x.GetRetryableRequestTransferPaths())
				.Returns(new List<TransferPath>());
			this.MockTransferJob = new Mock<ITransferJob>();
			this.MockTransferJob.Setup(x => x.Status).Returns(TransferJobStatus.Running);
			this.MockTransferJob.Setup(x => x.JobService).Returns(this.MockTransferJobService.Object);
			this.MockTransferClient = new Mock<ITransferClient>();
			this.MockTransferClient.SetupGet(x => x.Client).Returns(WellKnownTransferClient.FileShare);
			this.MockTransferClient.SetupGet(x => x.Configuration).Returns(this.TestClientConfiguration);
			this.MockTransferClient.SetupGet(x => x.Id).Returns(new Guid(TransferClientConstants.FileShareClientId));
			this.MockTransferClient.Setup(
					x => x.CreateJobAsync(It.IsAny<ITransferRequest>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(this.MockTransferJob.Object);
			this.MockRelativityTransferHost = new Mock<IRelativityTransferHost>();
			this.MockRelativityTransferHost.Setup(x => x.CreateClient(It.IsAny<ClientConfiguration>()))
				.Returns<ClientConfiguration>(
					configuration =>
						{
							// Take into account tests that switch to web mode.
							this.MockTransferClient.Setup(x => x.Id).Returns(configuration.ClientId);
							this.MockTransferClient.Setup(x => x.Client).Returns(configuration.Client);
							return this.MockTransferClient.Object;
						});
			this.MockRelativityTransferHost
				.Setup(x => x.CreateClientAsync(It.IsAny<ClientConfiguration>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(this.MockTransferClient.Object);
			this.MockRelativityTransferHost
				.Setup(
					x => x.CreateClientAsync(
						It.IsAny<ClientConfiguration>(),
						It.IsAny<ITransferClientStrategy>(),
						It.IsAny<CancellationToken>())).ReturnsAsync(this.MockTransferClient.Object);
			this.MockTapiObjectService = new Mock<ITapiObjectService>();
			this.MockTapiObjectService.Setup(
					x => x.CreateRelativityTransferHost(It.IsAny<Relativity.Transfer.RelativityConnectionInfo>(), It.IsAny<ITransferLog>()))
				.Returns(this.MockRelativityTransferHost.Object);
			this.MockTapiObjectService.Setup(x => x.CreateRelativityConnectionInfo(It.IsAny<TapiBridgeParameters2>()))
				.Returns(new Relativity.Transfer.RelativityConnectionInfo());
			this.MockTapiObjectService.Setup(x => x.GetClientId(It.IsAny<TapiBridgeParameters2>())).Returns(
				(TapiBridgeParameters2 parameters) => this.realObjectService.GetClientId(parameters));
			this.MockTapiObjectService.Setup(x => x.GetTapiClient(It.IsAny<Guid>()))
				.Returns((Guid id) => this.realObjectService.GetTapiClient(id));
			this.OnSetup();
		}

		[TearDown]
		public void Teardown()
		{
			if (this.TapiBridgeInstance != null)
			{
				this.TapiBridgeInstance.Dispose();
				this.TapiBridgeInstance = null;
			}

			if (this.CancellationTokenSource != null)
			{
				this.CancellationTokenSource.Dispose();
				this.CancellationTokenSource = null;
			}

			this.OnTeardown();
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldDefaultTheTapiProperties()
		{
			this.CreateTapiBridge(WellKnownTransferClient.Unassigned);
			Assert.That(this.TapiBridgeInstance.Client, Is.EqualTo(TapiClient.None));
			Assert.That(this.TapiBridgeInstance.ClientId, Is.EqualTo(Guid.Empty));
			Assert.That(this.TapiBridgeInstance.TargetPath, Is.Null.Or.Empty);
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldCreateTheTransferClientAsynchronously()
		{
			this.CreateTapiBridge(WellKnownTransferClient.Unassigned);

			// Adding a transfer path is expected to construct a transfer host, client, and job.
			this.TapiBridgeInstance.AddPath(TestTransferPath);
			this.MockTapiObjectService.Verify(
				x => x.CreateRelativityTransferHost(It.IsAny<Relativity.Transfer.RelativityConnectionInfo>(), It.IsAny<ITransferLog>()));
			this.MockRelativityTransferHost.Verify(
				x => x.CreateClientAsync(
					It.IsAny<ClientConfiguration>(),
					It.IsAny<ITransferClientStrategy>(),
					It.IsAny<CancellationToken>()));
			this.MockTransferClient.Verify(
				x => x.CreateJobAsync(It.IsAny<ITransferRequest>(), It.IsAny<CancellationToken>()));
			Assert.That(this.ChangedTapiClient, Is.Not.EqualTo(TapiClient.None));
		}

		[Test]
		[TestCase(WellKnownTransferClient.FileShare)]
		[TestCase(WellKnownTransferClient.Aspera)]
		[TestCase(WellKnownTransferClient.Http)]
		[Category(TestCategories.TransferApi)]
		public void ShouldCreateTheTransferClientSynchronously(WellKnownTransferClient client)
		{
			this.CreateTapiBridge(client);

			// Adding a transfer path is expected to perform the following:
			// 1. Construct a transfer host
			// 2. Construct a transfer client
			// 3. Construct a transfer job
			this.TapiBridgeInstance.AddPath(TestTransferPath);
			this.MockTapiObjectService.Verify(
				x => x.CreateRelativityTransferHost(It.IsAny<Relativity.Transfer.RelativityConnectionInfo>(), It.IsAny<ITransferLog>()));
			this.MockRelativityTransferHost.Verify(x => x.CreateClient(It.IsAny<ClientConfiguration>()));
			this.MockTransferClient.Verify(
				x => x.CreateJobAsync(It.IsAny<ITransferRequest>(), It.IsAny<CancellationToken>()));
			Assert.That(this.ChangedTapiClient, Is.Not.EqualTo(TapiClient.None));
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldPublishProgressEvents()
		{
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			const int TotalEvents = 10;
			for (int i = 1; i <= TotalEvents; i++)
			{
				long bytesTransferred = RandomHelper.NextInt64(1000, 100000);
				bool completed = RandomHelper.NextBoolean();
				TransferPathStatus status = (i % 2 == 0)
					                            ? TransferPathStatus.Successful
					                            : (TransferPathStatus)RandomHelper.NextEnum(typeof(TransferPathStatus));
				TransferPath path = this.CreateTransferPath(i);
				path.Order = i;
				this.TestTransferContext.PublishTransferPathProgress(
					new TransferRequest(),
					new TransferPathResult
						{
							BytesTransferred = bytesTransferred,
							Completed = completed,
							EndTime = completed ? DateTime.Now : (DateTime?)null,
							Path = path,
							Status = status,
							StartTime = DateTime.Now,
						});

				Assert.That(this.ProgressEvents.Count, Is.EqualTo(i));
				TapiProgressEventArgs args = this.ProgressEvents.LastOrDefault();
				Assert.That(args, Is.Not.Null);
				Assert.That(args.Completed, Is.EqualTo(completed));
				Assert.That(args.EndTime, Is.Not.Null);
				Assert.That(args.FileBytes, Is.EqualTo(bytesTransferred));
				Assert.That(args.FileName, Is.Not.Null.Or.Empty);
				Assert.That(args.LineNumber, Is.EqualTo(i));
				Assert.That(args.StartTime, Is.Not.Null);
				Assert.That(args.Successful, Is.EqualTo(status == TransferPathStatus.Successful));
			}
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		[Category(TestCategories.TransferApi)]
		public void ShouldPublishLargeFileProgressEventsWhenEnabled(bool enabled)
		{
			this.TestTransferContext.LargeFileProgressEnabled = enabled;
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			const int TotalEvents = 5;
			const double ProgressPerEvent = TotalEvents / 1;
			for (int i = 1; i <= TotalEvents; i++)
			{
				TransferPath path = TestTransferPath.DeepCopy();
				path.Order = i;
				this.TestTransferContext.PublishLargeFileProgress(
					new LargeFileProgressEventArgs(path, i, TotalEvents, i * ProgressPerEvent));
				if (!enabled)
				{
					Assert.That(this.LargeFileProgressEvents.Count, Is.Zero);
				}
				else
				{
					Assert.That(this.LargeFileProgressEvents.Count, Is.EqualTo(i));
					TapiLargeFileProgressEventArgs args = this.LargeFileProgressEvents.LastOrDefault();
					Assert.That(args, Is.Not.Null);
					Assert.That(args.LineNumber, Is.EqualTo(i));
					Assert.That(args.Path, Is.EqualTo(path));
					Assert.That(args.Progress, Is.EqualTo(i * ProgressPerEvent));
					Assert.That(args.TotalRequestBytes, Is.EqualTo(TotalEvents));
					Assert.That(args.TotalTransferredBytes, Is.EqualTo(i));
				}
			}
		}

		[Test]
		[TestCase(false, false)]
		[TestCase(false, true)]
		[TestCase(true, false)]
		[TestCase(true, true)]
		[Category(TestCategories.TransferApi)]
		public void ShouldNotThrowWhenWaitingForEmptyBatchedTransfers(bool keepJobAlive, bool createJob)
		{
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			if (createJob)
			{
				// Simulate the scenario where a job has been created but no paths were added to the queue.
				this.TapiBridgeInstance.AddPath(TestTransferPath);
				this.TapiBridgeInstance.ClearAllTotals();
			}

			TapiTotals totals = this.TapiBridgeInstance.WaitForTransfers(
				TestWaitMessage,
				TestSuccessMessage,
				TestErrorMessage,
				keepJobAlive);
			Assert.That(totals.TotalFileTransferRequests, Is.Zero);
			Assert.That(totals.TotalCompletedFileTransfers, Is.Zero);
			Assert.That(totals.TotalSuccessfulFileTransfers, Is.Zero);
			this.MockTransferLogger.Verify(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()));
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldWaitForBatchedTransfers()
		{
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			this.TapiBridgeInstance.AddPath(TestTransferPath);
			this.TestTransferContext.PublishTransferPathProgress(
				new TransferRequest(),
				new TransferPathResult
					{
						Completed = true,
						Path = TestTransferPath,
						Status = TransferPathStatus.Successful
					});

			const bool KeepJobAlive = true;
			TapiTotals totals = this.TapiBridgeInstance.WaitForTransfers(
				TestWaitMessage,
				TestSuccessMessage,
				TestErrorMessage,
				KeepJobAlive);
			Assert.That(totals.TotalCompletedFileTransfers, Is.EqualTo(1));
			Assert.That(totals.TotalFileTransferRequests, Is.EqualTo(1));
			Assert.That(totals.TotalSuccessfulFileTransfers, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.Client, Is.EqualTo(TapiClient.Direct));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalCompletedFileTransfers, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalFileTransferRequests, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalSuccessfulFileTransfers, Is.EqualTo(1));
			this.MockTransferJob.Verify(x => x.Dispose(), Times.Never);
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldWaitForBatchedTransfersAndHandleNonFatalExceptions()
		{
			// Simulate a scenario where a transfer job throws an exception, is handled, and continues.
			this.MockTransferJob.Setup(x => x.Status).Throws(new InvalidOperationException());
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			const int TotalFiles = 5;
			for (int i = 0; i < TotalFiles; i++)
			{
				TransferPath path = this.CreateTransferPath(i + 1);
				this.TapiBridgeInstance.AddPath(path);
				this.TestTransferContext.PublishTransferPathProgress(
					new TransferRequest(),
					new TransferPathResult
						{
							Completed = true,
							Path = path,
							Status = TransferPathStatus.Successful
						});
			}

			const bool KeepJobAlive = true;
			TapiTotals totals = this.TapiBridgeInstance.WaitForTransfers(
				TestWaitMessage,
				TestSuccessMessage,
				TestErrorMessage,
				KeepJobAlive);
			Assert.That(totals.TotalCompletedFileTransfers, Is.EqualTo(TotalFiles));
			Assert.That(totals.TotalFileTransferRequests, Is.EqualTo(TotalFiles));
			Assert.That(totals.TotalSuccessfulFileTransfers, Is.EqualTo(TotalFiles));
			Assert.That(this.ChangedTapiClient, Is.EqualTo(TapiClient.Web));
			Assert.That(this.TapiBridgeInstance.Client, Is.EqualTo(TapiClient.Web));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalCompletedFileTransfers, Is.EqualTo(TotalFiles));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalFileTransferRequests, Is.EqualTo(TotalFiles));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalSuccessfulFileTransfers, Is.EqualTo(TotalFiles));
			this.MockTransferJob.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA2201:DoNotRaiseReservedExceptionTypes",
			Justification = "This is for testing purposes only.")]
		public void ShouldWaitForBatchedTransfersAndRethrowTheFatalException()
		{
			// Simulate a scenario where a transfer job throws an exception, is handled, and rethrown when fatal.
			this.MockTransferJob.Setup(x => x.Status).Throws(new OutOfMemoryException());
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			TransferPath path = this.CreateTransferPath(1);
			this.TapiBridgeInstance.AddPath(path);
			this.TestTransferContext.PublishTransferPathProgress(
				new TransferRequest(),
				new TransferPathResult
					{
						Completed = false,
						Path = path,
						Status = TransferPathStatus.Failed
					});

			const bool KeepJobAlive = true;
			Assert.Throws<OutOfMemoryException>(
				() => this.TapiBridgeInstance.WaitForTransfers(
					TestWaitMessage,
					TestSuccessMessage,
					TestErrorMessage,
					KeepJobAlive));
			Assert.That(this.ChangedTapiClient, Is.EqualTo(TapiClient.Direct));
			Assert.That(this.TapiBridgeInstance.Client, Is.EqualTo(TapiClient.Direct));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalCompletedFileTransfers, Is.EqualTo(0));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalFileTransferRequests, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalSuccessfulFileTransfers, Is.EqualTo(0));
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		[Category(TestCategories.TransferApi)]
		public void ShouldCancelTheBatchedTransfersWhenRequested(bool transferJobStatusIsCanceled)
		{
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			this.TapiBridgeInstance.AddPath(TestTransferPath);
			if (transferJobStatusIsCanceled)
			{
				this.MockTransferJob.Setup(x => x.Status).Returns(TransferJobStatus.Canceled);
			}

			this.CancellationTokenSource.Cancel();

			const bool KeepJobAlive = true;
			Assert.Throws<OperationCanceledException>(
				() => this.TapiBridgeInstance.WaitForTransfers(
					TestWaitMessage,
					TestSuccessMessage,
					TestErrorMessage,
					KeepJobAlive));
			this.MockTransferJob.Verify(x => x.Dispose(), Times.Once());

			// Ensure the bridge never switches to web mode during cancel request.
			Assert.That(this.ChangedTapiClient, Is.Not.EqualTo(TapiClient.Web));
			Assert.That(this.TapiBridgeInstance.Client, Is.Not.EqualTo(TapiClient.Web));
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldCancelTheBatchedTransfersWhenCatchingTheOperationCanceledException()
		{
			this.MockTransferJob.Setup(x => x.Status).Throws(new OperationCanceledException());
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			this.TapiBridgeInstance.AddPath(TestTransferPath);

			const bool KeepJobAlive = true;
			Assert.Throws<OperationCanceledException>(
				() => this.TapiBridgeInstance.WaitForTransfers(
					TestWaitMessage,
					TestSuccessMessage,
					TestErrorMessage,
					KeepJobAlive));
			this.MockTransferJob.Verify(x => x.Dispose(), Times.Once());
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldWaitForJobTransfers()
		{
			this.MockTransferJob.Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>())).Returns(
				Task.FromResult(new TransferResult { Status = TransferStatus.Successful } as ITransferResult));
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			this.TapiBridgeInstance.AddPath(TestTransferPath);
			this.TestTransferContext.PublishTransferPathProgress(
				new TransferRequest(),
				new TransferPathResult
					{
						Completed = true, Path = TestTransferPath, Status = TransferPathStatus.Successful
					});

			const bool KeepJobAlive = false;
			TapiTotals totals = this.TapiBridgeInstance.WaitForTransfers(
				TestWaitMessage,
				TestSuccessMessage,
				TestErrorMessage,
				KeepJobAlive);
			Assert.That(totals.TotalCompletedFileTransfers, Is.EqualTo(1));
			Assert.That(totals.TotalFileTransferRequests, Is.EqualTo(1));
			Assert.That(totals.TotalSuccessfulFileTransfers, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.Client, Is.EqualTo(TapiClient.Direct));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalCompletedFileTransfers, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalFileTransferRequests, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalSuccessfulFileTransfers, Is.EqualTo(1));
			this.MockTransferJob.Verify(x => x.Dispose(), Times.Once());
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldWaitForJobTransfersWhenTheFirstJobIsFatal()
		{
			// The job is fatal on the first attempt and successful on the second.
			this.MockTransferJob.SetupSequence(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(new TransferResult { Status = TransferStatus.Fatal } as ITransferResult))
				.Returns(Task.FromResult(new TransferResult { Status = TransferStatus.Successful } as ITransferResult));
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			this.TapiBridgeInstance.AddPath(TestTransferPath);
			this.TestTransferContext.PublishTransferPathProgress(
				new TransferRequest(),
				new TransferPathResult
					{
						Completed = true,
						Path = TestTransferPath,
						Status = TransferPathStatus.Successful
					});

			const bool KeepJobAlive = false;
			TapiTotals totals = this.TapiBridgeInstance.WaitForTransfers(
				TestWaitMessage,
				TestSuccessMessage,
				TestErrorMessage,
				KeepJobAlive);

			// Retrying the job forces a switch to web mode.
			Assert.That(totals.TotalCompletedFileTransfers, Is.EqualTo(1));
			Assert.That(totals.TotalFileTransferRequests, Is.EqualTo(1));
			Assert.That(totals.TotalSuccessfulFileTransfers, Is.EqualTo(1));
			Assert.That(this.ChangedTapiClient, Is.EqualTo(TapiClient.Web));
			Assert.That(this.TapiBridgeInstance.Client, Is.EqualTo(TapiClient.Web));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalCompletedFileTransfers, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalFileTransferRequests, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalSuccessfulFileTransfers, Is.EqualTo(1));
			this.MockTransferJob.Verify(x => x.Dispose(), Times.Exactly(2));
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldWaitForJobTransfersWhenTheMaxInactivityIsExceeded()
		{
			// This will force a fallback to the job.
			this.TestMaxInactivitySeconds = 0;
			this.MockTransferJob.Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>())).Returns(
				Task.FromResult(new TransferResult { Status = TransferStatus.Successful } as ITransferResult));
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			this.TapiBridgeInstance.AddPath(TestTransferPath);

			const bool KeepJobAlive = true;
			TapiTotals totals = this.TapiBridgeInstance.WaitForTransfers(
				TestWaitMessage,
				TestSuccessMessage,
				TestErrorMessage,
				KeepJobAlive);
			Assert.That(totals.TotalCompletedFileTransfers, Is.EqualTo(0));
			Assert.That(totals.TotalFileTransferRequests, Is.EqualTo(1));
			Assert.That(totals.TotalSuccessfulFileTransfers, Is.EqualTo(0));
			Assert.That(this.TapiBridgeInstance.Client, Is.EqualTo(TapiClient.Direct));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalCompletedFileTransfers, Is.EqualTo(0));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalFileTransferRequests, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalSuccessfulFileTransfers, Is.EqualTo(0));
			this.MockTransferJob.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void ShouldAddPathsAndHandleNonFatalExceptions(bool keepJobAlive)
		{
			// Note: recent Moq releases allow sequencing void methods; however, it requires Castle v4 and is incompatible with our legacy version of Castle :(
			bool firstTime = true;
			this.queuedJobTransferPaths.Clear();
			this.MockTransferJob.Setup(x => x.AddPath(It.IsAny<TransferPath>(), It.IsAny<CancellationToken>()))
				.Callback(
					(TransferPath path, CancellationToken token) =>
						{
							if (firstTime)
							{
								firstTime = false;
								throw new TransferException("Simulated 1st time transfer exception");
							}

							this.queuedJobTransferPaths.Add(
								new JobTransferPath { Path = path, Status = TransferPathStatus.Started });
						});
			this.MockTransferJob.Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>())).Returns(
				Task.FromResult(new TransferResult { Status = TransferStatus.Successful } as ITransferResult));
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			this.TapiBridgeInstance.AddPath(TestTransferPath);
			this.TestTransferContext.PublishTransferPathProgress(
				new TransferRequest(),
				new TransferPathResult
					{
						Completed = true, Path = TestTransferPath, Status = TransferPathStatus.Successful
					});
			TapiTotals totals = this.TapiBridgeInstance.WaitForTransfers(
				TestWaitMessage,
				TestSuccessMessage,
				TestErrorMessage,
				keepJobAlive);
			Assert.That(totals.TotalFileTransferRequests, Is.EqualTo(1));
			Assert.That(totals.TotalCompletedFileTransfers, Is.EqualTo(1));
			Assert.That(totals.TotalSuccessfulFileTransfers, Is.EqualTo(1));
			Assert.That(this.ChangedTapiClient, Is.EqualTo(TapiClient.Web));
			Assert.That(this.TapiBridgeInstance.Client, Is.EqualTo(TapiClient.Web));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalCompletedFileTransfers, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalFileTransferRequests, Is.EqualTo(1));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalSuccessfulFileTransfers, Is.EqualTo(1));
			this.MockTransferJob.Verify(x => x.Dispose(), keepJobAlive ? Times.Once() : Times.Exactly(2));
		}

		[Test]
		[TestCase(1, 0)]
		[TestCase(2, 1)]
		[TestCase(3, 2)]
		[Category(TestCategories.TransferApi)]
		public void ShouldSwitchToWebModeWhenTheJobStatusIsNotRunningOrRetrying(int totalPaths, int retryablePaths)
		{
			// The switch should always take place regardless of the number of retryable paths.
			List<TransferPath> paths = new List<TransferPath>();
			for (int i = 0; i < totalPaths; i++)
			{
				TransferPath path = this.CreateTransferPath(i + 1);
				paths.Add(path);
			}

			this.MockTransferJobService.Setup(x => x.GetRetryableRequestTransferPaths())
				.Returns(paths.GetRange(0, retryablePaths));

			// Force a switch to web mode during the first check. The transfer job remains successful thereafter.
			this.MockTransferJob.SetupSequence(x => x.Status).Returns(TransferJobStatus.Fatal)
				.Returns(TransferJobStatus.Running);
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			this.queuedJobTransferPaths.Clear();
			foreach (TransferPath retryablePath in paths)
			{
				this.TapiBridgeInstance.AddPath(retryablePath);
				this.TestTransferContext.PublishTransferPathProgress(
					new TransferRequest(),
					new TransferPathResult
						{
							Completed = true,
							Path = retryablePath,
							Status = TransferPathStatus.Successful
						});

				this.queuedJobTransferPaths.Add(new JobTransferPath { Path = retryablePath });
			}

			const bool KeepJobAlive = true;
			TapiTotals totals = this.TapiBridgeInstance.WaitForTransfers(
				TestWaitMessage,
				TestSuccessMessage,
				TestErrorMessage,
				KeepJobAlive);
			Assert.That(totals.TotalCompletedFileTransfers, Is.EqualTo(totalPaths));
			Assert.That(totals.TotalFileTransferRequests, Is.EqualTo(totalPaths));
			Assert.That(totals.TotalSuccessfulFileTransfers, Is.EqualTo(totalPaths));
			Assert.That(this.ChangedTapiClient, Is.EqualTo(TapiClient.Web));
			Assert.That(this.TapiBridgeInstance.Client, Is.EqualTo(TapiClient.Web));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalCompletedFileTransfers, Is.EqualTo(totalPaths));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalFileTransferRequests, Is.EqualTo(totalPaths));
			Assert.That(this.TapiBridgeInstance.JobTotals.TotalSuccessfulFileTransfers, Is.EqualTo(totalPaths));
			this.MockTransferJob.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldThrowWhenAddingTheNullTransferPath()
		{
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			Assert.Throws<ArgumentNullException>(() => this.TapiBridgeInstance.AddPath(null));
		}

		[Test]
		[TestCase("", null)]
		[TestCase(" ", null)]
		[TestCase(null, null)]
		[TestCase("", 0)]
		[TestCase(" ", 0)]
		[TestCase(null, 0)]
		[TestCase("", -1)]
		[TestCase(" ", -1)]
		[TestCase(null, -1)]
		[Category(TestCategories.TransferApi)]
		public void ShouldThrowWhenAddingTheInvalidTransferPath(string path, long? sourcePathId)
		{
			foreach (TransferDirection direction in Enum.GetValues(typeof(TransferDirection)).Cast<TransferDirection>())
			{
				this.CreateTapiBridge(WellKnownTransferClient.FileShare);
				Assert.Throws<ArgumentException>(
					() => this.TapiBridgeInstance.AddPath(
						new TransferPath
							{
								Direction = direction, SourcePath = path, SourcePathId = sourcePathId
							}));
				this.TapiBridgeInstance.TargetPath = @"/";
				Assert.Throws<ArgumentException>(
					() => this.TapiBridgeInstance.AddPath(
						new TransferPath
							{
								Direction = direction,
								TargetPath = path
							}));
				this.TapiBridgeInstance.TargetPath = path;
				Assert.Throws<ArgumentException>(
					() => this.TapiBridgeInstance.AddPath(
						new TransferPath
							{
								Direction = direction,
								TargetPath = @"/"
						}));
			}
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		[Category(TestCategories.TransferApi)]
		public void ShouldThrowWhenSwitchingToWebModeWhenTheJobContainsPermissionIssues(bool keepJobAlive)
		{
			this.MockTransferJob.Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>())).Returns(
				Task.FromResult(new TransferResult { Status = TransferStatus.Fatal } as ITransferResult));
			this.MockTransferJob.Setup(x => x.Status).Returns(TransferJobStatus.Fatal);
			this.CreateTapiBridge(WellKnownTransferClient.FileShare);
			this.TapiBridgeInstance.AddPath(TestTransferPath);
			this.TestTransferContext.PublishTransferPathProgress(
				new TransferRequest(),
				new TransferPathResult
					{
						Completed = false,
						Path = TestTransferPath,
						Status = TransferPathStatus.Fatal
					});
			this.TestTransferContext.PublishTransferPathIssue(
				new TransferRequest(),
				new TransferIssue
					{
						Attributes = IssueAttributes.ReadWritePermissions,
						Path = TestTransferPath,
						Message = "You don't have permission to the file."
					});
			TransferException exception = Assert.Throws<TransferException>(
				() => this.TapiBridgeInstance.WaitForTransfers(
					TestWaitMessage,
					TestSuccessMessage,
					TestErrorMessage,
					keepJobAlive));
			Assert.That(this.ChangedTapiClient, Is.EqualTo(TapiClient.Direct));
			Assert.That(exception.Fatal, Is.True);
			this.MockTransferJob.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldThrowWhenSwitchingToWebModeWhenAlreadyInWebMode()
		{
			this.MockTransferJob.Setup(x => x.Status).Returns(TransferJobStatus.Fatal);
			this.CreateTapiBridge(WellKnownTransferClient.Http);
			this.TapiBridgeInstance.AddPath(TestTransferPath);
			this.TestTransferContext.PublishTransferPathProgress(
				new TransferRequest(),
				new TransferPathResult
					{
						Completed = false,
						Path = TestTransferPath,
						Status = TransferPathStatus.Failed
					});

			const bool KeepJobAlive = true;
			TransferException exception = Assert.Throws<TransferException>(
				() => this.TapiBridgeInstance.WaitForTransfers(
					TestWaitMessage,
					TestSuccessMessage,
					TestErrorMessage,
					KeepJobAlive));
			Assert.That(this.ChangedTapiClient, Is.EqualTo(TapiClient.Web));
			Assert.That(exception.Fatal, Is.True);
			this.MockTransferJob.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldForceCreatTheTransferClient([Values]WellKnownTransferClient client)
		{
			this.CreateTapiBridge(client);
			this.TapiBridgeInstance.CreateTransferClient();
			Assert.That(this.ChangedTapiClient, Is.Not.EqualTo(TapiClient.None));
		}

		protected abstract void CreateTapiBridge(WellKnownTransferClient client);

		protected virtual void OnTapiBridgeCreated()
		{
			this.TapiBridgeInstance.TapiClientChanged += (sender, args) => { this.ChangedTapiClient = args.Client; };
			this.TapiBridgeInstance.TapiProgress += (sender, e) =>
				{
					lock (this.syncRoot)
					{
						this.ProgressEvents.Add(e);
					}
				};
			this.TapiBridgeInstance.TapiLargeFileProgress += (sender, e) =>
				{
					lock (this.syncRoot)
					{
						this.LargeFileProgressEvents.Add(e);
					}
				};
		}

		protected virtual void OnSetup()
		{
		}

		protected virtual void OnTeardown()
		{
		}

		protected TapiBridgeParameters2 CreateTapiBridgeParameters(WellKnownTransferClient client)
		{
			TapiBridgeParameters2 parameters = new TapiBridgeParameters2
			{
				Credentials = new NetworkCredential(),
				WebServiceUrl = "https://relativity.one.com",
				WorkspaceId = this.TestWorkspaceId,
				ForceAsperaClient = false,
				ForceHttpClient = false,
				ForceFileShareClient = false,
				MaxInactivitySeconds = this.TestMaxInactivitySeconds,
			};

			this.ConfigureTapiBridgeParameters(parameters, client);
			return parameters;
		}

		protected void ConfigureTapiBridgeParameters(TapiBridgeParameters2 parameters, WellKnownTransferClient client)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			this.TestClientConfiguration.Client = client;
			switch (client)
			{
				case WellKnownTransferClient.Aspera:
					parameters.ForceAsperaClient = true;
					break;

				case WellKnownTransferClient.FileShare:
					parameters.ForceFileShareClient = true;
					break;

				case WellKnownTransferClient.Http:
					parameters.ForceHttpClient = true;
					break;
			}
		}

		protected TransferPath CreateTransferPath(int order)
		{
			TransferPath path = TestTransferPath.DeepCopy();
			path.SourcePath = RandomHelper.NextString(50, 100);
			path.TargetPath = RandomHelper.NextString(50, 100);
			path.Order = order;
			return path;
		}
	}
}