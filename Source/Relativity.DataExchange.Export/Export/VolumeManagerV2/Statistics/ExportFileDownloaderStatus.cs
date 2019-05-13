namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using kCura.WinEDDS.Service.Export;

	using Relativity.DataExchange.Transfer;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;

	public class ExportFileDownloaderStatus : IExportFileDownloaderStatus, ITransferClientHandler
	{
		private ITapiBridge _tapiBridge;
		private readonly ILog _logger;

		public event IExportFileDownloaderStatus.UploadModeChangeEventEventHandler UploadModeChangeEvent;

		public TapiClient UploaderType { get; set; }

		public ExportFileDownloaderStatus(ILog logger)
		{
			_logger = logger;
			UploaderType = TapiClient.Web;
		}

		public void Attach(ITapiBridge tapiBridge)
		{
			_tapiBridge = tapiBridge;
			_tapiBridge.TapiClientChanged += this.OnTapiClientChanged;
		}

		public void Detach()
		{
			_tapiBridge.TapiClientChanged -= this.OnTapiClientChanged;
		}

		private void OnTapiClientChanged(object sender, TapiClientEventArgs e)
		{
			_logger.LogInformation("Tapi client changed to {type}.", e.Name);
			this.UploaderType = e.Client;
			this.UploadModeChangeEvent?.Invoke(this.UploaderType);
		}
	}
}