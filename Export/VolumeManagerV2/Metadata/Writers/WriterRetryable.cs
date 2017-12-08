using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exceptions;
using kCura.WinEDDS.Exporters;
using Polly;
using Polly.Retry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class WriterRetryable : IStateful, IDisposable
	{
		private StreamWriter _streamWriter;
		private long _streamWriterLastPosition;
		private bool _isBroken;
		private bool _initialCreation;

		private long _lastBatchSavedState;

		private readonly RetryPolicy _retryPolicy;
		private readonly StreamFactory _streamFactory;
		private readonly ILog _logger;
		private readonly IStatus _status;
		private readonly IDestinationPath _destinationPath;
		private readonly IProcessingStatistics _processingStatistics;

		public WriterRetryable(WritersRetryPolicy writersRetryPolicy, StreamFactory streamFactory, ILog logger, IStatus status, IDestinationPath destinationPath, IProcessingStatistics processingStatistics)
		{
			_streamFactory = streamFactory;
			_logger = logger;
			_status = status;
			_destinationPath = destinationPath;
			_processingStatistics = processingStatistics;
			_retryPolicy = writersRetryPolicy.CreateRetryPolicy(OnRetry);

			_streamWriterLastPosition = 0;
			_lastBatchSavedState = 0;
			_isBroken = false;
			_initialCreation = true;
		}

		protected void Execute(Action<IEnumerator<ObjectExportInfo>, StreamWriter> write, ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			Context context = new Context(Guid.NewGuid().ToString());

			IEnumerator<ObjectExportInfo> enumerator = new ArtifactEnumerator(artifacts, context);

			_retryPolicy.Execute((c, t) =>
			{
				CreateStreamIfNeeded();
				try
				{
					write(enumerator, _streamWriter);
				}
				catch (IOException ex)
				{
					_logger.LogError(ex, "Error occurred during writing to file {type}.", _destinationPath.DestinationFileType);
					throw new FileWriteException(_destinationPath.DestinationFileType, ex);
				}
				SaveStreamPosition();
				UpdateStatistics();
			}, context, cancellationToken);
		}

		private void OnRetry(Exception exception, TimeSpan timeBetweenRetries, int retryCount, Context context)
		{
			_isBroken = true;

			_logger.LogWarning("Retrying writing to file. Error occurred {exception}.", exception.Message);

			int lastArtifactId = GetLastArtifactId(context);

			_status.WriteWarning($"Error writing entry for artifact {lastArtifactId} to file {_destinationPath.Path}.");
			_status.WriteWarning($"Actual error: {exception}");
			if (retryCount > 1)
			{
				_status.WriteWarning($"Waiting {timeBetweenRetries.Seconds} seconds to retry");
				_logger.LogVerbose("Waiting {time} before next retry.", timeBetweenRetries.Seconds);
			}
			else
			{
				_status.WriteWarning("Retrying now");
			}
		}

		private void CreateStreamIfNeeded()
		{
			if (_isBroken || _initialCreation)
			{
				bool append = !_initialCreation;
				_streamWriter = _streamFactory.Create(_streamWriter, _streamWriterLastPosition, _destinationPath.Path, _destinationPath.Encoding, append);
				_isBroken = false;
				_initialCreation = false;
			}
		}

		private void SaveStreamPosition()
		{
			if (_streamWriter != null)
			{
				FlushStream();
				_streamWriterLastPosition = _streamWriter.BaseStream.Position;

			}
		}

		private void UpdateStatistics()
		{
			_processingStatistics.AddStatisticsForFile(_destinationPath.Path);
		}

		private void FlushStream()
		{
			try
			{
				_streamWriter?.Flush();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to flush {type} file stream.", _destinationPath.DestinationFileType);
				throw new FileWriteException(_destinationPath.DestinationFileType, ex);
			}
		}

		private int GetLastArtifactId(Context context)
		{
			int lastArtifactId = -1;
			if (context.ContainsKey(WritersRetryPolicy.CONTEXT_LAST_ARTIFACT_ID_KEY))
			{
				lastArtifactId = (int) context[WritersRetryPolicy.CONTEXT_LAST_ARTIFACT_ID_KEY];
			}
			else
			{
				_logger.LogWarning("Failed to retrieve artifactId from retry context. Continuing with -1.");
			}
			return lastArtifactId;
		}

		public void Dispose()
		{
			_streamWriter?.Dispose();
		}

		public void SaveState()
		{
			_lastBatchSavedState = _streamWriterLastPosition;
		}

		public void RestoreLastState()
		{
			_streamWriter = _streamFactory.Create(_streamWriter, _lastBatchSavedState, _destinationPath.Path, _destinationPath.Encoding, true);
			_streamWriterLastPosition = _lastBatchSavedState;
		}
	}
}