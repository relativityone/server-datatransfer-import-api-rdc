using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class LongTextDownloader : ILongTextDownloader
	{
		private readonly SafeIncrement _safeIncrement;
		private readonly ILongTextTapiBridgePool _longTextTapiBridgePool;
		private readonly ILog _logger;

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

		private void Download(List<LongTextExportRequest> longTextExportRequests, CancellationToken cancellationToken)
		{
			IDownloadTapiBridge bridge = _longTextTapiBridgePool.Request(cancellationToken);

			foreach (LongTextExportRequest textExportRequest in longTextExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				try
				{
					_logger.LogVerbose("Adding export request for downloading long text {fieldId} to {destination}.", textExportRequest.FieldArtifactId, textExportRequest.DestinationLocation);
					TransferPath path = textExportRequest.CreateTransferPath(_safeIncrement.GetNext());
					textExportRequest.FileName = bridge.QueueDownload(path);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred during adding long text export request to TAPI bridge. Skipping.");
					throw;
				}
			}

			bridge.WaitForTransferJob();
			_longTextTapiBridgePool.Release(bridge);
		}
	}
}