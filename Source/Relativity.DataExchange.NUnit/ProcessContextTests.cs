// -----------------------------------------------------------------------------------------------------
// <copyright file="ProcessContextTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ProcessContext"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Linq;
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.TestFramework;

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
		private readonly List<CancellationRequestEventArgs> cancellationRequestEvents = new List<CancellationRequestEventArgs>();
		private readonly List<ErrorReportEventArgs> errorReportEvents = new List<ErrorReportEventArgs>();
		private readonly List<ExportErrorEventArgs> exportErrorEvents = new List<ExportErrorEventArgs>();
		private readonly List<FatalExceptionEventArgs> fatalExceptionEvents = new List<FatalExceptionEventArgs>();
		private readonly List<FieldMappedEventArgs> fieldMappedEvents = new List<FieldMappedEventArgs>();
		private readonly List<ParentFormClosingEventArgs> parentFormClosingEvents = new List<ParentFormClosingEventArgs>();
		private readonly List<ProcessCompleteEventArgs> processCompleteEvents = new List<ProcessCompleteEventArgs>();
		private readonly List<ProcessEndEventArgs> processEndEvents = new List<ProcessEndEventArgs>();
		private readonly List<ProcessEventArgs> processEvents = new List<ProcessEventArgs>();
		private readonly List<ProgressEventArgs> processProgressEvents = new List<ProgressEventArgs>();
		private readonly List<RecordCountEventArgs> recordCountIncrementedEvents = new List<RecordCountEventArgs>();
		private readonly List<RecordNumberEventArgs> recordProcessedEvents = new List<RecordNumberEventArgs>();
		private readonly List<ShowReportEventArgs> showReportEvents = new List<ShowReportEventArgs>();
		private readonly List<EventArgs> shutdownEvents = new List<EventArgs>();
		private readonly List<StatusBarEventArgs> statusBarEvents = new List<StatusBarEventArgs>();
		private Mock<IProcessErrorWriter> mockProcessErrorWriter;
		private Mock<IProcessEventWriter> mockProcessEventWriter;
		private Mock<IAppSettings> mockAppSettings;
		private Mock<Relativity.Logging.ILog> mockLogger;
		private ProcessContext context;

		[SetUp]
		public void Setup()
		{
			this.mockProcessErrorWriter = new Mock<IProcessErrorWriter>();
			this.mockProcessErrorWriter.SetupGet(x => x.HasErrors).Returns(false);
			this.mockProcessEventWriter = new Mock<IProcessEventWriter>();
			this.mockProcessEventWriter.SetupGet(x => x.File).Returns(string.Empty);
			this.mockAppSettings = new Mock<IAppSettings>();
			this.mockAppSettings.SetupGet(settings => settings.LogAllEvents).Returns(true);
			this.mockLogger = new Mock<Relativity.Logging.ILog>();
			this.context = new ProcessContext(
				this.mockProcessEventWriter.Object,
				this.mockProcessErrorWriter.Object,
				this.mockAppSettings.Object,
				this.mockLogger.Object);
			this.SetupEvents();
			this.cancellationRequestEvents.Clear();
			this.errorReportEvents.Clear();
			this.exportErrorEvents.Clear();
			this.fatalExceptionEvents.Clear();
			this.fieldMappedEvents.Clear();
			this.parentFormClosingEvents.Clear();
			this.processCompleteEvents.Clear();
			this.processEndEvents.Clear();
			this.processEvents.Clear();
			this.processProgressEvents.Clear();
			this.showReportEvents.Clear();
			this.shutdownEvents.Clear();
			this.recordCountIncrementedEvents.Clear();
			this.recordProcessedEvents.Clear();
			this.statusBarEvents.Clear();
		}

		[TearDown]
		public void Teardown()
		{
			this.context = null;
		}

		[Test]
		public void ShouldPublishTheCancellationRequestEvent()
		{
			Guid processId = Guid.NewGuid();
			this.context.PublishCancellationRequest(processId);
			Assert.That(this.cancellationRequestEvents.Count, Is.EqualTo(1));
			Assert.That(this.cancellationRequestEvents.All(x => x.ProcessId == processId), Is.True);

			// Assert that null events are handled.
			this.context.CancellationRequest -= this.OnCancellationRequest;
			this.context.PublishCancellationRequest(processId);
			Assert.That(this.cancellationRequestEvents.Count, Is.EqualTo(1));
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
		public void ShouldPublishTheExportErrorFileEvent()
		{
			this.context.PublishExportErrorFile("a");
			Assert.That(this.exportErrorEvents.Count, Is.EqualTo(1));

			// Assert that null events are handled.
			this.context.ExportErrorFile -= this.OnExportErrorFile;
			this.context.PublishExportErrorFile("a");
			Assert.That(this.exportErrorEvents.Count, Is.EqualTo(1));
		}

		[Test]
		public void ShouldPublishTheExportErrorReportEvent()
		{
			this.context.PublishExportErrorReport("a");
			Assert.That(this.exportErrorEvents.Count, Is.EqualTo(1));

			// Assert that null events are handled.
			this.context.ExportErrorReport -= this.OnExportErrorReport;
			this.context.PublishExportErrorReport("a");
			Assert.That(this.exportErrorEvents.Count, Is.EqualTo(1));
		}

		[Test]
		public void ShouldPublishTheExportServerErrorsEvent()
		{
			this.context.PublishExportServerErrors("a");
			Assert.That(this.exportErrorEvents.Count, Is.EqualTo(1));

			// Assert that null events are handled.
			this.context.ExportServerErrors -= this.OnExportServerErrors;
			this.context.PublishExportServerErrors("a");
			Assert.That(this.exportErrorEvents.Count, Is.EqualTo(1));
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ShouldPublishTheFatalExceptionEvent(bool logEvents)
		{
			// Note: fatal exceptions are always logged regardless of logEvents.
			this.ClearContext();
			this.mockLogger.ResetCalls();
			this.mockAppSettings.ResetCalls();
			this.mockAppSettings.SetupGet(settings => settings.LogAllEvents).Returns(logEvents);
			this.context.PublishFatalException(new InvalidOperationException());
			Assert.That(this.fatalExceptionEvents.Count, Is.EqualTo(1));

			// Assert that null events are handled.
			this.context.FatalException -= this.OnFatalException;
			this.context.PublishFatalException(new InvalidOperationException());
			Assert.That(this.fatalExceptionEvents.Count, Is.EqualTo(1));
			this.mockProcessErrorWriter.Verify(writer => writer.Write(It.IsAny<string>(), It.IsAny<string>()));
			this.mockLogger.Verify(
				log => log.LogFatal(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Exactly(2));
		}

		[Test]
		public void ShouldPublishTheFieldMappedEvent()
		{
			string sourceField = RandomHelper.NextString(5, 25);
			string targetField = RandomHelper.NextString(5, 25);
			this.context.PublishFieldMapped(sourceField, targetField);
			Assert.That(this.fieldMappedEvents.Count, Is.EqualTo(1));
			Assert.That(
				this.fieldMappedEvents.All(x => x.SourceField == sourceField && x.TargetField == targetField),
				Is.True);

			// Assert that null events are handled.
			this.context.FieldMapped -= this.OnFieldMapped;
			this.context.PublishFieldMapped(sourceField, targetField);
			Assert.That(this.fieldMappedEvents.Count, Is.EqualTo(1));
		}

		[Test]
		public void ShouldPublishTheProcessCompletedEvent()
		{
			this.context.PublishProcessCompleted();
			Assert.That(this.processCompleteEvents.Count, Is.EqualTo(1));
			Assert.That(
				this.processCompleteEvents.All(
					x => x.CloseForm == false && x.ExportLog == false && string.IsNullOrEmpty(x.ExportFilePath)),
				Is.True);
			this.context.PublishProcessCompleted(false);
			Assert.That(this.processCompleteEvents.Count, Is.EqualTo(2));
			Assert.That(
				this.processCompleteEvents.All(
					x => x.CloseForm == false && x.ExportLog == false && string.IsNullOrEmpty(x.ExportFilePath)),
				Is.True);
			this.context.PublishProcessCompleted(false, string.Empty);
			Assert.That(this.processCompleteEvents.Count, Is.EqualTo(3));
			Assert.That(
				this.processCompleteEvents.All(
					x => x.CloseForm == false && x.ExportLog == false && string.IsNullOrEmpty(x.ExportFilePath)),
				Is.True);
			this.context.PublishProcessCompleted(false, string.Empty, false);
			Assert.That(this.processCompleteEvents.Count, Is.EqualTo(4));
			Assert.That(
				this.processCompleteEvents.All(
					x => x.CloseForm == false && x.ExportLog == false && string.IsNullOrEmpty(x.ExportFilePath)),
				Is.True);
			this.context.PublishProcessCompleted(true, "a", true);
			Assert.That(this.processCompleteEvents.Count, Is.EqualTo(5));
			Assert.That(
				this.processCompleteEvents.Any(
					x => x.CloseForm == true && x.ExportLog == true && x.ExportFilePath == "a"),
				Is.True);

			// Assert that null events are handled.
			this.context.ProcessCompleted -= this.OnProcessCompleted;
			this.context.PublishProcessCompleted();
			this.context.PublishProcessCompleted(false);
			this.context.PublishProcessCompleted(false, string.Empty);
			this.context.PublishProcessCompleted(false, string.Empty, false);
			Assert.That(this.processCompleteEvents.Count, Is.EqualTo(5));
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
		public void ShouldPublishTheProcessEvents(bool writeEvents)
		{
			// Note: process events are only logged when configured.
			this.ClearContext();
			this.mockLogger.ResetCalls();
			this.mockAppSettings.ResetCalls();
			this.mockAppSettings.SetupGet(settings => settings.LogAllEvents).Returns(writeEvents);
			this.context.SafeMode = false;
			this.context.PublishErrorEvent("a", "b");
			Assert.That(this.processEvents.Count, Is.EqualTo(1));
			Assert.That(
				this.processEvents.Any(
					x => x.EventType == ProcessEventType.Error && x.RecordInfo == "a" && x.Message == "b"),
				Is.True);
			this.mockProcessEventWriter.Verify(
				writer => writer.Write(It.IsAny<ProcessEventDto>()),
				writeEvents ? Times.Exactly(1) : Times.Never());
			this.context.PublishWarningEvent("c", "d");
			Assert.That(this.processEvents.Count, Is.EqualTo(2));
			Assert.That(
				this.processEvents.Any(
					x => x.EventType == ProcessEventType.Warning && x.RecordInfo == "c" && x.Message == "d"),
				Is.True);
			this.mockProcessEventWriter.Verify(
				writer => writer.Write(It.IsAny<ProcessEventDto>()),
				writeEvents ? Times.Exactly(2) : Times.Never());
			this.context.PublishStatusEvent("e", "f");
			Assert.That(this.processEvents.Count, Is.EqualTo(3));
			Assert.That(
				this.processEvents.Any(
					x => x.EventType == ProcessEventType.Status && x.RecordInfo == "e" && x.Message == "f"),
				Is.True);
			this.mockProcessEventWriter.Verify(
				writer => writer.Write(It.IsAny<ProcessEventDto>()),
				writeEvents ? Times.Exactly(3) : Times.Never());

			// Assert that null events are handled.
			this.context.ProcessEvent -= this.OnProcessEvent;
			this.context.PublishErrorEvent("g", "h");
			this.context.PublishWarningEvent("i", "j");
			this.context.PublishStatusEvent("k", "l");
			Assert.That(this.processEvents.Count, Is.EqualTo(3));
			this.mockProcessEventWriter.Verify(
				writer => writer.Write(It.IsAny<ProcessEventDto>()),
				writeEvents ? Times.Exactly(6) : Times.Never());
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
			Assert.That(this.processProgressEvents.Count, Is.EqualTo(1));
			ProgressEventArgs eventArgs = this.processProgressEvents[0];
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
			Assert.That(this.processProgressEvents.Count, Is.EqualTo(2));
			eventArgs = this.processProgressEvents[1];
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
			Assert.That(this.processProgressEvents.Count, Is.EqualTo(2));
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
		public void ShouldPublishTheRecordProcessedEvent()
		{
			this.context.PublishRecordProcessed(1);
			Assert.That(this.recordProcessedEvents.Count, Is.EqualTo(1));
			Assert.That(this.recordProcessedEvents.Any(x => x.RecordNumber == 1), Is.True);
			this.context.PublishRecordProcessed(2);
			Assert.That(this.recordProcessedEvents.Count, Is.EqualTo(2));
			Assert.That(this.recordProcessedEvents.Any(x => x.RecordNumber == 2), Is.True);
			this.context.PublishRecordProcessed(3);
			Assert.That(this.recordProcessedEvents.Count, Is.EqualTo(3));
			Assert.That(this.recordProcessedEvents.Any(x => x.RecordNumber == 3), Is.True);

			// Assert that null events are handled.
			this.context.RecordProcessed -= this.OnRecordProcessed;
			this.context.PublishRecordProcessed(4);
			Assert.That(this.recordProcessedEvents.Count, Is.EqualTo(3));
		}

		[Test]
		public void ShouldPublishTheShowReportEvent()
		{
			// This event is only raised when "closeForm" is false and there are errors.
			this.context.PublishProcessCompleted(false, "a", false);
			Assert.That(this.showReportEvents.Count, Is.EqualTo(0));
			this.context.PublishProcessCompleted(true, "a", false);
			Assert.That(this.showReportEvents.Count, Is.EqualTo(0));

			// This should now publish the event.
			using (DataTable table = new DataTable())
			{
				table.Locale = CultureInfo.CurrentCulture;
				ProcessErrorReport report =
					new ProcessErrorReport { MaxLengthExceeded = false, Report = table };
				this.mockProcessErrorWriter.ResetCalls();
				this.mockProcessErrorWriter.SetupGet(x => x.HasErrors).Returns(true);
				this.mockProcessErrorWriter.Setup(x => x.BuildErrorReport(It.IsAny<CancellationToken>())).Returns(report);
				this.context.PublishProcessCompleted(false, "a", false);
				Assert.That(this.showReportEvents.Count, Is.EqualTo(1));
			}

			// Assert that null events are handled.
			this.context.ShowReportEvent -= this.OnShowReportEvents;
			this.context.PublishProcessCompleted(false, "a", false);
			Assert.That(this.showReportEvents.Count, Is.EqualTo(1));
		}

		[Test]
		public void ShouldPublishTheParentFormClosingEvent()
		{
			Guid processId = Guid.NewGuid();
			this.context.PublishParentFormClosing(processId);
			Assert.That(this.parentFormClosingEvents.Count, Is.EqualTo(1));
			Assert.That(this.parentFormClosingEvents.All(x => x.ProcessId == processId), Is.True);

			// Assert that null events are handled.
			this.context.ParentFormClosing -= this.OnParentFormClosing;
			this.context.PublishParentFormClosing(processId);
			Assert.That(this.parentFormClosingEvents.Count, Is.EqualTo(1));
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
			this.context.PublishStatusBarChanged("a", "b");
			Assert.That(this.statusBarEvents.Count, Is.EqualTo(1));
			Assert.That(this.statusBarEvents.Any(x => x.Message == "a" && x.PopupText == "b"), Is.True);
			this.context.PublishStatusBarChanged("c", "d");
			Assert.That(this.statusBarEvents.Count, Is.EqualTo(2));
			Assert.That(this.statusBarEvents.Any(x => x.Message == "c" && x.PopupText == "d"), Is.True);
			this.context.PublishStatusBarChanged("e", "f");
			Assert.That(this.statusBarEvents.Count, Is.EqualTo(3));
			Assert.That(this.statusBarEvents.Any(x => x.Message == "e" && x.PopupText == "f"), Is.True);

			// Assert that null events are handled.
			this.context.StatusBarChanged -= this.OnStatusBarChanged;
			this.context.PublishStatusBarChanged("g", "h");
			Assert.That(this.statusBarEvents.Count, Is.EqualTo(3));
		}

		[Test]
		public void ShouldSaveTheOutputFile()
		{
			string targetFile = RandomHelper.NextString(10, 200);
			this.context.SaveOutputFile(targetFile);
			this.mockProcessEventWriter.Verify(x => x.Save(targetFile));
		}

		private void OnCancellationRequest(object sender, CancellationRequestEventArgs e)
		{
			this.cancellationRequestEvents.Add(e);
		}

		private void OnErrorReport(object sender, ErrorReportEventArgs e)
		{
			this.errorReportEvents.Add(e);
		}

		private void OnExportErrorFile(object sender, ExportErrorEventArgs e)
		{
			this.exportErrorEvents.Add(e);
		}

		private void OnExportErrorReport(object sender, ExportErrorEventArgs e)
		{
			this.exportErrorEvents.Add(e);
		}

		private void OnExportServerErrors(object sender, ExportErrorEventArgs e)
		{
			this.exportErrorEvents.Add(e);
		}

		private void OnFatalException(object sender, FatalExceptionEventArgs e)
		{
			this.fatalExceptionEvents.Add(e);
		}

		private void OnFieldMapped(object sender, FieldMappedEventArgs e)
		{
			this.fieldMappedEvents.Add(e);
		}

		private void OnParentFormClosing(object sender, ParentFormClosingEventArgs e)
		{
			this.parentFormClosingEvents.Add(e);
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

		private void OnProgress(object sender, ProgressEventArgs e)
		{
			this.processProgressEvents.Add(e);
		}

		private void OnRecordCountIncremented(object sender, RecordCountEventArgs e)
		{
			this.recordCountIncrementedEvents.Add(e);
		}

		private void OnRecordProcessed(object sender, RecordNumberEventArgs e)
		{
			this.recordProcessedEvents.Add(e);
		}

		private void OnShowReportEvents(object sender, ShowReportEventArgs e)
		{
			this.showReportEvents.Add(e);
		}

		private void OnStatusBarChanged(object sender, StatusBarEventArgs e)
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
			this.context.CancellationRequest += this.OnCancellationRequest;
			this.context.ErrorReport += this.OnErrorReport;
			this.context.ExportErrorFile += this.OnExportErrorFile;
			this.context.ExportErrorReport += this.OnExportErrorReport;
			this.context.ExportServerErrors += this.OnExportServerErrors;
			this.context.FatalException += this.OnFatalException;
			this.context.FieldMapped += this.OnFieldMapped;
			this.context.ParentFormClosing += this.OnParentFormClosing;
			this.context.ProcessCompleted += this.OnProcessCompleted;
			this.context.ProcessEnded += this.OnProcessEnded;
			this.context.ProcessEvent += this.OnProcessEvent;
			this.context.Progress += this.OnProgress;
			this.context.RecordCountIncremented += this.OnRecordCountIncremented;
			this.context.RecordProcessed += this.OnRecordProcessed;
			this.context.ShowReportEvent += this.OnShowReportEvents;
			this.context.StatusBarChanged += this.OnStatusBarChanged;
			this.context.Shutdown += this.OnShutdown;
		}
	}
}