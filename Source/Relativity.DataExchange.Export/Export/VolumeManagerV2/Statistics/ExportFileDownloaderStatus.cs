namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using kCura.WinEDDS.Service.Export;

	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class ExportFileDownloaderStatus : IExportFileDownloaderStatus, ITransferClientHandler
	{
		private readonly ILog _logger;

		public ExportFileDownloaderStatus(ILog logger)
		{
			_logger = logger.ThrowIfNull(nameof(logger));
			this.UploaderType = TapiClient.Web;
		}

		public event IExportFileDownloaderStatus.UploadModeChangeEventEventHandler UploadModeChangeEvent;

		public TapiClient UploaderType { get; set; }

		public void Attach(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose(
				"Attached tapi bridge {TapiBridgeInstanceId} to the client change handler.",
				tapiBridge.InstanceId);
			tapiBridge.TapiClientChanged += this.OnTapiClientChanged;
		}

		public void Detach(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose(
				"Detached tapi bridge {TapiBridgeInstanceId} from the client change handler.",
				tapiBridge.InstanceId);
			tapiBridge.TapiClientChanged -= this.OnTapiClientChanged;
		}

		private void OnTapiClientChanged(object sender, TapiClientEventArgs e)
		{
			// TODO: Must account for hybrid clients (e.g. Aspera and Web).
			_logger.LogInformation("Tapi client changed to {type}.", e.Name);
			this.UploaderType = e.Client;
			this.UploadModeChangeEvent?.Invoke(this.UploaderType);
		}
	}
}