namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;


	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Logger;
	using Relativity.Logging;
	using Relativity.Transfer;

	public class LongTextDownloader : ILongTextDownloader, ILongTextFileDownloadSubscriber
	{
		private readonly SafeIncrement _safeIncrement;
		private readonly ILongTextTapiBridgePool _longTextTapiBridgePool;
		private readonly ILog _logger;
		private IFileDownloadSubscriber _fileDownloadSubscriber;
		private readonly IDownloadProgressManager _downloadProgressManager;

		public LongTextDownloader(
			SafeIncrement safeIncrement,
			ILongTextTapiBridgePool longTextTapiBridgePool,
			IDownloadProgressManager downloadProgressManager,
			ILog logger)
		{
			_safeIncrement = safeIncrement.ThrowIfNull(nameof(safeIncrement));
			_longTextTapiBridgePool = longTextTapiBridgePool.ThrowIfNull(nameof(longTextTapiBridgePool));
			_downloadProgressManager = downloadProgressManager.ThrowIfNull(nameof(downloadProgressManager));
			_logger = logger.ThrowIfNull(nameof(logger));
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

				this._fileDownloadSubscriber?.SubscribeForDownloadEvents(bridge, cancellationToken);

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
							textExportRequest.FieldArtifactId, textExportRequest.DestinationLocation.Secure());
						TransferPath path = textExportRequest.CreateTransferPath(_safeIncrement.GetNext());
						textExportRequest.FileName = bridge.QueueDownload(path);
					}
					catch (ArgumentException ex)
					{
						_logger.LogWarning(
							ex,
							"There was a problem downloading artifact {ArtifactId}.",
							textExportRequest.ArtifactId);
						_downloadProgressManager.MarkArtifactAsError(textExportRequest.ArtifactId, ex.Message);
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
					_fileDownloadSubscriber?.Dispose();
					_longTextTapiBridgePool.Release(bridge);
				}
			}
		}
	}
}