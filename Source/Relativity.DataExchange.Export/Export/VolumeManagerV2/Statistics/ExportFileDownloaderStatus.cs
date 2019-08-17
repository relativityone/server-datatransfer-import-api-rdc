namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System;
	using System.Collections.Generic;

	using kCura.WinEDDS.Service.Export;

	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class ExportFileDownloaderStatus : IExportFileDownloaderStatus, ITransferClientHandler
	{
		private readonly object _syncRoot = new object();
		private readonly ILog _logger;
		private readonly Dictionary<Guid, TapiClient> _attachedBridgeTapiClients = new Dictionary<Guid, TapiClient>();

		public ExportFileDownloaderStatus(ILog logger)
		{
			_logger = logger.ThrowIfNull(nameof(logger));
		}

		public event IExportFileDownloaderStatus.TransferModeChangeEventEventHandler TransferModeChangeEvent;

		public TapiClient TransferMode
		{
			get;
			private set;
		}

		public void Attach(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			lock (_syncRoot)
			{
				_attachedBridgeTapiClients[tapiBridge.InstanceId] = tapiBridge.Client;
				_logger.LogVerbose(
					"Attached native tapi bridge {TapiBridgeInstanceId} to the client change handler for {TotalTapiBridgeCount} transfer bridge(s).",
					tapiBridge.InstanceId,
					_attachedBridgeTapiClients.Count);
				tapiBridge.TapiClientChanged += this.OnTapiClientChanged;
				this.CalculateTransferMode();
			}
		}

		public void Detach(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			lock (_syncRoot)
			{
				_attachedBridgeTapiClients.Remove(tapiBridge.InstanceId);
				_logger.LogVerbose(
					"Detached native tapi bridge {TapiBridgeInstanceId} from the client change handler for {TotalTapiBridgeCount} transfer bridge(s).",
					tapiBridge.InstanceId,
					_attachedBridgeTapiClients.Count);
				tapiBridge.TapiClientChanged -= this.OnTapiClientChanged;
				this.CalculateTransferMode();
			}
		}

		private void CalculateTransferMode()
		{
			TapiClient newMode = TapiClient.None;
			foreach (Guid instanceId in _attachedBridgeTapiClients.Keys)
			{
				newMode |= _attachedBridgeTapiClients[instanceId];
			}

			this.TransferMode = newMode;
			_logger.LogInformation(
				"Calculated new native transfer mode {DownloadMode} from {TotalTapiBridgeCount} transfer bridge(s).",
				this.TransferMode,
				_attachedBridgeTapiClients.Count);
		}

		private void OnTapiClientChanged(object sender, TapiClientEventArgs e)
		{
			_logger.LogInformation(
				"Native download Tapi client {TapiBridgeInstanceId} changed to {TransferClient}.",
				e.InstanceId,
				e.Name);
			lock (_syncRoot)
			{
				_attachedBridgeTapiClients[e.InstanceId] = e.Client;
				this.CalculateTransferMode();
			}

			this.TransferModeChangeEvent?.Invoke(this.TransferMode);
		}
	}
}