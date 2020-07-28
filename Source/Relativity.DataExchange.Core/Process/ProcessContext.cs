// ----------------------------------------------------------------------------
// <copyright file="ProcessContext.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;
	using System.Collections;
	using System.Threading;

	using Relativity.DataExchange.Resources;
	using Relativity.Logging;

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
		/// The process event writer backing.
		/// </summary>
		private readonly IProcessEventWriter eventWriter;

		/// <summary>
		/// The logger instance backing.
		/// </summary>
		private readonly Relativity.Logging.ILog logger;

		/// <summary>
		/// The application settings backing.
		/// </summary>
		private readonly IAppSettings settings;

		/// <summary>
		/// The record count backing.
		/// </summary>
		private int recordCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessContext"/> class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "Both object perform no actual disposal.")]
		public ProcessContext()
			: this(new NullProcessEventWriter(), new NullProcessErrorWriter(), AppSettings.Instance, new NullLogger())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessContext"/> class.
		/// </summary>
		/// <param name="eventWriter">
		/// The process event writer.
		/// </param>
		/// <param name="errorWriter">
		/// The process error writer.
		/// </param>
		/// <param name="settings">
		/// The application settings.
		/// </param>
		/// <param name="logger">
		/// The logger instance.
		/// </param>
		public ProcessContext(
			IProcessEventWriter eventWriter,
			IProcessErrorWriter errorWriter,
			IAppSettings settings,
			Relativity.Logging.ILog logger)
		{
			if (eventWriter == null)
			{
				throw new ArgumentNullException(nameof(eventWriter));
			}

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

			this.eventWriter = eventWriter;
			this.errorWriter = errorWriter;
			this.settings = settings;
			this.logger = logger;
			this.SafeMode = false;
		}

		/// <summary>
		/// Occurs when the runnable process has been requested for cancellation.
		/// </summary>
		public event EventHandler<CancellationRequestEventArgs> CancellationRequest;

		/// <summary>
		/// Occurs when the runnable process non-fatal error is reported.
		/// </summary>
		public event EventHandler<ErrorReportEventArgs> ErrorReport;

		/// <summary>
		/// Occurs when the runnable process requests the export server errors file.
		/// </summary>
		public event EventHandler<ExportErrorEventArgs> ExportServerErrors;

		/// <summary>
		/// Occurs when the runnable process requests an export error report file.
		/// </summary>
		public event EventHandler<ExportErrorEventArgs> ExportErrorReport;

		/// <summary>
		/// Occurs when the runnable process requests an export error file.
		/// </summary>
		public event EventHandler<ExportErrorEventArgs> ExportErrorFile;

		/// <summary>
		/// Occurs when the runnable process throws a fatal exception.
		/// </summary>
		public event EventHandler<FatalExceptionEventArgs> FatalException;

		/// <summary>
		/// Occurs when the runnable process has mapped a source field to a target field.
		/// </summary>
		public event EventHandler<FieldMappedEventArgs> FieldMapped;

		/// <summary>
		/// Occurs when the runnable process associated with a parent form is closing.
		/// </summary>
		public event EventHandler<ParentFormClosingEventArgs> ParentFormClosing;

		/// <summary>
		/// Occurs when the runnable process has completed.
		/// </summary>
		public event EventHandler<ProcessCompleteEventArgs> ProcessCompleted;

		/// <summary>
		/// Occurs when the runnable process has ended.
		/// </summary>
		public event EventHandler<ProcessEndEventArgs> ProcessEnded;

		/// <summary>
		/// Occurs when the runnable process event takes place.
		/// </summary>
		public event EventHandler<ProcessEventArgs> ProcessEvent;

		/// <summary>
		/// Occurs when the runnable process progress event takes place.
		/// </summary>
		public event EventHandler<ProgressEventArgs> Progress;

		/// <summary>
		/// Occurs when the runnable process increments the record count.
		/// </summary>
		public event EventHandler<RecordCountEventArgs> RecordCountIncremented;

		/// <summary>
		/// Occurs when the runnable process has completed processing a single record.
		/// </summary>
		public event EventHandler<RecordNumberEventArgs> RecordProcessed;

		/// <summary>
		/// Occurs when the runnable process is requested to show the error report.
		/// </summary>
		public event EventHandler<ShowReportEventArgs> ShowReportEvent;

		/// <summary>
		/// Occurs when the runnable process has shutdown.
		/// </summary>
		public event EventHandler<EventArgs> Shutdown;

		/// <summary>
		/// Occurs when a status bar change takes place.
		/// </summary>
		public event EventHandler<StatusBarEventArgs> StatusBarChanged;

		/// <summary>
		/// Gets or sets input arguments.
		/// </summary>
		/// <value>
		/// The input arguments.
		/// </value>
		public object InputArgs
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether safe mode is enabled.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when safe mode is enabled; otherwise, <see langword="false" />.
		/// </value>
		public bool SafeMode
		{
			get;
			set;
		}

		/// <summary>
		/// Clears this instance and assigns <see langword="null" /> to all events.
		/// </summary>
		public void Clear()
		{
			this.recordCount = 0;
			this.CancellationRequest = null;
			this.ErrorReport = null;
			this.ExportErrorFile = null;
			this.ExportErrorReport = null;
			this.ExportServerErrors = null;
			this.FatalException = null;
			this.FieldMapped = null;
			this.ParentFormClosing = null;
			this.ProcessCompleted = null;
			this.ProcessEnded = null;
			this.ProcessEvent = null;
			this.Progress = null;
			this.RecordCountIncremented = null;
			this.ShowReportEvent = null;
			this.Shutdown = null;
			this.StatusBarChanged = null;
		}

		/// <summary>
		/// Halts the runnable process with the specified process unique identifier. This assumes cancellation is requested by the user.
		/// </summary>
		/// <param name="processId">
		/// The process unique identifier.
		/// </param>
		public void PublishCancellationRequest(Guid processId)
		{
			const bool RequestByUser = true;
			this.PublishCancellationRequest(processId, RequestByUser);
		}

		/// <summary>
		/// Halts the runnable process with the specified process unique identifier.
		/// </summary>
		/// <param name="processId">
		/// The process unique identifier.
		/// </param>
		/// <param name="requestByUser">
		/// <see langword="true" /> when cancellation is requested by the user; otherwise, <see langword="false" /> when requested to terminate the process.
		/// </param>
		public void PublishCancellationRequest(Guid processId, bool requestByUser)
		{
			CancellationRequestEventArgs args = new CancellationRequestEventArgs(processId, requestByUser);
			this.CancellationRequest?.Invoke(this, args);
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
		public void PublishErrorEvent(string recordInfo, string message)
		{
			ProcessEventArgs args = new ProcessEventArgs(ProcessEventType.Error, recordInfo, message);
			this.ProcessEvent?.Invoke(this, args);
			this.LogProcessEvent(args);
			this.WriteError(recordInfo, message);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process is reporting a non-fatal error.
		/// </summary>
		/// <param name="error">
		/// The dictionary containing the error information.
		/// </param>
		public void PublishErrorReport(IDictionary error)
		{
			ErrorReportEventArgs args = new ErrorReportEventArgs(error);
			this.ErrorReport?.Invoke(this, args);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process requests an export error file.
		/// </summary>
		/// <param name="file">
		/// The full path to the export error file.
		/// </param>
		public void PublishExportErrorFile(string file)
		{
			ExportErrorEventArgs args = new ExportErrorEventArgs(file);
			this.ExportErrorFile?.Invoke(this, args);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process requests an export error report file.
		/// </summary>
		/// <param name="file">
		/// The full path to the export error report file.
		/// </param>
		public void PublishExportErrorReport(string file)
		{
			ExportErrorEventArgs args = new ExportErrorEventArgs(file);
			this.ExportErrorReport?.Invoke(this, args);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process requests the export server errors file.
		/// </summary>
		/// <param name="file">
		/// The full path to the export server errors file.
		/// </param>
		public void PublishExportServerErrors(string file)
		{
			ExportErrorEventArgs args = new ExportErrorEventArgs(file);
			this.ExportServerErrors?.Invoke(this, args);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process has mapped a source field to a target field.
		/// </summary>
		/// <param name="sourceField">
		/// The source field name.
		/// </param>
		/// <param name="targetField">
		/// The target field name.
		/// </param>
		public void PublishFieldMapped(string sourceField, string targetField)
		{
			FieldMappedEventArgs args = new FieldMappedEventArgs(sourceField, targetField);
			this.FieldMapped?.Invoke(this, args);
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

			FatalExceptionEventArgs args = new FatalExceptionEventArgs(exception);
			this.FatalException?.Invoke(this, args);
			this.WriteError("FATAL ERROR", exception.ToString());
			this.LogProcessEvent(new ProcessEventArgs(ProcessEventType.Error, string.Empty, exception.ToString()));
			this.LogFatalException(exception);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process associated with a parent form is closing.
		/// </summary>
		/// <param name="processId">
		/// The process unique identifier.
		/// </param>
		public void PublishParentFormClosing(Guid processId)
		{
			ParentFormClosingEventArgs args = new ParentFormClosingEventArgs(processId);
			this.ParentFormClosing?.Invoke(this, args);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process has completed. By default,
		/// <c>closeForm</c> is <see langword="false" />,
		/// <c>exportFilePath</c> is <see cref="string.Empty"/>,
		/// and <c>exportLog</c> is <see langword="false" />.
		/// </summary>
		public void PublishProcessCompleted()
		{
			this.PublishProcessCompleted(false, string.Empty, false);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process has completed. By default,
		/// <c>exportFilePath</c> is <see cref="string.Empty"/>
		/// and <c>exportLog</c> is <see langword="false" />.
		/// </summary>
		/// <param name="closeForm">
		/// Specify whether to close any form that started the runnable process.
		/// </param>
		public void PublishProcessCompleted(bool closeForm)
		{
			this.PublishProcessCompleted(closeForm, string.Empty, false);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process has completed. By default,
		/// <c>exportLog</c> is <see langword="false" />.
		/// </summary>
		/// <param name="closeForm">
		/// Specify whether to close any form that started the runnable process.
		/// </param>
		/// <param name="exportFilePath">
		/// The full path to the exported process file.
		/// </param>
		public void PublishProcessCompleted(bool closeForm, string exportFilePath)
		{
			this.PublishProcessCompleted(closeForm, exportFilePath, false);
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
		public void PublishProcessCompleted(bool closeForm, string exportFilePath, bool exportLog)
		{
			ProcessCompleteEventArgs args1 = new ProcessCompleteEventArgs(closeForm, exportFilePath, exportLog);
			this.ProcessCompleted?.Invoke(this, args1);
			this.errorWriter.Close();
			if (!closeForm && this.errorWriter.HasErrors)
			{
				ProcessErrorReport report = this.errorWriter.BuildErrorReport(CancellationToken.None);
				if (report == null)
				{
					throw new InvalidOperationException(Strings.BuildErrorReportArgError);
				}

				ShowReportEventArgs args2 = new ShowReportEventArgs(report.Report, report.MaxLengthExceeded);
				this.ShowReportEvent?.Invoke(this, args2);
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
			ProcessEndEventArgs args = new ProcessEndEventArgs(nativeFileBytes, metadataBytes);
			this.ProcessEnded?.Invoke(this, args);
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
			ProgressEventArgs args = new ProgressEventArgs(
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
				nativeFileThroughput,
				ProgressEventArgs.UnitOfMeasurement.Records);
			this.Progress?.Invoke(this, args);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process progress event has occurred.
		/// Progress is measured in number of bytes.
		/// </summary>
		/// <param name="totalBytes">
		/// The total number of bytes to process.
		/// </param>
		/// <param name="processedBytes">
		/// The total number of bytes that have been processed.
		/// </param>
		/// <param name="startTime">
		/// The process start time.
		/// </param>
		/// <param name="timestamp">
		/// The timestamp when the event occurred.
		/// </param>
		/// <param name="processId">
		/// The process unique identifier.
		/// </param>
		/// <param name="totalBytesDisplay">
		/// The display string for the total number of bytes to process.
		/// </param>
		/// <param name="processedBytesDisplay">
		/// The display string for the total number of bytes that have been processed.
		/// </param>
		public void PublishProgressInBytes(
			long totalBytes,
			long processedBytes,
			DateTime startTime,
			DateTime timestamp,
			Guid processId,
			string totalBytesDisplay,
			string processedBytesDisplay)
		{
			ProgressEventArgs args = new ProgressEventArgs(
				processId,
				null,
				startTime,
				timestamp,
				totalBytes,
				totalBytesDisplay,
				processedBytes,
				processedBytesDisplay,
				0,
				0,
				0,
				0,
				ProgressEventArgs.UnitOfMeasurement.Bytes);
			this.Progress?.Invoke(this, args);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process record count has incremented.
		/// </summary>
		public void PublishRecordCountIncremented()
		{
			this.recordCount++;
			RecordCountEventArgs args = new RecordCountEventArgs(this.recordCount);
			this.RecordCountIncremented?.Invoke(this, args);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process has completed processing a single record.
		/// </summary>
		/// <param name="recordNumber">
		/// The record number.
		/// </param>
		public void PublishRecordProcessed(long recordNumber)
		{
			RecordNumberEventArgs args = new RecordNumberEventArgs(recordNumber);
			this.RecordProcessed?.Invoke(this, args);
		}

		/// <summary>
		/// Publishes an event indicating the runnable process has shutdown.
		/// </summary>
		public void PublishShutdown()
		{
			this.Shutdown?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Publishes a status bar change event.
		/// </summary>
		/// <param name="message">
		/// The status bar message.
		/// </param>
		/// <param name="popupText">
		/// The status bar popup text.
		/// </param>
		public void PublishStatusBarChanged(string message, string popupText)
		{
			StatusBarEventArgs args = new StatusBarEventArgs(message, popupText);
			this.StatusBarChanged?.Invoke(this, args);
		}

		/// <summary>
		/// Publishes a status event.
		/// </summary>
		/// <param name="recordInfo">
		/// The current record information.
		/// </param>
		/// <param name="message">
		/// The event message.
		/// </param>
		public void PublishStatusEvent(string recordInfo, string message)
		{
			ProcessEventArgs args = new ProcessEventArgs(ProcessEventType.Status, recordInfo, message);
			this.ProcessEvent?.Invoke(this, args);
			this.LogProcessEvent(args);
		}

		/// <summary>
		/// Saves the output file to the specified file path.
		/// </summary>
		/// <param name="file">
		/// The full path where the output file is saved.
		/// </param>
		public void SaveOutputFile(string file)
		{
			this.eventWriter.Save(file);
		}

		/// <summary>
		/// Publishes a warning event.
		/// </summary>
		/// <param name="recordInfo">
		/// The current record information.
		/// </param>
		/// <param name="message">
		/// The event message.
		/// </param>
		public void PublishWarningEvent(string recordInfo, string message)
		{
			ProcessEventArgs args = new ProcessEventArgs(ProcessEventType.Warning, recordInfo, message);
			this.ProcessEvent?.Invoke(this, args);
			this.LogProcessEvent(args);
		}

		/// <summary>
		/// Logs the process event.
		/// </summary>
		/// <param name="args">
		/// The <see cref="ProcessEventArgs" /> instance containing the event data.
		/// </param>
		private void LogProcessEvent(ProcessEventArgs args)
		{
			if (this.settings.LogAllEvents && !this.SafeMode)
			{
				this.eventWriter.Write(
					new ProcessEventDto(args.EventType, args.RecordInfo, args.Message, args.Timestamp));
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