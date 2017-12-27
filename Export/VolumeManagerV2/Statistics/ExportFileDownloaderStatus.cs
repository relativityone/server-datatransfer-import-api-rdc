using System;
using kCura.WinEDDS.Service.Export;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class ExportFileDownloaderStatus : IExportFileDownloaderStatus, ITransferClientHandler
	{
		private TapiBridgeBase _tapiBridge;
		private readonly ILog _logger;

		public event IExportFileDownloaderStatus.UploadModeChangeEventEventHandler UploadModeChangeEvent;

		public FileDownloader.FileAccessType UploaderType { get; set; }

		public ExportFileDownloaderStatus(ILog logger)
		{
			_logger = logger;
			UploaderType = FileDownloader.FileAccessType.Initializing;
		}

		public void Attach(TapiBridgeBase tapiBridge)
		{
			_tapiBridge = tapiBridge;
			_tapiBridge.TapiClientChanged += OnTapiClientChanged;
		}

		public void Detach()
		{
			_tapiBridge.TapiClientChanged -= OnTapiClientChanged;
		}

		private void OnTapiClientChanged(object sender, TapiClientEventArgs e)
		{
			_logger.LogInformation("Tapi client changed to {type}.", e.Name);
			FileDownloader.FileAccessType uploaderType;
			if (Enum.TryParse(e.Name, out uploaderType))
			{
				UploaderType = uploaderType;
			}

			UploadModeChangeEvent?.Invoke(UploaderType.ToString());
		}
	}
}