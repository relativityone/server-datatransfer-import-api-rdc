using System;
using kCura.WinEDDS.Exceptions;
using Polly;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata
{
	public class WritersRetryPolicy : IWritersRetryPolicy
	{
		private readonly int _numberOfRetries;
		private readonly int _waitTimeBetweenRetryAttempts;

		public const string CONTEXT_LAST_ARTIFACT_ID_KEY = "LastArtifactId";

		public WritersRetryPolicy(IExportConfig exportConfig)
		{
			_numberOfRetries = exportConfig.ExportErrorNumberOfRetries;
			_waitTimeBetweenRetryAttempts = exportConfig.ExportIOErrorNumberOfRetries;
		}

		public Policy CreateRetryPolicy(Action<Exception, TimeSpan, int, Context> onRetry)
		{
			PolicyBuilder handler = Policy.Handle<ExportBaseException>();
			return handler.WaitAndRetry(_numberOfRetries, SleepDurationProvider, onRetry);
		}

		private TimeSpan SleepDurationProvider(int retryAttempt)
		{
			if (retryAttempt > 1)
			{
				return TimeSpan.FromSeconds(_waitTimeBetweenRetryAttempts);
			}

			return TimeSpan.Zero;
		}
	}
}