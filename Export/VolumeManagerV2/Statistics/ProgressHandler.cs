using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.TApi;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public abstract class ProgressHandler : IProgressHandler
	{
		private TapiBridge _tapiBridge;

		protected DownloadStatistics DownloadStatistics { get; }

		protected ProgressHandler(DownloadStatistics downloadStatistics)
		{
			DownloadStatistics = downloadStatistics;
		}

		public void Attach(TapiBridge tapiBridge)
		{
			_tapiBridge = tapiBridge;
			_tapiBridge.TapiProgress += OnFileProgress;
		}

		private void OnFileProgress(object sender, TapiProgressEventArgs e)
		{
			MarkAsDownloaded(e.FileName);
		}

		protected abstract void MarkAsDownloaded(string id);

		public void Detach()
		{
			_tapiBridge.TapiProgress -= OnFileProgress;
		}
	}
}