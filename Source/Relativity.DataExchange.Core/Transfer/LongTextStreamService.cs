// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LongTextStreamService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a service class object to retrieve long text data through the Object Manager streaming API.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;

	using Polly;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Service;
	using Relativity.Kepler.Transport;
	using Relativity.Logging;
	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;

	/// <summary>
	/// Represents a service class object to retrieve long text data through the Object Manager streaming API.
	/// </summary>
	internal class LongTextStreamService : ILongTextStreamService
	{
		private const string RetryCountKey = "RetryCount";
		private const string RequestKey = "Request";
		private readonly IServiceProxyFactory serviceProxyFactory;
		private readonly IAppSettings settings;
		private readonly IFileSystem fileSystem;
		private readonly ILog logger;
		private readonly int exportLongTextBufferSizeBytes;
		private readonly int exportLongTextLargeFileProgressRateSeconds;
		private readonly ISyncPolicy syncPolicy;
		private readonly IAsyncPolicy asyncRetryPolicy;
		private readonly Lazy<IObjectManager> objectManager;
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="LongTextStreamService"/> class.
		/// </summary>
		/// <param name="serviceProxyFactory">
		/// The factory used to create service proxy instances.
		/// </param>
		/// <param name="serviceRetryPolicyFactory">
		/// The factory used to create service retry policies.
		/// </param>
		/// <param name="settings">
		/// The settings.
		/// </param>
		/// <param name="fileSystem">
		/// The file system wrapper.
		/// </param>
		/// <param name="logger">
		/// The logger instance.
		/// </param>
		public LongTextStreamService(
			IServiceProxyFactory serviceProxyFactory,
			IServiceRetryPolicyFactory serviceRetryPolicyFactory,
			IAppSettings settings,
			IFileSystem fileSystem,
			ILog logger)
		{
			this.serviceProxyFactory = serviceProxyFactory.ThrowIfNull(nameof(serviceProxyFactory));
			this.settings = settings.ThrowIfNull(nameof(settings));
			this.fileSystem = fileSystem.ThrowIfNull(nameof(fileSystem));
			this.logger = logger.ThrowIfNull(nameof(logger));
			this.exportLongTextBufferSizeBytes = settings.ExportLongTextBufferSizeBytes;
			this.exportLongTextLargeFileProgressRateSeconds = settings.ExportLongTextLargeFileProgressRateSeconds;
			this.objectManager = new Lazy<IObjectManager>(this.CreateObjectManager);
			serviceRetryPolicyFactory.ThrowIfNull(nameof(serviceRetryPolicyFactory));
			this.syncPolicy = serviceRetryPolicyFactory.CreatePolicy(this.OnRetryCreateObjectManagerInstance);
			this.asyncRetryPolicy = serviceRetryPolicyFactory.CreateAsyncPolicy(this.OnRetrySaveLongTextStream);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <inheritdoc />
		public Task<LongTextStreamResult> SaveLongTextStreamAsync(
			LongTextStreamRequest request,
			CancellationToken token,
			IProgress<LongTextStreamProgressEventArgs> progress)
		{
			request.ThrowIfNull(nameof(request));
			ValidateRequest(request);
			Context context = new Context("LongTextExecutionKey") { { RetryCountKey, 0 } };
			return this.asyncRetryPolicy.ExecuteAsync(
				(ctx, ct) => this.OnExecuteSaveLongTextStream(request, context, token, progress),
				context,
				token);
		}

		private static int GetRetryCount(Context context)
		{
			return context.TryGetValue(RetryCountKey, out var value) ? Convert.ToInt32(value) : 0;
		}

		private static LongTextStreamRequest GetLongTextStreamRequest(Context context)
		{
			return context.TryGetValue(RequestKey, out var value)
				       ? value as LongTextStreamRequest
				       : new LongTextStreamRequest();
		}

		private static void ValidateRequest(LongTextStreamRequest request)
		{
			if (request.SourceObjectArtifactId < 1 || request.SourceFieldArtifactId < 1)
			{
				throw new ArgumentException(
					$"The long text streaming service request requires a source {(request.SourceFieldArtifactId < 1 ? "field" : "object")} artifact identifier greater than zero.",
					nameof(request));
			}

			if (string.IsNullOrWhiteSpace(request.TargetFile))
			{
				throw new ArgumentException(
					"The long text streaming service request requires the target file to be non-empty.",
					nameof(request));
			}

			if (request.TargetEncoding == null || request.SourceEncoding == null)
			{
				throw new ArgumentException(
					$"The long text streaming service request requires the {(request.SourceEncoding == null ? "source" : "target")} encoding to be defined.",
					nameof(request));
			}

			if (request.WorkspaceId < 1)
			{
				throw new ArgumentException(
					$"The long text streaming service request requires the workspace artifact identifier to be greater than zero.",
					nameof(request));
			}
		}

		private IObjectManager CreateObjectManager()
		{
			// This will address potential issues with exception caching.
			return this.syncPolicy.Execute(() => this.serviceProxyFactory.CreateProxyInstance<IObjectManager>());
		}

		private void LogNonFatalInvalidRequestWarning(LongTextStreamRequest request, Exception exception)
		{
			this.logger.LogWarning(
				exception,
				"The {ArtifactId} - {FieldArtifactId} long text request returned a non-fatal and non-retryable error because 1 or more parameters is invalid.",
				request.SourceObjectArtifactId,
				request.SourceFieldArtifactId);
		}

		private void LogNonFatalObjectManagerWarning(LongTextStreamRequest request, Exception exception)
		{
			this.logger.LogWarning(
				exception,
				"The {ArtifactId} - {FieldArtifactId} long text request returned a non-fatal and non-retryable object manager specific error.",
				request.SourceObjectArtifactId,
				request.SourceFieldArtifactId);
		}

		private async Task<LongTextStreamResult> OnExecuteSaveLongTextStream(LongTextStreamRequest request, Context context, CancellationToken token, IProgress<LongTextStreamProgressEventArgs> progress)
		{
			int retryCount = GetRetryCount(context);

			try
			{
				context[RequestKey] = request;
				RelativityObjectRef exportObject =
					new RelativityObjectRef { ArtifactID = request.SourceObjectArtifactId };
				FieldRef longTextField = new FieldRef { ArtifactID = request.SourceFieldArtifactId };
				try
				{
					using (IKeplerStream keplerStream = await this.objectManager.Value
						                                    .StreamLongTextAsync(
							                                    request.WorkspaceId,
							                                    exportObject,
							                                    longTextField).ConfigureAwait(false))
					using (Stream sourceStream = await keplerStream.GetStreamAsync().ConfigureAwait(false))
					using (FileStream targetStream = this.fileSystem.File.Create(request.TargetFile))
					{
						return this.CopySourceStreamData(request, context, sourceStream, targetStream, token, progress);
					}
				}
				catch (Exception)
				{
					this.TryDeleteTargetFile(request.TargetFile);
					throw;
				}
			}
			catch (ArgumentException e)
			{
				// Non-OM invalid request or path.
				this.LogNonFatalInvalidRequestWarning(request, e);
				return LongTextStreamResult.CreateNonFatalIssueResult(request, retryCount, e);
			}
			catch (NotSupportedException e)
			{
				// Non-OM invalid path.
				this.LogNonFatalInvalidRequestWarning(request, e);
				return LongTextStreamResult.CreateNonFatalIssueResult(request, retryCount, e);
			}
			catch (Exception e)
			{
				if (ObjectManagerExceptionHelper.IsNonFatalError(e))
				{
					this.LogNonFatalObjectManagerWarning(request, e);
					return LongTextStreamResult.CreateNonFatalIssueResult(request, retryCount, e);
				}

				// All other errors are logged and will go through the service retry policy.
				throw;
			}
		}

		private void OnRetryCreateObjectManagerInstance(Exception exception, TimeSpan duration, int retryCount, Context context)
		{
			this.logger.LogError(
				exception,
				"Failed to create the ObjectManager proxy instance. Currently on attempt {RetryCount} out of {MaxRetries} and waiting {WaitSeconds} seconds before the next retry attempt.",
				retryCount,
				this.settings.HttpErrorNumberOfRetries,
				duration.TotalSeconds);
		}

		private void OnRetrySaveLongTextStream(Exception exception, TimeSpan duration, int retryCount, Context context)
		{
			context[RetryCountKey] = retryCount;
			LongTextStreamRequest request = GetLongTextStreamRequest(context);
			this.logger.LogError(
				exception,
				"An error occurred retrieving the {ArtifactId} - {FieldArtifactId} long text data from Object Manager. Currently on attempt {RetryCount} out of {MaxRetries} and waiting {WaitSeconds} seconds before the next retry attempt.",
				request.SourceObjectArtifactId,
				request.SourceFieldArtifactId,
				retryCount,
				this.settings.HttpErrorNumberOfRetries,
				duration.TotalSeconds);
		}

		private LongTextStreamResult CopySourceStreamData(
			LongTextStreamRequest request,
			Context context,
			Stream sourceStream,
			FileStream targetStream,
			CancellationToken token,
			IProgress<LongTextStreamProgressEventArgs> progress)
		{
			// Note: this method is critical - avoid all possible overhead (e.g. Stopwatch).
			// Note: synchronous read/write operations are significantly faster than their asynchronous counterparts.
			long totalBytesWritten = 0;
			DateTime startTimestamp = DateTime.Now;
			const bool DetectEncodingFromByteOrderMarks = false;
			using (StreamReader reader = new StreamReader(
				sourceStream,
				request.SourceEncoding,
				DetectEncodingFromByteOrderMarks,
				this.exportLongTextBufferSizeBytes))
			using (StreamWriter writer = new StreamWriter(
				targetStream,
				request.TargetEncoding,
				this.exportLongTextBufferSizeBytes))
			{
				char[] buffer = new char[this.exportLongTextBufferSizeBytes / sizeof(char)];
				DateTime progressTimestamp = startTimestamp;
				bool continueReading = true;
				while (continueReading)
				{
					token.ThrowIfCancellationRequested();
					try
					{
						int chunkCharacters = reader.Read(buffer, 0, buffer.Length);
						if (chunkCharacters == 0)
						{
							continueReading = false;
							writer.Flush();
						}
						else
						{
							writer.Write(buffer, 0, chunkCharacters);
						}
					}
					finally
					{
						totalBytesWritten = writer.BaseStream.Length;
						if ((!continueReading && startTimestamp != progressTimestamp)
						    || (DateTime.Now - progressTimestamp).TotalSeconds
						    >= this.exportLongTextLargeFileProgressRateSeconds)
						{
							progress?.Report(
								new LongTextStreamProgressEventArgs(request, totalBytesWritten, !continueReading));
							progressTimestamp = DateTime.Now;
						}
					}
				}

				DateTime endTimestamp = DateTime.Now;
				return LongTextStreamResult.CreateSuccessfulResult(
					request,
					request.TargetFile,
					totalBytesWritten,
					GetRetryCount(context),
					endTimestamp - startTimestamp);
			}
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				if (this.objectManager.IsValueCreated)
				{
					this.objectManager.Value.Dispose();
				}

				this.serviceProxyFactory?.Dispose();
				this.disposed = true;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "This is OK for what this method is doing and prefixed the method name to make it obvious.")]
		private void TryDeleteTargetFile(string file)
		{
			try
			{
				// Don't allow expected errors to rethrow.
				this.fileSystem.File.Delete(file);
			}
			catch (Exception e)
			{
				this.logger.LogWarning(e, "Failed to delete the target file.");
			}
		}
	}
}