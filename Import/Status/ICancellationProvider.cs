using System.Threading;

namespace kCura.WinEDDS.Core.Import.Status
{
	public interface ICancellationProvider
	{
		CancellationToken GetToken();

		void Cancel();

		void ThrowIfCancellationRequested();
	}
}
