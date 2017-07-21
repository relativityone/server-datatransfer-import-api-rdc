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

    using Relativity.Logging;
    using Relativity.Services.ServiceProxy;
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
        /// The native file transfer parameters.
        /// </summary>
        private readonly NativeFileTransferParameters parameters;

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
        /// The list of transfer event listeners.
        /// </summary>
        private List<TransferListenerBase> transferListeners;

        /// <summary>
        /// The disposed backing.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeFileTransfer"/> class.
        /// </summary>
        /// <param name="parameters">
        /// The native file transfer parameters.
        /// </param>
        /// <param name="direction">
        /// The transfer direction.
        /// </param>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="token">
        /// The cancellation token.
        /// </param>
        /// <remarks>
        /// Don't expose TAPI objects to WinEDDS - at least not yet. This is reserved for integration tests.
        /// </remarks>
        internal NativeFileTransfer(
            NativeFileTransferParameters parameters,
            TransferDirection direction,
            ILog log,
            CancellationToken token)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.Credentials == null)
            {
                throw new ArgumentException("The credentials information must be specified.", nameof(parameters));
            }

            if (parameters.WorkspaceId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(parameters),
                    Strings.WorkspaceExceptionMessage);
            }

            if (string.IsNullOrEmpty(parameters.TargetPath))
            {
                throw new ArgumentException("That target path must be specified.", nameof(parameters));
            }

            if (parameters.WebCookieContainer == null)
            {
                parameters.WebCookieContainer = new CookieContainer();
            }

            this.currentDirection = direction;
            this.parameters = parameters;
            this.transferHost = new RelativityTransferHost(
                CreateRelativityConnectionInfo(parameters),
                log);
            this.TargetPath = parameters.TargetPath;
            this.cancellationToken = token;
            this.transferLog = log;
            this.pathManager = new FileSharePathManager(parameters.MaxFilesPerFolder);

            // The context is optional and must be supplied on the transfer request (see below).
            this.context = new TransferContext();

            this.SetupTransferListeners();
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
        /// Occurs when a file finishes transferring.
        /// </summary>
        public event EventHandler<TransferMessageEventArgs> ProgressEvent = delegate { };

        /// <summary>
        /// Occurs when there is a fatal error in the transfer.
        /// </summary>
        public event EventHandler<TransferMessageEventArgs> FatalError = delegate { };

        /// <summary>
        /// Gets a value indicating whether the bulk setting is enabled.
        /// </summary>
        /// <value>
        /// The bulk setting value.
        /// </value>
        public bool IsBulkEnabled => this.parameters.IsBulkEnabled;

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
        /// Gets a value indicating whether there are transfers pending.
        /// </summary>
        public bool TransfersPending => this.transferJob != null && this.transferJob.Paths.Count > 0;

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
                var nextTargetPath = this.parameters.SortIntoVolumes
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
                this.RaiseWarningMessage("An unexpected error has occurred. Message: " + e.Message, 0);
                this.FallbackHttpClient();
            }
            finally
            {
                this.DestroyTransferJob();
            }
        }

        /// <summary>
        /// Prints transfer statistics for each line.
        /// </summary>
        /// <param name="fileCount">
        /// The file count.
        /// </param>
        public void DumpTransferStats(int fileCount)
        {
            Console.WriteLine($"Summary:\nBatch Size: {fileCount}");
            var listener = this.transferListeners.OfType<TransferFileListener>().FirstOrDefault();
            listener?.Dump();
        }

        /// <summary>
        /// Raises a client changed event.
        /// </summary>
        /// <param name="reason">
        /// The reason for the client change.
        /// </param>
        internal void RaiseClientChanged(ClientChangeReason reason)
        {
            string message;
            switch (reason)
            {
                case ClientChangeReason.BestFit:
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Strings.TransferClientChangedBestFitMessage,
                        this.ClientName);
                    break;

                case ClientChangeReason.ForceConfig:
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Strings.TransferClientChangedForceConfigMessage,
                        this.ClientName);
                    break;

                case ClientChangeReason.HttpFallback:
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Strings.TransferClientChangedHttpFallbackMessage,
                        this.ClientName);
                    break;

                default:
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Strings.TransferClientChangedDefaultMessage,
                        this.ClientName);
                    break;
            }

            this.RaiseStatusMessage(message, 0);
            this.ClientChanged.Invoke(this, new TransferClientEventArgs(this.transferClient.Name, this.parameters.IsBulkEnabled));
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
                if (this.parameters.ForceClientId == Guid.Empty)
                {
                    this.transferClient = this.transferHost.CreateClientAsync().GetAwaiter().GetResult();
                    this.RaiseClientChanged(ClientChangeReason.BestFit);
                }
                else if (this.parameters.ForceHttpClient)
                {
                    this.CreateHttpClient();
                    this.RaiseClientChanged(ClientChangeReason.ForceConfig);
                }
                else
                {
                    this.transferClient =
                        this.transferHost.CreateClient(new ClientConfiguration(this.parameters.ForceClientId,
                            this.parameters.ForceClientName));
                    this.RaiseClientChanged(ClientChangeReason.ForceConfig);
                }
            }
            catch (Exception)
            {
                this.CreateHttpClient();
                this.RaiseClientChanged(ClientChangeReason.HttpFallback);
            }
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
            this.jobRequest.MaxRetryAttempts = this.parameters.MaxRetryCount;
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
        /// Creates a Relativity connection information object.
        /// </summary>
        /// <param name="parameters">
        /// The transfer parameters.
        /// </param>
        /// <returns>
        /// The <see cref="RelativityConnectionInfo"/> instance.
        /// </returns>
        private static RelativityConnectionInfo CreateRelativityConnectionInfo(NativeFileTransferParameters parameters)
        {
            var baseUri = new Uri(parameters.WebServiceUrl);
            var host = new Uri(baseUri.GetLeftPart(UriPartial.Authority));
            return new RelativityConnectionInfo(
                host,
                new UsernamePasswordCredentials(parameters.Credentials.UserName, parameters.Credentials.Password),
                parameters.WorkspaceId);
        }

        /// <summary>
        /// Creates the HTTP client.
        /// </summary>
        private void CreateHttpClient()
        {
            this.transferClient =
                this.transferHost.CreateClient(
                    new HttpClientConfiguration { CookieContainer = this.parameters.WebCookieContainer });
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
        /// Setup the transfer listeners.
        /// </summary>
        private void SetupTransferListeners()
        {
            this.transferListeners = new List<TransferListenerBase>();

            this.CreateFileListener();
            this.CreateFileIssueListener();
            this.CreateRequestListener();
            this.CreateJobRetryListener();
            this.CreateStatisticsListener();

            foreach (TransferListenerBase listener in this.transferListeners)
            {
                listener.StatusMessage += (sender, args) => this.StatusMessage.Invoke(sender, args);
                listener.WarningMessage += (sender, args) => this.WarningMessage.Invoke(sender, args);
            }
        }

        /// <summary>
        /// Creates initializes a <inheritdoc cref="TransferFileListener"/> instance.
        /// </summary>
        private void CreateFileListener()
        {
            var listener = new TransferFileListener(this.transferLog, this.context);
            listener.ProgressEvent += (sender, args) => this.ProgressEvent.Invoke(sender, args);
            this.transferListeners.Add(listener);
        }

        /// <summary>
        /// Creates and initializes a <inheritdoc cref="TransferFileIssueListener"/> instance. 
        /// </summary>
        private void CreateFileIssueListener()
        {
            var listener = new TransferFileIssueListener(this.transferLog, this.currentDirection, this.ClientName);
            listener.FatalError += (sender, args) => this.FatalError.Invoke(sender, args);
            this.transferListeners.Add(listener);
        }

        /// <summary>
        /// Creates and initializes a <inheritdoc cref="TransferRequestListener"/> to listen for transfer request events. 
        /// </summary>
        private void CreateRequestListener()
        {
            var listener = new TransferRequestListener(this.transferLog, this.context);
            this.transferListeners.Add(listener);
        }

        /// <summary>
        /// Creates and initializes a <inheritdoc cref="TransferJobRetryListener"/> to listen for job retry events.
        /// </summary>
        private void CreateJobRetryListener()
        {
            var listener = new TransferJobRetryListener(this.transferLog, this.parameters.MaxRetryCount, this.context);
            this.transferListeners.Add(listener);
        }

        /// <summary>
        /// Creates and initializes a <inheritdoc cref="TransferStatisticsListener"/> to listen for transfer statistic events. 
        /// </summary>
        private void CreateStatisticsListener()
        {
            var listener = new TransferStatisticsListener(this.transferLog, this.context);
            this.transferListeners.Add(listener);
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
                this.DestroyTransferJob();
                this.DestroyTransferClient();
                this.DestroyTransferHost();
                this.DestroyTransferListeners();
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
        /// Destroys transfer listeners.
        /// </summary>
        private void DestroyTransferListeners()
        {
            if (this.transferListeners == null)
            {
                return;
            }

            foreach (TransferListenerBase listener in this.transferListeners)
            {
                listener.Dispose();
            }

            this.transferListeners.Clear();
            this.transferListeners = null;
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