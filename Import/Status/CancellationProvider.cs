using System.Threading;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Import.Status
{
	public class CancellationProvider : ICancellationProvider
	{
		private readonly ILog _log;
		private readonly CancellationTokenSource _cancelTokenSource;

		public CancellationProvider(ILog log)
		{
			_log = log;
			_cancelTokenSource = new CancellationTokenSource();
		}

		public CancellationToken GetToken()
		{
			return _cancelTokenSource.Token;
		}

		public void Cancel()
		{
			_log.LogInformation(LogMessages.UserImportCancelMessage);
			_cancelTokenSource.Cancel();
		}

		public void ThrowIfCancellationRequested()
		{
			_cancelTokenSource.Token.ThrowIfCancellationRequested();
		}
	}
}
