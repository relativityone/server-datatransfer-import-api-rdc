using System.Threading;

namespace kCura.WinEDDS.Core.Import.Status
{
	public class CancellationProvider : ICancellationProvider
	{
		private readonly CancellationTokenSource _cancelTokenSource;

		public CancellationProvider()
		{
			_cancelTokenSource = new CancellationTokenSource();
		}

		public CancellationToken GetToken()
		{
			return _cancelTokenSource.Token;
		}

		public void Cancel()
		{
			_cancelTokenSource.Cancel();
		}
	}
}
