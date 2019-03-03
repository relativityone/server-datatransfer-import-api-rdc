// -----------------------------------------------------------------------------------------------------
// <copyright file="ProcessContextTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ProcessContext"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Linq;
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.Import.Export.Process;

	/// <summary>
	/// Represents <see cref="ProcessContext"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class ProcessContextTests
	{
		private readonly List<ProcessErrorReportEventArgs> errorReportEvents = new List<ProcessErrorReportEventArgs>();
		private readonly List<ProcessFatalExceptionEventArgs> fatalExceptionEvents = new List<ProcessFatalExceptionEventArgs>();
		private readonly List<ProcessCompleteEventArgs> processCompleteEvents = new List<ProcessCompleteEventArgs>();
		private readonly List<ProcessEndEventArgs> processEndEvents = new List<ProcessEndEventArgs>();
		private readonly List<ProcessEventArgs> processEvents = new List<ProcessEventArgs>();
		private readonly List<ProcessProgressEventArgs> progressEvents = new List<ProcessProgressEventArgs>();
		private readonly List<ProcessShowReportEventArgs> processShowReportEvents = new List<ProcessShowReportEventArgs>();
		private readonly List<EventArgs> shutdownEvents = new List<EventArgs>();
		private readonly List<ProcessRecordCountEventArgs> recordCountIncrementedEvents = new List<ProcessRecordCountEventArgs>();
		private readonly List<StatusBarEventArgs> statusBarEvents = new List<StatusBarEventArgs>();
		private Mock<IProcessErrorWriter> mockProcessWriter;
		private Mock<IAppSettings> mockAppSettings;
		private Mock<Relativity.Logging.ILog> mockLogger;
		private ProcessContext context;

		[SetUp]
		public void Setup()
		{
			this.mockProcessWriter = new Mock<IProcessErrorWriter>();
			this.mockProcessWriter.SetupGet(x => x.HasErrors).Returns(false);
			this.mockAppSettings = new Mock<IAppSettings>();
			this.mockAppSettings.SetupGet(settings => settings.LogAllEvents).Returns(true);
			this.mockLogger = new Mock<Relativity.Logging.ILog>();
			this.context = new ProcessContext(
				this.mockProcessWriter.Object,
				this.mockAppSettings.Object,
				this.mockLogger.Object);
			this.SetupEvents();
			this.errorReportEvents.Clear();
			this.fatalExceptionEvents.Clear();
			this.processCompleteEvents.Clear();
			this.processEndEvents.Clear();
			this.processEvents.Clear();
			this.progressEvents.Clear();
			this.processShowReportEvents.Clear();
			this.shutdownEvents.Clear();
			this.recordCountIncrementedEvents.Clear();
			this.statusBarEvents.Clear();
		}

		[TearDown]
		public void Teardown()
		{
			this.context = null;
		}

		[Test]
		public void ShouldPublishTheErrorReportEvent()
		{
			this.context.PublishErrorReport(new Dictionary<string, string>());
			Assert.That(this.errorReportEvents.Count, Is.EqualTo(1));

			// Assert that null events are handled.
			this.context.ErrorReport -= this.OnErrorReport;
			this.context.PublishErrorReport(new Dictionary<string, string>());
			Assert.That(this.errorReportEvents.Count, Is.EqualTo(1));
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ShouldPublishTheFatalExceptionEvent(bool logEvents)
		{
			// Note: fatal exceptions are always logged regardless of logEvents.
			this.ClearContext();
			this.mockLogger.Invocations.Clear();
			this.mockAppSettings.Invocations.Clear();
			this.mockAppSettings.SetupGet(settings => settings.LogAllEvents).Returns(logEvents);
			this.context.PublishFatalException(new InvalidOperationException());
			Assert.That(this.fatalExceptionEvents.Count, Is.EqualTo(1));

			// Assert that null events are handled.
			this.context.FatalException -= this.OnFatalException;
			this.context.PublishFatalException(new InvalidOperationException());
			Assert.That(this.fatalExceptionEvents.Count, Is.EqualTo(1));
			this.mockProcessWriter.Verify(writer => writer.Write(It.IsAny<string>(), It.IsAny<string>()));
			this.mockLogger.Verify(
				log => log.LogFatal(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Exactly(2));
		}

		[Test]
		public void ShouldPublishTheProcessCompletedEvent()
		{
			this.context.PublishProcessComplete(false, string.Empty, false);
			Assert.That(this.processCompleteEvents.Count, Is.EqualTo(1));

			// Assert that null events are handled.
			this.context.ProcessCompleted -= this.OnProcessCompleted;
			this.context.PublishProcessComplete(false, string.Empty, false);
			Assert.That(this.processCompleteEvents.Count, Is.EqualTo(1));
		}

		[Test]
		public void ShouldPublishTheProcessEndedEvent()
		{
			this.context.PublishProcessEnded(49, 99);
			Assert.That(this.processEndEvents.Count, Is.EqualTo(1));
			Assert.That(this.processEndEvents.Any(x => x.MetadataBytes == 99 && x.NativeFileBytes == 49), Is.True);

			// Assert that null events are handled.
			this.context.ProcessEnded -= this.OnProcessEnded;
			this.context.PublishProcessEnded(49, 99);
			Assert.That(this.processEndEvents.Count, Is.EqualTo(1));
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ShouldPublishTheProcessEvents(bool logEvents)
		{
			// Note: process events are only logged when configured.
			this.ClearContext();
			this.mockLogger.Invocations.Clear();
			this.mockAppSettings.Invocations.Clear();
			this.mockAppSettings.SetupGet(settings => settings.LogAllEvents).Returns(logEvents);
			this.context.PublishProcessErrorEvent("a", "b");
			Assert.That(this.processEvents.Count, Is.EqualTo(1));
			Assert.That(
				this.processEvents.Any(
					x => x.EventType == ProcessEventType.Error && x.RecordInfo == "a" && x.Message == "b"),
				Is.True);
			this.mockLogger.Verify(
				log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
				logEvents ? Times.Exactly(1) : Times.Never());
			this.context.PublishProcessWarningEvent("c", "d");
			Assert.That(this.processEvents.Count, Is.EqualTo(2));
			Assert.That(
				this.processEvents.Any(
					x => x.EventType == ProcessEventType.Warning && x.RecordInfo == "c" && x.Message == "d"),
				Is.True);
			this.mockLogger.Verify(
				log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
				logEvents ? Times.Exactly(2) : Times.Never());
			this.context.PublishProcessStatusEvent("e", "f");
			Assert.That(this.processEvents.Count, Is.EqualTo(3));
			Assert.That(
				this.processEvents.Any(
					x => x.EventType == ProcessEventType.Status && x.RecordInfo == "e" && x.Message == "f"),
				Is.True);
			this.mockLogger.Verify(
				log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
				logEvents ? Times.Exactly(3) : Times.Never());

			// Assert that null events are handled.
			this.context.ProcessEvent -= this.OnProcessEvent;
			this.context.PublishProcessErrorEvent("g", "h");
			this.context.PublishProcessWarningEvent("i", "j");
			this.context.PublishProcessStatusEvent("k", "l");
			Assert.That(this.processEvents.Count, Is.EqualTo(3));
			this.mockLogger.Verify(
				log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
				logEvents ? Times.Exactly(6) : Times.Never());
		}

		[Test]
		public void ShouldPublishTheProgressEvent()
		{
			Guid processId = Guid.NewGuid();
			DateTime startTime = DateTime.Now.Subtract(TimeSpan.FromHours(2));
			DateTime timestamp = DateTime.Now;
			string totalRecordsDisplay = "Total records: 100";
			string totalProcessedRecordsDisplay = "Total processed: 20";
			Dictionary<string, string> metadata = new Dictionary<string, string> { { "a", "1" }, { "b", "2" } };
			this.context.PublishProgress(
				100,
				20,
				30,
				40,
				startTime,
				timestamp,
				5.0,
				8.0,
				processId,
				totalRecordsDisplay,
				totalProcessedRecordsDisplay,
				metadata);
			Assert.That(this.progressEvents.Count, Is.EqualTo(1));
			ProcessProgressEventArgs eventArgs = this.progressEvents[0];
			Assert.That(eventArgs.Metadata, Is.Not.Null);
			Assert.That(eventArgs.Metadata.Contains("a"), Is.True);
			Assert.That(eventArgs.Metadata.Contains("b"), Is.True);
			Assert.That(eventArgs.MetadataThroughput, Is.EqualTo(5.0));
			Assert.That(eventArgs.NativeFileThroughput, Is.EqualTo(8.0));
			Assert.That(eventArgs.ProcessId, Is.EqualTo(processId));
			Assert.That(eventArgs.StartTime, Is.EqualTo(startTime));
			Assert.That(eventArgs.Timestamp, Is.EqualTo(timestamp));
			Assert.That(eventArgs.TotalProcessedErrorRecords, Is.EqualTo(40));
			Assert.That(eventArgs.TotalProcessedRecordsDisplay, Is.EqualTo(totalProcessedRecordsDisplay));
			Assert.That(eventArgs.TotalProcessedWarningRecords, Is.EqualTo(30));
			Assert.That(eventArgs.TotalRecords, Is.EqualTo(100));
			Assert.That(eventArgs.TotalRecordsDisplay, Is.EqualTo(totalRecordsDisplay));

			// Don't supply the optional parameters.
			this.context.PublishProgress(
				100,
				20,
				30,
				40,
				startTime,
				timestamp,
				5.0,
				8.0,
				processId);
			Assert.That(this.progressEvents.Count, Is.EqualTo(2));
			eventArgs = this.progressEvents[1];
			Assert.That(eventArgs.Metadata, Is.Null);
			Assert.That(eventArgs.TotalProcessedRecordsDisplay, Is.EqualTo("20"));
			Assert.That(eventArgs.TotalRecordsDisplay, Is.EqualTo("100"));

			// Assert that null events are handled.
			this.context.Progress -= this.OnProgress;
			this.context.PublishProgress(
				100,
				20,
				30,
				40,
				startTime,
				timestamp,
				5.0,
				8.0,
				processId,
				totalRecordsDisplay,
				totalProcessedRecordsDisplay,
				metadata);
			Assert.That(this.progressEvents.Count, Is.EqualTo(2));
		}

		[Test]
		public void ShouldPublishTheRecordCountIncrementedEvent()
		{
			this.context.PublishRecordCountIncremented();
			Assert.That(this.recordCountIncrementedEvents.Count, Is.EqualTo(1));
			Assert.That(this.recordCountIncrementedEvents.Any(x => x.Count == 1), Is.True);
			this.context.PublishRecordCountIncremented();
			Assert.That(this.recordCountIncrementedEvents.Count, Is.EqualTo(2));
			Assert.That(this.recordCountIncrementedEvents.Any(x => x.Count == 2), Is.True);
			this.context.PublishRecordCountIncremented();
			Assert.That(this.recordCountIncrementedEvents.Count, Is.EqualTo(3));
			Assert.That(this.recordCountIncrementedEvents.Any(x => x.Count == 3), Is.True);

			// Assert that null events are handled.
			this.context.RecordCountIncremented -= this.OnRecordCountIncremented;
			this.context.PublishRecordCountIncremented();
			Assert.That(this.recordCountIncrementedEvents.Count, Is.EqualTo(3));
		}

		[Test]
		public void ShouldPublishTheShowReportEvent()
		{
			// This event is only raised when "closeForm" is false and there are errors.
			this.context.PublishProcessComplete(false, "a", false);
			Assert.That(this.processShowReportEvents.Count, Is.EqualTo(0));
			this.context.PublishProcessComplete(true, "a", false);
			Assert.That(this.processShowReportEvents.Count, Is.EqualTo(0));

			// This should now publish the event.
			using (DataTable table = new DataTable())
			{
				table.Locale = CultureInfo.CurrentCulture;
				ProcessErrorReport report =
					new ProcessErrorReport { MaxLengthExceeded = false, Report = table };
				this.mockProcessWriter.Invocations.Clear();
				this.mockProcessWriter.SetupGet(x => x.HasErrors).Returns(true);
				this.mockProcessWriter.Setup(x => x.BuildErrorReport(It.IsAny<CancellationToken>())).Returns(report);
				this.context.PublishProcessComplete(false, "a", false);
				Assert.That(this.processShowReportEvents.Count, Is.EqualTo(1));
			}

			// Assert that null events are handled.
			this.context.ShowReportEvent -= this.OnShowReportEvents;
			this.context.PublishProcessComplete(false, "a", false);
			Assert.That(this.processShowReportEvents.Count, Is.EqualTo(1));
		}

		[Test]
		public void ShouldPublishTheShutdownEvent()
		{
			this.context.PublishShutdown();
			Assert.That(this.shutdownEvents.Count, Is.EqualTo(1));

			// Assert that null events are handled.
			this.context.Shutdown -= this.OnShutdown;
			this.context.PublishShutdown();
			Assert.That(this.shutdownEvents.Count, Is.EqualTo(1));
		}

		[Test]
		public void ShouldPublishTheStatusBarEvent()
		{
			this.context.PublishStatusBarUpdate("a", "b");
			Assert.That(this.statusBarEvents.Count, Is.EqualTo(1));
			Assert.That(this.statusBarEvents.Any(x => x.Message == "a" && x.PopupText == "b"), Is.True);
			this.context.PublishStatusBarUpdate("c", "d");
			Assert.That(this.statusBarEvents.Count, Is.EqualTo(2));
			Assert.That(this.statusBarEvents.Any(x => x.Message == "c" && x.PopupText == "d"), Is.True);
			this.context.PublishStatusBarUpdate("e", "f");
			Assert.That(this.statusBarEvents.Count, Is.EqualTo(3));
			Assert.That(this.statusBarEvents.Any(x => x.Message == "e" && x.PopupText == "f"), Is.True);

			// Assert that null events are handled.
			this.context.StatusBarUpdate -= this.OnStatusBarUpdate;
			this.context.PublishStatusBarUpdate("g", "h");
			Assert.That(this.statusBarEvents.Count, Is.EqualTo(3));
		}

		private void OnErrorReport(object sender, ProcessErrorReportEventArgs e)
		{
			this.errorReportEvents.Add(e);
		}

		private void OnFatalException(object sender, ProcessFatalExceptionEventArgs e)
		{
			this.fatalExceptionEvents.Add(e);
		}

		private void OnProcessCompleted(object sender, ProcessCompleteEventArgs e)
		{
			this.processCompleteEvents.Add(e);
		}

		private void OnProcessEnded(object sender, ProcessEndEventArgs e)
		{
			this.processEndEvents.Add(e);
		}

		private void OnProcessEvent(object sender, ProcessEventArgs e)
		{
			this.processEvents.Add(e);
		}

		private void OnProgress(object sender, ProcessProgressEventArgs e)
		{
			this.progressEvents.Add(e);
		}

		private void OnRecordCountIncremented(object sender, ProcessRecordCountEventArgs e)
		{
			this.recordCountIncrementedEvents.Add(e);
		}

		private void OnShowReportEvents(object sender, ProcessShowReportEventArgs e)
		{
			this.processShowReportEvents.Add(e);
		}

		private void OnStatusBarUpdate(object sender, StatusBarEventArgs e)
		{
			this.statusBarEvents.Add(e);
		}

		private void OnShutdown(object sender, EventArgs e)
		{
			this.shutdownEvents.Add(e);
		}

		private void ClearContext()
		{
			this.context.Clear();
			this.SetupEvents();
		}

		private void SetupEvents()
		{
			this.context.ErrorReport += this.OnErrorReport;
			this.context.FatalException += this.OnFatalException;
			this.context.ProcessCompleted += this.OnProcessCompleted;
			this.context.ProcessEnded += this.OnProcessEnded;
			this.context.Progress += this.OnProgress;
			this.context.ProcessEvent += this.OnProcessEvent;
			this.context.RecordCountIncremented += this.OnRecordCountIncremented;
			this.context.ShowReportEvent += this.OnShowReportEvents;
			this.context.StatusBarUpdate += this.OnStatusBarUpdate;
			this.context.Shutdown += this.OnShutdown;
		}
	}
}