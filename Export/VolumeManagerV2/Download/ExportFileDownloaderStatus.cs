using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Service.Export;
using kCura.WinEDDS.TApi;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class ExportFileDownloaderStatus : IExportFileDownloaderStatus, ITransferClientHandler
	{
		private TapiBridge _tapiBridge;

		public event IExportFileDownloaderStatus.UploadModeChangeEventEventHandler UploadModeChangeEvent;

		public FileDownloader.FileAccessType UploaderType { get; set; }

		public ExportFileDownloaderStatus()
		{
			UploaderType = FileDownloader.FileAccessType.Initializing;
		}

		public void Attach(TapiBridge tapiBridge)
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
			FileDownloader.FileAccessType uploaderType;
			if (Enum.TryParse(e.Name, out uploaderType))
			{
				UploaderType = uploaderType;
			}
			UploadModeChangeEvent?.Invoke(UploaderType.ToString());
		}
	}
}