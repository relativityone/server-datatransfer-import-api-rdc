namespace Relativity.Export.VolumeManagerV2.Statistics
{
	using System;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Service.Export;

	using Relativity.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Import.Export.Transfer;
	using Relativity.Logging;

	public class ExportFileDownloaderStatus : IExportFileDownloaderStatus, ITransferClientHandler
	{
		private ITapiBridge _tapiBridge;
		private readonly ILog _logger;

		public event IExportFileDownloaderStatus.UploadModeChangeEventEventHandler UploadModeChangeEvent;

		public FileDownloader.FileAccessType UploaderType { get; set; }

		public ExportFileDownloaderStatus(ILog logger)
		{
			_logger = logger;
			UploaderType = FileDownloader.FileAccessType.Web;
		}

		public void Attach(ITapiBridge tapiBridge)
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
			if (Enum.TryParse(e.Name, true, out uploaderType))
			{
				UploaderType = uploaderType;
			}

			UploadModeChangeEvent?.Invoke(UploaderType.ToString());
		}
	}
}