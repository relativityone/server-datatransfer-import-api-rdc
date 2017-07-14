// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeFileTransfer.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Defines the core file transfer class object to support native files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Globalization;
    using System.Threading;

    using kCura.Utility;

    using Relativity.Logging;
    using Relativity.Transfer;

    using Strings = kCura.WinEDDS.TApi.Resources.Strings;

    /// <summary>
    /// Represents a class object to support native file transfers.
    /// </summary>
    public class NativeFileTransfer : IDisposable
    {
        /// <summary>
        /// The manager used to limit the maximum number of files per folder.
        /// </summary>
        private readonly FileSharePathManager pathManager;

        /// <summary>
        /// The cancellation token source.
        /// </summary>
        private readonly CancellationToken cancellationToken;

        /// <summary>
        /// The context used for transfer events.
        /// </summary>
        private readonly TransferContext context;

        /// <summary>
        /// The is bulk enabled flag.
        /// </summary>
        private readonly bool isBulkEnabled;

        /// <summary>
        /// The Relativity transfer log.
        /// </summary>
        private readonly ILog transferLog;

        /// <summary>
        /// The file system service used to wrap up all IO API's.
        /// </summary>
        private readonly IFileSystemService fileSystemService = new FileSystemService();

        /// <summary>
        /// The current transfer direction.
        /// </summary>
        private readonly TransferDirection currentDirection;

        /// <summary>
        /// The client request unique identifier used to tie all jobs to a single request.
        /// </summary>
        private Guid? clientRequestId;

        /// <summary>
        /// The transfer unique identifier associated with the current job.
        /// </summary>
        private Guid? currentTransferId;


        /// <summary>
        /// The current job request.
        /// </summary>
        private ITransferRequest jobRequest;

        /// <summary>
        /// The Relativity transfer host.
        /// </summary>
        private IRelativityTransferHost transferHost;

        /// <summary>
        /// The transfer client.
        /// </summary>
        private ITransferClient transferClient;

        /// <summary>
        /// The transfer job.
        /// </summary>
        private ITransferJob transferJob;

        /// <summary>
        /// The disposed backing.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeFileTransfer"/> class.
        /// </summary>
        /// <param name="connectionInfo">
        /// The Relativity connection information.
        /// </param>
        /// <param name="workspaceId">
        /// The workspace artifact identifier.
        /// </param>
        /// <param name="targetPath">
        /// The target path.
        /// </param>
        /// <param name="isBulkEnabled">
        /// Specify whether the bulk feature is enabled.
        /// </param>
        /// <param name="direction">
        /// Specify the desired transfer direction for all created jobs.
        /// </param>
        /// <param name="token">
        /// The cancellation token.
        /// </param>
        /// <param name="log">
        /// The Relativity transfer log.  
        /// </param>
        /// <remarks>
        /// Don't expose TAPI objects to WinEDDS - at least not yet. This is reserved for integration tests.
        /// </remarks>
        internal NativeFileTransfer(
            RelativityConnectionInfo connectionInfo,
            int workspaceId,
            string targetPath,
            bool isBulkEnabled,
            TransferDirection direction,
            CancellationToken token,
            ILog log)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException(nameof(connectionInfo));
            }

            if (workspaceId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(workspaceId), Strings.WorkspaceExceptionMessage);
            }

            if (string.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentNullException(nameof(targetPath));
            }

            if (log == null)
            {
                log = new NullLogger();
            }

            this.currentDirection = direction;
            this.transferHost = new RelativityTransferHost(connectionInfo, log);
            this.MaxRetryCount = Config.IOErrorNumberOfRetries;
            this.WorkspaceId = workspaceId;
            this.TargetPath = targetPath;
            this.isBulkEnabled = isBulkEnabled;
            this.cancellationToken = token;
            this.transferLog = log;
            this.pathManager = new FileSharePathManager(int.MaxValue - 1);

            // The context is optional and must be supplied on the transfer request (see below).
            this.context = new TransferContext();
            this.context.TransferFileIssue += this.ContextOnTransferFileIssue;
            this.context.TransferJobRetry += this.ContextOnTransferJobRetry;
            this.context.TransferRequest += this.ContextOnTransferRequest;
            this.context.TransferStatistics += this.ContextOnTransferStatistics;
        }

        /// <summary>
        /// Occurs when a status message is available.
        /// </summary>
        public event EventHandler<TransferMessageEventArgs> StatusMessage = delegate { };

        /// <summary>
        /// Occurs when a warning message is available.
        /// </summary>
        public event EventHandler<TransferMessageEventArgs> WarningMessage = delegate { };

        /// <summary>
        /// Occurs when the transfer client is changed.
        /// </summary>
        public event EventHandler<TransferClientEventArgs> ClientChanged = delegate { };

        /// <summary>
        /// Gets the current client identifier.
        /// </summary>
        /// <value>
        /// The <see cref="Guid"/> value.
        /// </value>
        public Guid ClientId => this.transferClient?.Id ?? Guid.Empty;

        /// <summary>
        /// Gets the current client name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string ClientName => this.transferClient != null ? this.transferClient.Name : string.Empty;

        /// <summary>
        /// Gets the max retry count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int MaxRetryCount
        {
            get;
        }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string TargetPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target folder name.
        /// </summary>
        /// <value>
        /// The folder name.
        /// </value>
        public string TargetFolderName => this.pathManager.CurrentTargetFolderName;

        /// <summary>
        /// Gets or sets the current transfer client plugin.
        /// </summary>
        /// <value>
        /// The <see cref="TransferClientPlugin"/> value.
        /// </value>
        public TransferClientPlugin Plugin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the wait time between retry attempts.
        /// </summary>
        /// <value>
        /// The wait time between retry attempts.
        /// </value>
        public int WaitTimeBetweenRetryAttempts => this.transferJob == null
                                                       ? Config.IOErrorWaitTimeInSeconds
                                                       : Convert.ToInt32(this.transferJob.RetryWaitPeriod.TotalSeconds);

        /// <summary>
        /// Gets the workspace artifact identifier.
        /// </summary>
        /// <value>
        /// The artifact identifier.
        /// </value>
        public int WorkspaceId
        {
            get;
        }

        /// <summary>
        /// Adds the path to a transfer job.
        /// </summary>
        /// <param name="sourceFile">
        /// The full path to the source file.
        /// </param>
        /// <param name="targetFileName">
        /// The optional target filename.
        /// </param>
        /// <returns>
        /// The file name.
        /// </returns>
        public string AddPath(string sourceFile, string targetFileName)
        {
            this.CreateTransferJob();
            if (this.transferJob == null)
            {
                throw new InvalidOperationException(Strings.TransferJobNullExceptionMessage);
            }

            // TODO: Need to dynamically update this after the job has been created.
            //// this.jobRequest.TargetPath = this.pathManager.GetNextTargetPath(this.TargetFolderName);

            try
            {
                var transferPath = new TransferPath { SourcePath = sourceFile, TargetFileName = targetFileName };
                this.transferJob.AddPath(transferPath);
                return !string.IsNullOrEmpty(targetFileName)
                           ? targetFileName
                           : this.fileSystemService.GetFileName(sourceFile);
            }
            catch (OperationCanceledException)
            {
                this.LogCancelRequest();
                return string.Empty;
            }
            catch (TransferException e)
            {
                // TODO: Decide which exception to rethrow.
                this.transferLog.LogError(e, "Failed to add a path to the transfer job.");
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Waits for the transfer job to complete all pending transfers in the queue.
        /// </summary>
        public void WaitForTransferJob()
        {
            if (this.transferJob == null)
            {
                throw new InvalidOperationException(Strings.TransferJobNullExceptionMessage);
            }

            try
            {
                // Note: retry is already built into TAPI.
                var taskResult = this.transferJob.CompleteAsync(this.cancellationToken);
                var transferResult = taskResult.GetAwaiter().GetResult();
                if (transferResult.Status != TransferStatus.Failed && transferResult.Status != TransferStatus.Canceled)
                {
                    return;
                }

                var errorMessage = Strings.TransferJobExceptionMessage;
                var error = transferResult.TransferError;
                if (!string.IsNullOrEmpty(error?.Message))
                {
                    errorMessage = error.Message;
                }

                throw new InvalidOperationException(errorMessage);
            }
            catch (OperationCanceledException)
            {
                this.LogCancelRequest();
            }
            catch (Exception)
            {
                // TODO: When this fails, use the next best client.
            }
            finally
            {
                this.DestroyTransferJob();
            }
        }

        /// <summary>
        /// Creates the best transfer client.
        /// </summary>        
        protected void CreateTransferClient()
        {
            if (this.transferClient != null)
            {
                return;
            }

            this.transferClient = this.transferHost.CreateClientAsync().GetAwaiter().GetResult();
            this.RaiseClientChanged();
        }

        /// <summary>
        /// Creates a new transfer job.
        /// </summary>
        protected void CreateTransferJob()
        {
            if (this.transferJob != null)
            {
                return;
            }

            this.CreateTransferClient();
            if (this.clientRequestId == null)
            {
                this.clientRequestId = Guid.NewGuid();
            }

            this.currentTransferId = Guid.NewGuid();
            this.jobRequest = this.currentDirection == TransferDirection.Upload
                ? TransferRequest.ForUploadJob(this.TargetPath, this.context)
                : TransferRequest.ForDownloadJob(this.TargetPath, this.context);
            this.jobRequest.MaxRetryAttempts = this.MaxRetryCount;
            this.jobRequest.ClientRequestId = this.clientRequestId;
            this.jobRequest.TransferId = this.currentTransferId;

            try
            {
                var task = this.transferClient.CreateJobAsync(this.jobRequest, this.cancellationToken);
                this.transferJob = task.GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                this.LogCancelRequest();
                this.DestroyTransferJob();
            }
            catch (Exception e)
            {
                // TODO: When this fails, use the next best client.
                Console.WriteLine("Failed to create job. Error: " + e);
            }
        }

        /// <summary>
        /// Raises a status message event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        protected void RaiseStatusMessage(string message)
        {
            this.StatusMessage.Invoke(this, new TransferMessageEventArgs(message));
        }

        /// <summary>
        /// Raises a warning message event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        protected void RaiseWarningMessage(string message)
        {
            this.WarningMessage.Invoke(this, new TransferMessageEventArgs(message));
        }

        /// <summary>
        /// Raises a client changed event.
        /// </summary>
        protected void RaiseClientChanged()
        {
            this.ClientChanged.Invoke(this, new TransferClientEventArgs(this.transferClient.Name, this.isBulkEnabled));
        }

        /// <summary>
        /// Occurs when transfer statistics are available.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferStatisticsEventArgs"/> instance containing the event data.
        /// </param>
        private void ContextOnTransferStatistics(object sender, TransferStatisticsEventArgs e)
        {
            var progressMessage = string.Format(
                CultureInfo.CurrentCulture,
                Strings.ProgressMessage,
                e.Statistics.TotalTransferredFiles,
                e.Statistics.TotalFiles,
                e.Statistics.Progress);
            this.RaiseStatusMessage(progressMessage);
        }

        /// <summary>
        /// Occurs when transfer requests start and end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferRequestEventArgs"/> instance containing the event data.
        /// </param>
        private void ContextOnTransferRequest(object sender, TransferRequestEventArgs e)
        {
            switch (e.Status)
            {
                case TransferRequestStatus.Started:
                    this.RaiseStatusMessage(Strings.TransferJobStartedMessage);
                    break;

                case TransferRequestStatus.Ended:
                    this.RaiseStatusMessage(Strings.TransferJobEndedMessage);
                    break;

                case TransferRequestStatus.EndedMaxRetry:
                    this.RaiseStatusMessage(Strings.TransferJobEndedMaxRetryMessage);
                    break;

                case TransferRequestStatus.Canceled:
                    this.RaiseStatusMessage(Strings.TransferJobCanceledMessage);
                    break;
            }
        }

        /// <summary>
        /// Occurs when transfer file issues occur.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferFileIssueEventArgs"/> instance containing the event data.
        /// </param>
        private void ContextOnTransferFileIssue(object sender, TransferFileIssueEventArgs e)
        {
            var message = string.Format(CultureInfo.CurrentCulture,
                this.currentDirection == TransferDirection.Download
                    ? Strings.TransferFileDownloadIssueMessage
                    : Strings.TransferFileUploadIssueMessage, this.ClientName, e.Issue.Message);
            this.RaiseWarningMessage(message);
            if (e.Issue.Attributes.HasFlag(IssueAttributes.Error))
            {
                this.transferLog.LogError("A serious transfer error has occurred. Issue={Issue}.", e.Issue);
            }
            else if (e.Issue.Attributes.HasFlag(IssueAttributes.Warning))
            {
                this.transferLog.LogWarning("A transfer warning has occurred. Issue={Issue}.", e.Issue);
            }
        }

        /// <summary>
        /// Occurs when the transfer job is retried.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferJobRetryEventArgs"/> instance containing the event data.
        /// </param>
        private void ContextOnTransferJobRetry(object sender, TransferJobRetryEventArgs e)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                Strings.RetryJobMessage,
                e.Count,
                this.MaxRetryCount);
            this.RaiseStatusMessage(message);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                try
                {
                    this.DestroyTransferJob();
                    this.DestroyTransferClient();
                    this.DestroyTransferHost();
                }
                finally
                {
                    this.context.TransferJobRetry -= this.ContextOnTransferJobRetry;
                    this.context.TransferFileIssue -= this.ContextOnTransferFileIssue;
                    this.context.TransferRequest -= this.ContextOnTransferRequest;
                    this.context.TransferStatistics -= this.ContextOnTransferStatistics;
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// Destroys the transfer job.
        /// </summary>
        private void DestroyTransferJob()
        {
            this.currentTransferId = null;
            if (this.transferJob == null)
            {
                return;
            }

            this.transferJob.Dispose();
            this.transferJob = null;
        }

        /// <summary>
        /// Destroys the transfer client.
        /// </summary>
        private void DestroyTransferClient()
        {
            if (this.transferClient == null)
            {
                return;
            }

            this.transferClient.Dispose();
            this.transferClient = null;
        }

        /// <summary>
        /// Destroys the transfer host.
        /// </summary>
        private void DestroyTransferHost()
        {
            if (this.transferHost == null)
            {
                return;
            }

            this.transferHost.Dispose();
            this.transferHost = null;
        }

        /// <summary>
        /// Logs a cancellation request.
        /// </summary>
        private void LogCancelRequest()
        {
            this.transferLog.LogInformation("The file transfer has been cancelled. ClientId={ClientId}, TransferId={TransferId} ",
                this.clientRequestId, this.currentTransferId);
        }
    }
}