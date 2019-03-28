using System;
using Polly;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata
{
	public interface IWritersRetryPolicy
	{
		Policy CreateRetryPolicy(Action<Exception, TimeSpan, int, Context> onRetry);
	}
}