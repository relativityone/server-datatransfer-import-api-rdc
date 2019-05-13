﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata
{
	using System;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exceptions;

	using Polly;

	public class WritersRetryPolicy : IWritersRetryPolicy
	{
		private readonly int _numberOfRetries;
		private readonly int _waitTimeBetweenRetryAttempts;

		public const string CONTEXT_LAST_ARTIFACT_ID_KEY = "LastArtifactId";

		public WritersRetryPolicy(IExportConfig exportConfig)
		{
			_numberOfRetries = exportConfig.ExportIOErrorNumberOfRetries;
			_waitTimeBetweenRetryAttempts = exportConfig.ExportIOErrorWaitTime;
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