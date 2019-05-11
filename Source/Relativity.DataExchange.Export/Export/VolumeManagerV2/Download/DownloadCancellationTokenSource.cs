namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System.Threading;

	public class DownloadCancellationTokenSource : CancellationTokenSource
	{
		private CancellationToken _batchCancellationToken;

		public DownloadCancellationTokenSource(CancellationToken batchCancellationToken)
		{
			_batchCancellationToken = batchCancellationToken;

			_batchCancellationToken.Register(Cancel);
		}

		public bool IsBatchCancelled()
		{
			return _batchCancellationToken.IsCancellationRequested;
		}

		public void CancelDownload()
		{
			Cancel();
		}
	}
}
