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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
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
        /// The list of transfer event listeners.
        /// </summary>
        private readonly List<TransferListenerBase> transferListeners = new List<TransferListenerBase>();

        /// <summary>
        /// The current transfer direction.
        /// </summary>
        private readonly TransferDirection currentDirection;

        /// <summary>
        /// The client request unique identifier used to tie all jobs to a single request.
        /// </summary>
        private Guid? clientRequestId;

        /// <summary>
        /// The job unique identifier associated with the current job.
        /// </summary>
        private Guid? currentJobId;

        /// <summary>
        /// The current job number. This is tagged to the request to provide a non-zero line number.
        /// </summary>
        private int currentJobNumber;

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
                throw new ArgumentOutOfRangeException(nameof(parameters), Strings.WorkspaceExceptionMessage);
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
            var connectionInfo = TapiWinEddsHelper.CreateRelativityConnectionInfo(
                parameters.WebServiceUrl,
                parameters.WorkspaceId,
                parameters.Credentials.UserName,
                parameters.Credentials.Password);
            this.transferHost = new RelativityTransferHost(connectionInfo, log);
            this.TargetPath = parameters.TargetPath;
            this.cancellationToken = token;
            this.transferLog = log;
            this.pathManager = new FileSharePathManager(parameters.MaxFilesPerFolder);
            this.currentJobNumber = 0;
            this.context = new TransferContext
            {
                StatisticsRateSeconds = 1.0,
                LargeFileProgressEnabled = parameters.LargeFileProgressEnabled
            };

            this.SetupTransferListeners();
        }

        /// <summary>
        /// Occurs when a status message is available.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> TapiStatusMessage = delegate { };

        /// <summary>
        /// Occurs when a non-fatal error message is available.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> TapiErrorMessage = delegate { };

        /// <summary>
        /// Occurs when a warning message is available.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> TapiWarningMessage = delegate { };

        /// <summary>
        /// Occurs when the transfer client is changed.
        /// </summary>
        public event EventHandler<TapiClientEventArgs> TapiClientChanged = delegate { };

        /// <summary>
        /// Occurs when a path finishes transferring.
        /// </summary>
        public event EventHandler<TapiProgressEventArgs> TapiProgress = delegate { };

        /// <summary>
        /// Occurs when transfer statistics are available.
        /// </summary>
        public event EventHandler<TapiStatisticsEventArgs> TapiStatistics = delegate { };

        /// <summary>
        /// Occurs when there is a fatal error in the transfer.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> TapiFatalError = delegate { };

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
        /// Gets the current transfer client display name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string ClientDisplayName => this.transferClient?.DisplayName ?? Strings.ClientInitializing;

        /// <summary>
        /// Gets a value indicating whether there are transfers pending.
        /// </summary>
        /// <remarks>
        /// Be careful here. The PathCount property was added to avoid costly hits to the repository.
        /// </remarks>
        public bool TransfersPending => this.transferJob != null && this.transferJob.PathCount > 0;

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
            this.CheckDispose();
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
            catch (ArgumentException e)
            {
                // Note: this exception is only thrown when ValidateSourcePaths is true.
                this.transferLog.LogWarning(e, "There was a problem adding the '{SourceFile}' source file to the transfer job.", sourceFile);
                throw new FileNotFoundException(e.Message, sourceFile);
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
            this.CheckDispose();
            if (this.transferJob == null)
            {
                throw new InvalidOperationException(Strings.TransferJobNullExceptionMessage);
            }

            const int ValidLineNumber = 1;

            try
            {
                var taskResult = this.transferJob.CompleteAsync(this.cancellationToken);
                var transferResult = taskResult.GetAwaiter().GetResult();
                this.transferLog.LogInformation(
                    "{Name} transfer status: {Status}, elapsed time: {Elapsed}, data rate: {TransferRate:0.00} Mbps",
                    this.ClientDisplayName,
                    transferResult.Status,
                    transferResult.Elapsed,
                    transferResult.TransferRateMbps);
                this.transferLog.LogInformation(
                    "{Name} total transferred files: {TotalTransferredFiles}, total failed files: {TotalFailedFiles}",
                    this.ClientDisplayName,
                    transferResult.TotalTransferredFiles,
                    transferResult.TotalFailedFiles);
                switch (transferResult.Status)
                {
                    case TransferStatus.Fatal:

                        // Note: Fatal status is non-retryable and normally indicative of issues with data or permissions.
                        var lastIssue = transferResult.Issues.OrderBy(x => x.Index).ToList().FindLast(x => x.Path != null) ??
                                        transferResult.TransferError;
                        if (lastIssue != null && lastIssue.Path != null)
                        {
                            var formattedMessage = transferResult.Request.Direction == TransferDirection.Download
                                ? Strings.TransferFileDownloadFatalMessage
                                : Strings.TransferFileUploadFatalMessage;
                            var message = string.Format(CultureInfo.CurrentCulture, formattedMessage, lastIssue.Message);
                            var lineNumber = lastIssue.Path.Order > 0 ? lastIssue.Path.Order : ValidLineNumber;
                            this.RaiseStatusMessage(message, lineNumber);
                            this.RaiseFatalError(message, lineNumber);
                        }

                        break;
                    case TransferStatus.Failed:

                        // Note: Failed status indicates a problem with the transport.
                        throw new TransferException(Strings.TransferJobExceptionMessage);
                }
            }
            catch (OperationCanceledException)
            {
                this.LogCancelRequest();
            }
            catch (Exception e)
            {
                this.transferLog.LogWarning2(e, this.jobRequest, Strings.CompleteJobExceptionMessage);
                this.RaiseWarningMessage(Strings.CompleteJobExceptionMessage + " Message: " + e.Message, 0);
                this.FallbackHttpClient();
            }
            finally
            {
                this.DestroyTransferJob();
            }
        }

        /// <summary>
        /// Raises a client changed event.
        /// </summary>
        /// <param name="reason">
        /// The reason for the client change.
        /// </param>
        internal void RaiseClientChanged(ClientChangeReason reason)
        {
            this.CheckDispose();
            string message;
            switch (reason)
            {
                case ClientChangeReason.BestFit:
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Strings.TransferClientChangedBestFitMessage,
                        this.ClientDisplayName);
                    break;

                case ClientChangeReason.ForceConfig:
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Strings.TransferClientChangedForceConfigMessage,
                        this.ClientDisplayName);
                    break;

                case ClientChangeReason.HttpFallback:
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Strings.TransferClientChangedHttpFallbackMessage,
                        this.ClientDisplayName);
                    break;

                default:
                    message = string.Format(
                        CultureInfo.CurrentCulture,
                        Strings.TransferClientChangedDefaultMessage,
                        this.ClientDisplayName);
                    break;
            }

            this.RaiseStatusMessage(message, TapiConstants.NoLineNumber);
            var eventArgs = new TapiClientEventArgs(this.ClientDisplayName, this.Client, this.parameters.IsBulkEnabled);
            this.TapiClientChanged.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Creates the best transfer client.
        /// </summary>        
        protected void CreateTransferClient()
        {
            this.CheckDispose();
            if (this.transferClient != null)
            {
                return;
            }

            // Due to FileNotFoundException expectations, do NOT enable this setting.
            const bool ValidateSourcePaths = false;

            // Intentionally limiting resiliency here; otherwise, status messages don't make it to IAPI.
            const int MaxHttpRetryAttempts = 1;
            var configuration =
                new ClientConfiguration
                    {
                        CookieContainer = this.parameters.WebCookieContainer,
                        MaxJobParallelism = this.parameters.MaxJobParallelism,
                        MaxJobRetryAttempts = this.parameters.MaxJobRetryAttempts,
                        MaxHttpRetryAttempts = MaxHttpRetryAttempts,
                        PreCalculateJobSize = false,
                        PreserveDates = false,
                        TimeoutSeconds = this.parameters.TimeoutSeconds,
                        ValidateSourcePaths = ValidateSourcePaths
                };

            try
            {
                var clientId = Guid.Empty;
                if (this.parameters.ForceAsperaClient)
                {
                    clientId = new Guid(TransferClientConstants.AsperaClientId);
                }
                else if (this.parameters.ForceHttpClient)
                {
                    clientId = new Guid(TransferClientConstants.HttpClientId);
                }
                else if (this.parameters.ForceFileShareClient)
                {
                    clientId = new Guid(TransferClientConstants.FileShareClientId);
                }

                if (clientId != Guid.Empty)
                {
                    configuration.ClientId = clientId;
                    this.CreateClient(configuration);
                    this.RaiseClientChanged(ClientChangeReason.ForceConfig);
                }
                else
                {
                    configuration.ClientId = Guid.Empty;
                    this.transferClient = this.transferHost
                        .CreateClientAsync(configuration, this.cancellationToken)
                        .GetAwaiter()
                        .GetResult();
                    this.RaiseClientChanged(ClientChangeReason.BestFit);
                }
            }
            catch (Exception e)
            {
                this.transferLog.LogError(e, "The transfer client construction failed.");
                configuration.ClientId = new Guid(TransferClientConstants.HttpClientId);
                this.CreateClient(configuration);
                this.RaiseClientChanged(ClientChangeReason.HttpFallback);
            }
            finally
            {
                this.OptimizeClient();
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
            this.CheckDispose();
            if (this.transferJob != null)
            {
                return;
            }

            this.transferLog.LogInformation("Create job started...");
            this.CreateTransferClient();
            if (this.clientRequestId == null)
            {
                this.clientRequestId = Guid.NewGuid();
            }

            this.currentJobNumber++;
            this.currentJobId = Guid.NewGuid();
            this.jobRequest = this.currentDirection == TransferDirection.Upload
                                  ? TransferRequest.ForUploadJob(this.TargetPath, this.context)
                                  : TransferRequest.ForDownloadJob(this.TargetPath, this.context);
            this.jobRequest.ClientRequestId = this.clientRequestId;
            this.jobRequest.JobId = this.currentJobId;
            this.jobRequest.Tag = this.currentJobNumber;

            // Note: avoid exponential backoff since that number will be excessive given the default max retry period.
            this.jobRequest.RetryStrategy =
                RetryStrategies.CreateFixedTimeStrategy(this.parameters.WaitTimeBetweenRetryAttempts);
            this.SetupTargetPathResolvers();

            try
            {
                var task = this.transferClient.CreateJobAsync(this.jobRequest, this.cancellationToken);
                this.transferJob = task.GetAwaiter().GetResult();
                this.transferLog.LogInformation("Create job ended.");
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
                    // Nothing more can be done.
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
        protected void RaiseStatusMessage(string message)
        {
            this.RaiseStatusMessage(message, TapiConstants.NoLineNumber);
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
            this.CheckDispose();
            this.TapiStatusMessage.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
        }

        /// <summary>
        /// Raises a warning message event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        protected void RaiseWarningMessage(string message)
        {
            this.CheckDispose();
            this.TapiWarningMessage.Invoke(this, new TapiMessageEventArgs(message, TapiConstants.NoLineNumber));
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
            this.CheckDispose();
            this.TapiWarningMessage.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
        }

        /// <summary>
        /// Raises a fatal error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <remarks>
        /// A non-zero line number MUST be supplied.
        /// </remarks>
        protected void RaiseFatalError(string message)
        {
            this.CheckDispose();
            this.RaiseFatalError(message, TapiConstants.NoLineNumber);
        }

        /// <summary>
        /// Raises a fatal error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        protected void RaiseFatalError(string message, int lineNumber)
        {
            this.CheckDispose();
            this.TapiFatalError.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
        }

        /// <summary>
        /// Checks to see whether this instance has been disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when this instance has been disposed.
        /// </exception>
        protected void CheckDispose()
        {
            if (!this.disposed)
            {
                return;
            }

            throw new ObjectDisposedException(Strings.ObjectDisposedExceptionMessage);
        }

        /// <summary>
        /// Apply optimizations to the client.
        /// </summary>
        private void OptimizeClient()
        {
            if (this.transferClient == null)
            {
                return;
            }

            // Tune the job retry to account for different kinds of jobs.
            switch (this.transferClient.Id.ToString().ToUpperInvariant())
            {
                case TransferClientConstants.FileShareClientId:
                    this.transferClient.Configuration.PreserveDates = false;
                    break;

                case TransferClientConstants.HttpClientId:
                    this.transferClient.Configuration.MaxJobParallelism = 1;
                    this.transferClient.Configuration.PreserveDates = false;
                    break;

                case TransferClientConstants.AsperaClientId:
                    this.transferClient.Configuration.MaxJobParallelism = 1;
                    this.transferClient.Configuration.PreserveDates = false;
                    break;
            }
        }

        /// <summary>
        /// Creates the HTTP client.
        /// </summary>
        private void CreateHttpClient()
        {
            this.CreateClient(
                new HttpClientConfiguration { CookieContainer = this.parameters.WebCookieContainer });
        }

        /// <summary>
        /// Creates the client using only the specified configuration object.
        /// </summary>
        /// <param name="configuration">
        /// The transfer client configuration.
        /// </param>
        private void CreateClient(ClientConfiguration configuration)
        {
            this.transferHost.Clear();
            this.transferClient = this.transferHost.CreateClient(configuration);
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
                    .Where(x => x.Status != TransferPathStatus.Successful)
                    .Select(jobPath => jobPath.Path));
            }

            this.DestroyTransferJob();
            this.DestroyTransferClient();
            this.CreateHttpClient();
            this.RaiseClientChanged(ClientChangeReason.HttpFallback);
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
            this.CreatePathProgressListener();
            this.CreatePathIssueListener();
            this.CreateRequestListener();
            this.CreateJobRetryListener();
            this.CreateStatisticsListener();
            foreach (var listener in this.transferListeners)
            {
                listener.ErrorMessage += (sender, args) =>
                    {
                        this.TapiErrorMessage.Invoke(sender, args);
                    };
                listener.FatalError += (sender, args) =>
                    {
                        this.TapiFatalError.Invoke(sender, args);
                    };
                listener.StatusMessage += (sender, args) =>
                    {
                        this.TapiStatusMessage.Invoke(sender, args);
                    };
                listener.WarningMessage += (sender, args) =>
                    {
                        this.TapiWarningMessage.Invoke(sender, args);
                    };
            }
        }

        /// <summary>
        /// Creates initializes a <inheritdoc cref="TransferPathProgressListener"/> instance.
        /// </summary>
        private void CreatePathProgressListener()
        {
            var listener = new TransferPathProgressListener(this.transferLog, this.context);
            listener.ProgressEvent += (sender, args) =>
                {
                    this.TapiProgress.Invoke(sender, args);
                };
            this.transferListeners.Add(listener);
        }

        /// <summary>
        /// Creates and initializes a <inheritdoc cref="TransferPathIssueListener"/> instance. 
        /// </summary>
        private void CreatePathIssueListener()
        {
            this.transferListeners.Add(
                new TransferPathIssueListener(
                    this.transferLog,
                    this.currentDirection,
                    this.ClientDisplayName,
                    this.context));
        }

        /// <summary>
        /// Creates and initializes a <inheritdoc cref="TransferRequestListener"/> to listen for transfer request events. 
        /// </summary>
        private void CreateRequestListener()
        {
            this.transferListeners.Add(new TransferRequestListener(this.transferLog, this.context));
        }

        /// <summary>
        /// Creates and initializes a <inheritdoc cref="TransferJobRetryListener"/> to listen for job retry events.
        /// </summary>
        private void CreateJobRetryListener()
        {
            this.transferListeners.Add(
                new TransferJobRetryListener(this.transferLog, this.parameters.MaxJobRetryAttempts, this.context));
        }

        /// <summary>
        /// Creates and initializes a <inheritdoc cref="TransferStatisticsListener"/> to listen for transfer statistic events. 
        /// </summary>
        private void CreateStatisticsListener()
        {
            var listener = new TransferStatisticsListener(this.transferLog, this.context);
            listener.StatisticsEvent += (sender, args) => this.TapiStatistics.Invoke(sender, args);
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
            this.currentJobId = null;
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
        }

        /// <summary>
        /// Logs a cancellation request.
        /// </summary>
        private void LogCancelRequest()
        {
            this.transferLog.LogInformation(
                "The file transfer has been cancelled. ClientId={ClientId}, JobId={JobId} ",
                this.clientRequestId,
                this.currentJobId);
        }
    }
}