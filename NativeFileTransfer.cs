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
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System;
    using System.Globalization;
    using System.Threading;

    using kCura.Utility;

    using Relativity.Logging;
    using Relativity.Transfer;
    using Relativity.Transfer.Aspera;
    using Relativity.Transfer.Http;

    using Strings = kCura.WinEDDS.TApi.Resources.Strings;

    /// <summary>
    /// Represents a class object to support native file transfers.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
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
        /// The cookie container.
        /// </summary>
        private readonly CookieContainer cookieContainer;

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
        /// <param name="container">
        /// The cookie container.
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
            CookieContainer container,
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

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
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
            this.cookieContainer = container;
            this.isBulkEnabled = isBulkEnabled;
            this.cancellationToken = token;
            this.transferLog = log;
            this.pathManager = new FileSharePathManager(int.MaxValue - 1);

            // This should always default to true (BCP specific!).
            this.SortIntoVolumes = true;

            // The context is optional and must be supplied on the transfer request (see below).
            this.context = new TransferContext();
            this.context.TransferFile += this.ContextOnTransferFile;
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
        /// Gets a value indicating whether the bulk setting is enabled.
        /// </summary>
        /// <value>
        /// The bulk setting value.
        /// </value>
        public bool IsBulkEnabled => this.isBulkEnabled;

        /// <summary>
        /// Gets the current transfer client unique identifier.
        /// </summary>
        /// <value>
        /// The <see cref="Guid"/> value.
        /// </value>
        public Guid ClientId => this.transferClient?.Id ?? Guid.Empty;

        /// <summary>
        /// Gets the current transfer client name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string ClientName => this.transferClient != null ? this.transferClient.Name : Strings.ClientNotSet;

        /// <summary>
        /// Gets the current transfer client.
        /// </summary>
        /// <value>
        /// The <see cref="TransferClient"/> value.
        /// </value>
        public TransferClient Client
        {
            get
            {
                // Note: for backwards compatibility only.
                switch (this.ClientId.ToString())
                {
                    case TransferClientConstants.AsperaClientId:
                        return TransferClient.Aspera;

                    case TransferClientConstants.FileShareClientId:
                        return TransferClient.Direct;

                    case TransferClientConstants.HttpClientId:
                        return TransferClient.Web;

                    default:
                       return TransferClient.None;                    
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force using the HTTP client.
        /// </summary>
        /// <value>
        /// <c>true</c> to force the HTTP client; otherwise, <c>false</c>.
        /// </value>
        public bool ForceHttpClient
        {
            get;
            set;
        }

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
        /// Gets or sets a value indicating whether to sort all transfers into a volumes folder.
        /// </summary>
        /// <value>
        /// <see langword="true" /> to sort all transfers into a volumes folder; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>
        /// This is always <see langword="true" /> unless transferring BCP files.
        /// </remarks>
        public bool SortIntoVolumes
        {
            get;
            set;
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
        /// <param name="order">
        /// The order the path is added to the transfer job.
        /// </param>
        /// <returns>
        /// The file name.
        /// </returns>
        public string AddPath(string sourceFile, string targetFileName, int order)
        {
            this.CreateTransferJob(false);
            if (this.transferJob == null)
            {
                throw new InvalidOperationException(Strings.TransferJobNullExceptionMessage);
            }

            try
            {
                var nextTargetPath = this.SortIntoVolumes
                    ? this.pathManager.GetNextTargetPath(this.TargetPath)
                    : this.TargetPath;
                var transferPath = new TransferPath
                {
                    SourcePath = sourceFile,
                    TargetPath = nextTargetPath,
                    TargetFileName = targetFileName,
                    Order = order
                };
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
                this.transferLog.LogError(e, "Failed to add a path to the transfer job.");
                this.FallbackHttpClient();
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
                Console.WriteLine("{0} transfer elapsed time: {1}", this.ClientName, transferResult.Elapsed);
                if (transferResult.Status == TransferStatus.Success ||
                    transferResult.Status == TransferStatus.PartialSuccess ||
                    transferResult.Status == TransferStatus.Canceled)
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
            catch (Exception e)
            {
                this.transferLog.LogInformation(e, "An unexpected error has occurred attempting to wait for the transfer job to complete.");
                this.RaiseWarningMessage("An unexpected error has occurred. Message: " + e.Message, -1);
                this.FallbackHttpClient();
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

            try
            {
                if (!this.ForceHttpClient)
                {
                    this.transferClient = this.transferHost.CreateClientAsync().GetAwaiter().GetResult();
                }
                else
                {
                    this.CreateHttpClient();
                }
            }
            catch (Exception)
            {
                this.CreateHttpClient();
            }

            this.RaiseClientChanged();
        }

        /// <summary>
        /// Creates a new transfer job.
        /// </summary>
        /// <param name="httpFallback">
        /// Specify whether this method is setting up the HTTP fallback client.
        /// </param>
        protected void CreateTransferJob(bool httpFallback)
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
            this.SetupTargetPathResolvers();
            
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
                this.transferLog.LogError(e, "Failed to create the transfer job.");
                if (httpFallback)
                {
                    throw;
                }

                this.FallbackHttpClient();
            }
        }

        /// <summary>
        /// Raises a status message event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        protected void RaiseStatusMessage(string message, int lineNumber)
        {
            this.StatusMessage.Invoke(this, new TransferMessageEventArgs(message, lineNumber));
        }

        /// <summary>
        /// Raises a warning message event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        protected void RaiseWarningMessage(string message, int lineNumber)
        {
            this.WarningMessage.Invoke(this, new TransferMessageEventArgs(message, lineNumber));
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
            this.RaiseStatusMessage(progressMessage, 0);
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
                    this.RaiseStatusMessage(Strings.TransferJobStartedMessage, 0);
                    break;

                case TransferRequestStatus.Ended:
                    this.RaiseStatusMessage(Strings.TransferJobEndedMessage, 0);
                    break;

                case TransferRequestStatus.EndedMaxRetry:
                    this.RaiseStatusMessage(Strings.TransferJobEndedMaxRetryMessage, 0);
                    break;

                case TransferRequestStatus.Canceled:
                    this.RaiseStatusMessage(Strings.TransferJobCanceledMessage, 0);
                    break;
            }
        }

        /// <summary>
        /// Occurs when a transfer file event occurs.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferFileEventArgs"/> instance containing the event data.
        /// </param>
        private void ContextOnTransferFile(object sender, TransferFileEventArgs e)
        {
            switch (e.Status)
            {
                case TransferFileStatus.Failed:
                    this.RaiseStatusMessage($"Failed to transfer file. Path={e.Path.SourcePath}.", e.Path.Order);
                    break;

                case TransferFileStatus.FailedRetryable:
                    this.RaiseStatusMessage($"Failed to transfer file. Path={e.Path.SourcePath} and will re-queue after the current job is complete.", e.Path.Order);
                    break;

                case TransferFileStatus.Started:
                    this.RaiseStatusMessage($"Starting file transfer. Path={e.Path.SourcePath}.", e.Path.Order);
                    break;

                case TransferFileStatus.Successful:
                    this.RaiseStatusMessage($"Successfully transferred file. Path={e.Path.SourcePath}.", e.Path.Order);
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
            this.RaiseWarningMessage(message, e.Issue.Path != null ? e.Issue.Path.Order : -1);
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
            this.RaiseStatusMessage(message, 0);
        }

        /// <summary>
        /// Creates the HTTP client.
        /// </summary>
        private void CreateHttpClient()
        {
            this.transferClient =
                this.transferHost.CreateClient(
                    new HttpClientConfiguration { CookieContainer = this.cookieContainer });
        }

        /// <summary>
        /// Setup a new HTTP client and potentially re-queue all paths that previously failed.
        /// </summary>
        private void FallbackHttpClient()
        {
            // If we're already using the HTTP client, it's hopeless.
            if (this.transferClient.Id == new Guid(TransferClientConstants.HttpClientId))
            {
                throw new TransferException(Strings.HttpFallbackExceptionMessage);
            }

            this.transferLog.LogInformation("Preparing to fallback the transfer client to HTTP.");
            var requeuedPaths = new List<TransferPath>();
            if (this.transferJob != null)
            {
                requeuedPaths.AddRange(this.transferJob.ReadAllJobPaths()
                    .Where(x => x.Status != TransferFileStatus.Successful)
                    .Select(jobPath => jobPath.Path));
            }

            this.DestroyTransferJob();
            this.DestroyTransferClient();
            this.CreateHttpClient();
            this.CreateTransferJob(true);
            foreach (var path in requeuedPaths)
            {
                this.AddPath(path.SourcePath, path.TargetFileName, path.Order);
            }

            this.transferLog.LogInformation("Successfully switched the transfer client to HTTP.");
        }

        /// <summary>
        /// Setup the target path resolvers.
        /// </summary>
        private void SetupTargetPathResolvers()
        {
            switch (this.ClientId.ToString().ToUpperInvariant())
            {
                case TransferClientConstants.AsperaClientId:
                    if (this.currentDirection == TransferDirection.Upload)
                    {
                        this.jobRequest.TargetPathResolver = new UncNativeFilePathResolver();
                    }
                    else
                    {
                        this.jobRequest.SourcePathResolver = new UncNativeFilePathResolver();
                    }

                    break;
            }
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
                    this.context.TransferFile -= this.ContextOnTransferFile;
                    this.context.TransferFileIssue -= this.ContextOnTransferFileIssue;
                    this.context.TransferJobRetry -= this.ContextOnTransferJobRetry;
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