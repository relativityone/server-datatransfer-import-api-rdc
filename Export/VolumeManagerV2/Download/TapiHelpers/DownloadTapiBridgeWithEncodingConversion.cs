using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using Relativity.Logging;
using Relativity.Transfer;
using ITransferStatistics = kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics.ITransferStatistics;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class DownloadTapiBridgeWithEncodingConversion : DownloadTapiBridgeAdapter
	{
		private bool _initialized;

		private readonly ILongTextEncodingConverter _longTextEncodingConverter;

		private readonly ILog _logger;

		public DownloadTapiBridgeWithEncodingConversion(ITapiBridge downloadTapiBridge, IProgressHandler progressHandler, IMessagesHandler messagesHandler,
			ITransferStatistics transferStatistics, ILongTextEncodingConverter longTextEncodingConverter, ILog logger) : base(downloadTapiBridge, progressHandler, messagesHandler,
			transferStatistics)
		{
			_longTextEncodingConverter = longTextEncodingConverter;
			_logger = logger;

			_initialized = false;
		}

		public override string AddPath(TransferPath transferPath)
		{
			if (!_initialized)
			{
				_logger.LogVerbose("Initializing long text encoding converter.");
				_initialized = true;
				_longTextEncodingConverter.StartListening(TapiBridge);
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
					_longTextEncodingConverter.StopListening(TapiBridge);
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