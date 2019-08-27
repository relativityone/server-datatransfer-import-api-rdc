namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;


	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;
	using Relativity.Transfer;

	public class LongTextDownloader : ILongTextDownloader
	{
		private readonly SafeIncrement _safeIncrement;
		private readonly ILongTextTapiBridgePool _longTextTapiBridgePool;
		private readonly ILog _logger;
		private IFileDownloadSubscriber _fileDownloadSubscriber;

		public LongTextDownloader(SafeIncrement safeIncrement, ILongTextTapiBridgePool longTextTapiBridgePool, ILog logger)
		{
			_safeIncrement = safeIncrement;
			_longTextTapiBridgePool = longTextTapiBridgePool;
			_logger = logger;
		}

		public async Task DownloadAsync(List<LongTextExportRequest> longTextExportRequests, CancellationToken cancellationToken)
		{
			await Task.Run(() => Download(longTextExportRequests, cancellationToken)).ConfigureAwait(false);
		}

		public void RegisterSubscriber(IFileDownloadSubscriber fileDownloadSubscriber)
		{
			fileDownloadSubscriber.ThrowIfNull(nameof(fileDownloadSubscriber));

			this._fileDownloadSubscriber = fileDownloadSubscriber;
		}

		private void Download(List<LongTextExportRequest> longTextExportRequests, CancellationToken cancellationToken)
		{
			if (longTextExportRequests == null)
			{
				throw new ArgumentNullException(nameof(longTextExportRequests));
			}

			IDownloadTapiBridge bridge = null;
			try
			{
				bridge = _longTextTapiBridgePool.Request(cancellationToken);

				_logger.LogDebug("Subscribing '{_fileDownloadSubscriber}' for the download event", this._fileDownloadSubscriber);

				if (longTextExportRequests.Any())
				{
					this._fileDownloadSubscriber?.SubscribeForDownloadEvents(bridge);
				}

				foreach (LongTextExportRequest textExportRequest in longTextExportRequests)
				{
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}

					try
					{
						_logger.LogVerbose(
							"Adding export request for downloading long text {fieldId} to {destination}.",
							textExportRequest.FieldArtifactId, textExportRequest.DestinationLocation);
						TransferPath path = textExportRequest.CreateTransferPath(_safeIncrement.GetNext());
						textExportRequest.FileName = bridge.QueueDownload(path);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex,
							"Error occurred during adding long text export request to TAPI bridge. Skipping.");
						throw;
					}
				}

				bridge.WaitForTransfers();
			}
			finally
			{
				if (bridge != null)
				{
					_longTextTapiBridgePool.Release(bridge);
				}
			}
		}
	}
}