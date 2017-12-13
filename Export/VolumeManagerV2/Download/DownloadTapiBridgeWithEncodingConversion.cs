using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Relativity.Logging;
using Relativity.Transfer;
using ITransferStatistics = kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics.ITransferStatistics;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class DownloadTapiBridgeWithEncodingConversion : DownloadTapiBridgeAdapter
	{
		private bool _initialized;

		private readonly LongTextEncodingConverter _longTextEncodingConverter;

		private readonly ILog _logger;

		public DownloadTapiBridgeWithEncodingConversion(DownloadTapiBridge downloadTapiBridge, IProgressHandler progressHandler, IMessagesHandler messagesHandler, ITransferStatistics transferStatistics,
			LongTextEncodingConverter longTextEncodingConverter, ILog logger) : base(downloadTapiBridge, progressHandler, messagesHandler, transferStatistics)
		{
			_longTextEncodingConverter = longTextEncodingConverter;
			_logger = logger;

			_initialized = false;
			progressHandler.Attach(downloadTapiBridge);
		}

		public override string AddPath(TransferPath transferPath)
		{
			if (!_initialized)
			{
				_logger.LogVerbose("Initializing long text encoding converter.");
				_initialized = true;
				_longTextEncodingConverter.StartListening();
				TapiBridge.TapiProgress += _longTextEncodingConverter.OnTapiProgress;
			}
			return TapiBridge.AddPath(transferPath);
		}

		public override void WaitForTransferJob()
		{
			if (!_initialized)
			{
				_logger.LogVerbose("Long text encoding conversion bridge hasn't been initialized, so skipping waiting.");
				return;
			}
			try
			{
				TapiBridge.WaitForTransferJob();
			}
			finally
			{
				try
				{
					TapiBridge.TapiProgress -= _longTextEncodingConverter.OnTapiProgress;
					_longTextEncodingConverter.StopListening();
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Error occurred when trying to stop LongText encoding conversion after TAPI client failure.");
				}
			}
			_longTextEncodingConverter.WaitForConversionCompletion();
		}
	}
}