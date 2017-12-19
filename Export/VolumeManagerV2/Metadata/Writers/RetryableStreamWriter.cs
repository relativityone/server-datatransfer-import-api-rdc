﻿using System;
using System.IO;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exceptions;
using Polly;
using Polly.Retry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public abstract class RetryableStreamWriter : IRetryableStreamWriter
	{
		private StreamWriter _streamWriter;
		private long _streamWriterLastPosition;
		private bool _isBroken;
		private bool _initialCreation;
		private long _lastBatchSavedState;

		private readonly RetryPolicy _retryPolicy;
		private readonly StreamFactory _streamFactory;
		private readonly IDestinationPath _destinationPath;
		private readonly IProcessingStatistics _processingStatistics;
		private readonly IStatus _status;
		private readonly ILog _logger;

		protected RetryableStreamWriter(WritersRetryPolicy writersRetryPolicy, StreamFactory streamFactory, IDestinationPath destinationPath, IProcessingStatistics processingStatistics,
			IStatus status, ILog logger)
		{
			_streamFactory = streamFactory;
			_destinationPath = destinationPath;
			_processingStatistics = processingStatistics;
			_status = status;
			_logger = logger;
			_retryPolicy = writersRetryPolicy.CreateRetryPolicy(OnRetry);

			_isBroken = false;
			_initialCreation = true;
			_streamWriterLastPosition = 0;
			_lastBatchSavedState = 0;
		}

		private void OnRetry(Exception exception, TimeSpan timeBetweenRetries, int retryCount, Context context)
		{
			_isBroken = true;

			_logger.LogWarning("Retrying writing to file. Error occurred {exception}.", exception.Message);

			_status.WriteWarning($"Error writing entry to file {_destinationPath.Path}.");
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

		public void WriteEntry(string loadFileEntry, CancellationToken token)
		{
			_retryPolicy.Execute(t =>
			{
				CreateStreamIfNeeded();

				try
				{
					_streamWriter.Write(loadFileEntry);
				}
				catch (IOException ex)
				{
					_logger.LogError(ex, "Error occurred during writing to file {type}.", _destinationPath.DestinationFileType);
					throw new FileWriteException(_destinationPath.DestinationFileType, ex);
				}

				SaveStreamPosition();
				UpdateStatistics();
			}, token);
		}

		public void WriteChunk(string chunk, CancellationToken token)
		{
			_retryPolicy.Execute(t =>
			{
				CreateStreamIfNeeded();

				try
				{
					_streamWriter.Write(chunk);
				}
				catch (IOException ex)
				{
					_logger.LogError(ex, "Error occurred during writing to file {type}.", _destinationPath.DestinationFileType);
					throw new FileWriteException(_destinationPath.DestinationFileType, ex);
				}
			}, token);
		}

		public void FlushChunks(CancellationToken token)
		{
			_retryPolicy.Execute(t =>
			{
				CreateStreamIfNeeded();
				SaveStreamPosition();
				UpdateStatistics();
			}, token);
		}

		private void CreateStreamIfNeeded()
		{
			if (_isBroken || _initialCreation)
			{
				_logger.LogVerbose("Stream broken or hasn't been initialized. Creating.");
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
				_logger.LogVerbose("Stream position {position} saved.", _streamWriterLastPosition);
			}
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

		private void UpdateStatistics()
		{
			_processingStatistics.AddStatisticsForFile(_destinationPath.Path);
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
			if (_initialCreation)
			{
				_logger.LogVerbose("StreamWriter hasn't been initialized. Nothing to restore.");
				return;
			}
			_streamWriter = _streamFactory.Create(_streamWriter, _lastBatchSavedState, _destinationPath.Path, _destinationPath.Encoding, true);
			_streamWriterLastPosition = _lastBatchSavedState;
		}
	}
}