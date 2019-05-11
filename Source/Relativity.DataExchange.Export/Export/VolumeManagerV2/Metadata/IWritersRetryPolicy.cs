namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata
{
	using System;

	using Polly;

	public interface IWritersRetryPolicy
	{
		Policy CreateRetryPolicy(Action<Exception, TimeSpan, int, Context> onRetry);
	}
}