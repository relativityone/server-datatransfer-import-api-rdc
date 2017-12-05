using System;
using kCura.WinEDDS.Service.Export;
using kCura.WinEDDS.TApi;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class ExportFileDownloaderStatus : IExportFileDownloaderStatus
	{
		public event IExportFileDownloaderStatus.UploadModeChangeEventEventHandler UploadModeChangeEvent;

		public FileDownloader.FileAccessType UploaderType { get; set; }

		public ExportFileDownloaderStatus()
		{
			UploaderType = FileDownloader.FileAccessType.Initializing;
		}

		public void OnTapiClientChanged(object sender, TapiClientEventArgs e)
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