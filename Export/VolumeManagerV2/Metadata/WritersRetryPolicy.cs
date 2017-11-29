using System;
using kCura.WinEDDS.Exceptions;
using Polly;
using Polly.Retry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata
{
	public class WritersRetryPolicy
	{
		private const string _RETRY_IMAGES_LOAD_FILE_WARNING = "Error writing .opt file entry for artifact {0}";
		private const string _RETRY_LOAD_FILE_WARNING = "Error writing metadata for artifact {0}";

		private readonly int _numberOfRetries;
		private readonly int _waitTimeBetweenRetryAttempts;

		private readonly IStatus _status;
		private readonly ILog _logger;

		public const string CONTEXT_LAST_ARTIFACT_ID_KEY = "LastArtifactId";

		public WritersRetryPolicy(Settings.Config config, IStatus status, ILog logger)
		{
			_status = status;
			_logger = logger;
			_numberOfRetries = config.NumberOfIORetries;
			_waitTimeBetweenRetryAttempts = config.WaitTimeBetweenIORetryAttempts;
		}

		public RetryPolicy CreateRetryPolicyForImageLoadFile()
		{
			PolicyBuilder handler = Policy.Handle<ExportBaseException>();
			return handler.WaitAndRetry(_numberOfRetries, SleepDurationProvider, OnImagesRetry);
		}

		public RetryPolicy CreateRetryPolicyForLoadFile()
		{
			PolicyBuilder handler = Policy.Handle<ExportBaseException>();
			return handler.WaitAndRetry(_numberOfRetries, SleepDurationProvider, OnMetadataRetry);
		}

		private TimeSpan SleepDurationProvider(int retryAttempt)
		{
			if (retryAttempt > 1)
			{
				return TimeSpan.FromSeconds(_waitTimeBetweenRetryAttempts);
			}
			return TimeSpan.Zero;
		}

		private void OnImagesRetry(Exception exception, TimeSpan timeBetweenRetries, int retryCount, Context context)
		{
			OnRetry(exception, retryCount, context, _RETRY_IMAGES_LOAD_FILE_WARNING);
		}

		private void OnMetadataRetry(Exception exception, TimeSpan timeBetweenRetries, int retryCount, Context context)
		{
			OnRetry(exception, retryCount, context, _RETRY_LOAD_FILE_WARNING);
		}

		private void OnRetry(Exception exception, int retryCount, Context context, string warningMessage)
		{
			_logger.LogWarning("Retrying writing to file. Error occurred {exception}.", exception.Message);

			int lastArtifactId = -1;
			if (context.ContainsKey(CONTEXT_LAST_ARTIFACT_ID_KEY))
			{
				lastArtifactId = (int) context[CONTEXT_LAST_ARTIFACT_ID_KEY];
			}
			else
			{
				_logger.LogWarning("Failed to retrieve artifactId from retry context. Continuing with -1.");
			}
			_status.WriteWarning(string.Format(warningMessage, lastArtifactId));
			_status.WriteWarning($"Actual error: {exception}");
			if (retryCount > 1)
			{
				_status.WriteWarning($"Waiting {_waitTimeBetweenRetryAttempts} seconds to retry");
				_logger.LogVerbose("Waiting {time} before next retry.", _waitTimeBetweenRetryAttempts);
			}
			else
			{
				_status.WriteWarning("Retrying now");
			}
		}
	}
}