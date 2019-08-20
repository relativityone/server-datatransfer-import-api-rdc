namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

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

		public event IExportFileDownloaderStatus.TransferModesChangeEventEventHandler TransferModesChangeEvent;

		public IList<TapiClient> TransferModes
		{
			get
			{
				lock (_syncRoot)
				{
					return this._attachedBridgeTapiClients.Values.ToList();
				}
			}
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
				this.CalculateTransferModes();
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
				this.CalculateTransferModes();
			}
		}

		private void CalculateTransferModes()
		{
			List<TapiClient> clients = new List<TapiClient>();
			foreach (Guid instanceId in _attachedBridgeTapiClients.Keys)
			{
				clients.Add(_attachedBridgeTapiClients[instanceId]);
			}

			_logger.LogInformation(
				"Calculated new native transfer modes {DownloadMode} from {TotalTapiBridgeCount} transfer bridge(s).",
				string.Join(",", clients),
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
				this.CalculateTransferModes();
			}

			this.TransferModesChangeEvent?.Invoke(this, new TapiMultiClientEventArgs(this.TransferModes));
		}
	}
}