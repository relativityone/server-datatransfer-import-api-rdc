// ----------------------------------------------------------------------------
// <copyright file="ProcessContext.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;
	using System.Collections;
	using System.Threading;

	using Relativity.Import.Export.Resources;

	/// <summary>
	/// Represents a thread-safe context for a <see cref="IRunnable"/> process to publish events. This class cannot be inherited.
	/// </summary>
	public sealed class ProcessContext
	{
		/// <summary>
		/// The process error writer backing.
		/// </summary>
		private readonly IProcessErrorWriter errorWriter;

		/// <summary>
		/// The logger instance backing.
		/// </summary>
		private readonly Relativity.Logging.ILog logger;

		/// <summary>
		/// The application settings backing.
		/// </summary>
		private readonly IAppSettings settings;

		/// <summary>
		/// The cached setting to log all events.
		/// </summary>
		private bool? cachedLogAllEvents;

		/// <summary>
		/// The record count backing.
		/// </summary>
		private int recordCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessContext"/> class.
		/// </summary>
		/// <param name="errorWriter">
		/// The process error writer.
		/// </param>
		/// <param name="settings">
		/// The application settings.
		/// </param>
		/// <param name="logger">
		/// The logger instance.
		/// </param>
		public ProcessContext(IProcessErrorWriter errorWriter, IAppSettings settings, Relativity.Logging.ILog logger)
		{
			if (errorWriter == null)
			{
				throw new ArgumentNullException(nameof(errorWriter));
			}

			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			this.errorWriter = errorWriter;
			this.settings = settings;
			this.logger = logger;
			this.SafeMode = false;
		}

		/// <summary>
		/// Occurs when a runnable process non-fatal error is reported.
		/// </summary>
		public event EventHandler<ProcessErrorReportEventArgs> ErrorReport;

		/// <summary>
		/// Occurs when the runnable process throws a fatal exception.
		/// </summary>
		public event EventHandler<ProcessFatalExceptionEventArgs> FatalException;

		/// <summary>
		/// Occurs when the runnable process has completed.
		/// </summary>
		public event EventHandler<ProcessCompleteEventArgs> ProcessCompleted;

		/// <summary>
		/// Occurs when the runnable process has ended.
		/// </summary>
		public event EventHandler<ProcessEndEventArgs> ProcessEnded;

		/// <summary>
		/// Occurs when a runnable process event takes place.
		/// </summary>
		public event EventHandler<ProcessEventArgs> ProcessEvent;

		/// <summary>
		/// Occurs when a runnable process progress event takes place.
		/// </summary>
		public event EventHandler<ProcessProgressEventArgs> Progress;

		/// <summary>
		/// Occurs when the record count is incremented.
		/// </summary>
		public event EventHandler<ProcessRecordCountEventArgs> RecordCountIncremented;

		/// <summary>
		/// Occurs when the error report is displayed.
		/// </summary>
		public event EventHandler<ProcessShowReportEventArgs> ShowReportEvent;

		/// <summary>
		/// Occurs when the process has shutdown.
		/// </summary>
		public event EventHandler<EventArgs> Shutdown;

		/// <summary>
		/// Occurs when a status bar update takes place.
		/// </summary>
		public event EventHandler<StatusBarEventArgs> StatusBarUpdate;

		/// <summary>
		/// Gets or sets a value indicating whether safe mode is enabled.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when safe mode is enabled; otherwise, <see langword="false" />.
		/// </value>
		public bool SafeMode { get; set; }

		/// <summary>
		/// Clears this instance and assigns <see langword="null" /> to all events.
		/// </summary>
		public void Clear()
		{
			this.recordCount = 0;
			this.ErrorReport = null;
			this.FatalException = null;
			this.ProcessCompleted = null;
			this.ProcessEnded = null;
			this.ProcessEvent = null;
			this.Progress = null;
			this.RecordCountIncremented = null;
			this.Shutdown = null;
			this.StatusBarUpdate = null;
			this.cachedLogAllEvents = null;
		}

		/// <summary>
		/// Publishes an event indicating the runnable process is reporting a non-fatal error.
		/// </summary>
		/// <param name="error">
		/// The dictionary containing the error information.
		/// </param>
		public void PublishErrorReport(IDictionary error)
		{
			this.ErrorReport?.Invoke(this, new ProcessErrorReportEventArgs(error));
		}

		/// <summary>
		/// Publishes an event indicating the runnable process has handled a fatal exception.
		/// </summary>
		/// <param name="exception">
		/// The fatal exception.
		/// </param>
		public void PublishFatalException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			this.FatalException?.Invoke(this, new ProcessFatalExceptionEventArgs(exception));
			this.WriteError("FATAL ERROR", exception.ToString());
			this.LogFatalException(exception);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process has completed.
		/// </summary>
		/// <param name="closeForm">
		/// Specify whether to close any form that started the runnable process.
		/// </param>
		/// <param name="exportFilePath">
		/// The full path to the exported process file.
		/// </param>
		/// <param name="exportLog">
		/// Specify whether logs were exported.
		/// </param>
		public void PublishProcessComplete(bool closeForm, string exportFilePath, bool exportLog)
		{
			this.ProcessCompleted?.Invoke(this, new ProcessCompleteEventArgs(closeForm, exportFilePath, exportLog));
			this.errorWriter.Close();
			if (!closeForm && this.errorWriter.HasErrors)
			{
				ProcessErrorReport report = this.errorWriter.BuildErrorReport(CancellationToken.None);
				if (report == null)
				{
					throw new InvalidOperationException(Strings.BuildErrorReportArgError);
				}

				this.ShowReportEvent?.Invoke(
					this,
					new ProcessShowReportEventArgs(report.Report, report.MaxLengthExceeded));
			}
		}

		/// <summary>
		/// Publishes an event indicating the runnable process has ended.
		/// </summary>
		/// <param name="nativeFileBytes">
		/// The total number of native file bytes that were transferred.
		/// </param>
		/// <param name="metadataBytes">
		/// The total number of metadata bytes that were transferred.
		/// </param>
		public void PublishProcessEnded(long nativeFileBytes, long metadataBytes)
		{
			this.ProcessEnded?.Invoke(this, new ProcessEndEventArgs(nativeFileBytes, metadataBytes));
		}

		/// <summary>
		/// Publishes an event indicating the runnable process has handled a non-fatal error.
		/// </summary>
		/// <param name="recordInfo">
		/// The current record information.
		/// </param>
		/// <param name="message">
		/// The event message.
		/// </param>
		public void PublishProcessErrorEvent(string recordInfo, string message)
		{
			ProcessEventArgs args = new ProcessEventArgs(ProcessEventType.Error, recordInfo, message);
			this.ProcessEvent?.Invoke(this, args);
			this.LogProcessEvent(args);
			this.WriteError(recordInfo, message);
		}

		/// <summary>
		/// Publishes a process warning event.
		/// </summary>
		/// <param name="recordInfo">
		/// The current record information.
		/// </param>
		/// <param name="message">
		/// The event message.
		/// </param>
		public void PublishProcessWarningEvent(string recordInfo, string message)
		{
			ProcessEventArgs args = new ProcessEventArgs(ProcessEventType.Warning, recordInfo, message);
			this.ProcessEvent?.Invoke(this, args);
			this.LogProcessEvent(args);
		}

		/// <summary>
		/// Publishes a process status event.
		/// </summary>
		/// <param name="recordInfo">
		/// The current record information.
		/// </param>
		/// <param name="message">
		/// The event message.
		/// </param>
		public void PublishProcessStatusEvent(string recordInfo, string message)
		{
			ProcessEventArgs args = new ProcessEventArgs(ProcessEventType.Status, recordInfo, message);
			this.ProcessEvent?.Invoke(this, args);
			this.LogProcessEvent(args);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process progress event has occurred.
		/// </summary>
		/// <param name="totalRecords">
		/// The total number of records to process.
		/// </param>
		/// <param name="totalProcessedRecords">
		/// The total number of records that have been processed.
		/// </param>
		/// <param name="totalProcessedWarningRecords">
		/// The total number of records that have been processed with warnings.
		/// </param>
		/// <param name="totalProcessedErrorRecords">
		/// The total number of records that have been processed with errors.
		/// </param>
		/// <param name="startTime">
		/// The process start time.
		/// </param>
		/// <param name="timestamp">
		/// The timestamp when the event occurred.
		/// </param>
		/// <param name="metadataThroughput">
		/// The metadata throughput in MB/sec units.
		/// </param>
		/// <param name="nativeFileThroughput">
		/// The native file throughput in MB/sec units.
		/// </param>
		/// <param name="processId">
		/// The process unique identifier.
		/// </param>
		/// <param name="totalRecordsDisplay">
		/// The display string for the total number of records to process.
		/// </param>
		/// <param name="totalProcessedRecordsDisplay">
		/// The display string for the total number of records that have been processed.
		/// </param>
		/// <param name="metadata">
		/// The progress metadata.
		/// </param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1026:DefaultParametersShouldNotBeUsed",
			Justification = "This was done for backwards compatibility reasons.")]
		public void PublishProgress(
			long totalRecords,
			long totalProcessedRecords,
			long totalProcessedWarningRecords,
			long totalProcessedErrorRecords,
			DateTime startTime,
			DateTime timestamp,
			double metadataThroughput,
			double nativeFileThroughput,
			Guid processId,
			string totalRecordsDisplay = null,
			string totalProcessedRecordsDisplay = null,
			IDictionary metadata = null)
		{
			ProcessProgressEventArgs args = new ProcessProgressEventArgs(
				processId,
				metadata,
				startTime,
				timestamp,
				totalRecords,
				totalRecordsDisplay,
				totalProcessedRecords,
				totalProcessedRecordsDisplay,
				totalProcessedWarningRecords,
				totalProcessedErrorRecords,
				metadataThroughput,
				nativeFileThroughput);
			this.Progress?.Invoke(this, args);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process record count has incremented.
		/// </summary>
		public void PublishRecordCountIncremented()
		{
			this.recordCount++;
			this.RecordCountIncremented?.Invoke(this, new ProcessRecordCountEventArgs(this.recordCount));
		}

		/// <summary>
		/// Publishes an event indicating the runnable process has shutdown.
		/// </summary>
		public void PublishShutdown()
		{
			this.Shutdown?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Publishes a status bar update event.
		/// </summary>
		/// <param name="message">
		/// The status bar message
		/// </param>
		/// <param name="popupText">
		/// The status bar popup text.
		/// </param>
		public void PublishStatusBarUpdate(string message, string popupText)
		{
			this.StatusBarUpdate?.Invoke(this, new StatusBarEventArgs(message, popupText));
		}

		/// <summary>
		/// Halts the runnable process with the specified process unique identifier.
		/// </summary>
		/// <param name="processId">
		/// The process unique identifier.
		/// </param>
		public void PublishHaltProcess(Guid processId)
		{
			// TODO: Implement once the kCura assembly is migrated.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Terminate the runnable process with the specified process unique identifier.
		/// </summary>
		/// <param name="processId">
		/// The process unique identifier.
		/// </param>
		public void PublishParentFormClosing(Guid processId)
		{
			// TODO: Implement once the kCura assembly is migrated.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Exports the error file to the specified file.
		/// </summary>
		/// <param name="file">
		/// The full path to the export error file.
		/// </param>
		public void PublishExportErrorFile(string file)
		{
			// TODO: Implement once the kCura assembly is migrated.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Exports the error report to the specified file.
		/// </summary>
		/// <param name="file">
		/// The full path to the export error report file.
		/// </param>
		public void PublishExportErrorReport(string file)
		{
			// TODO: Implement once the kCura assembly is migrated.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Exports the server errors file to the specified file.
		/// </summary>
		/// <param name="file">
		/// The full path to the export server errors file.
		/// </param>
		public void PublishExportServerErrors(string file)
		{
			// TODO: Implement once the kCura assembly is migrated.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Saves the output file to the specified file path.
		/// </summary>
		/// <param name="file">
		/// The full path where the output file is saved.
		/// </param>
		public void SaveOutputFile(string file)
		{
			// TODO: Implement once the kCura assembly is migrated.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Logs the process event.
		/// </summary>
		/// <param name="args">
		/// The <see cref="Relativity.Import.Export.Process.ProcessEventArgs" /> instance containing the event data.
		/// </param>
		private void LogProcessEvent(ProcessEventArgs args)
		{
			if (this.cachedLogAllEvents == null)
			{
				this.cachedLogAllEvents = this.settings.LogAllEvents;
			}

			if (this.cachedLogAllEvents == true)
			{
				this.logger.LogInformation(
					"Event type: {EventType}, Message: {Message}, Record info: {RecordInfo}, Event timestamp: {Timestamp}",
					args.EventType,
					args.Message,
					args.RecordInfo,
					args.Timestamp);
			}
		}

		/// <summary>
		/// Logs the fatal exception.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		private void LogFatalException(Exception exception)
		{
			this.logger.LogFatal(exception, "A fatal exception has occurred.");
		}

		/// <summary>
		/// Writes the error to a file.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <param name="description">
		/// The description.
		/// </param>
		private void WriteError(string key, string description)
		{
			this.errorWriter.Write(key, description);
		}
	}
}