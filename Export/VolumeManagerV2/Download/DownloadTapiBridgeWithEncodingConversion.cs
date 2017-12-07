using System;
using kCura.WinEDDS.TApi;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class DownloadTapiBridgeWithEncodingConversion : IDownloadTapiBridge
	{
		private bool _initialized;

		private readonly DownloadTapiBridge _downloadTapiBridge;
		private readonly LongTextEncodingConverter _longTextEncodingConverter;

		private readonly ILog _logger;

		public DownloadTapiBridgeWithEncodingConversion(DownloadTapiBridge downloadTapiBridge, LongTextEncodingConverter longTextEncodingConverter, ILog logger)
		{
			_downloadTapiBridge = downloadTapiBridge;
			_longTextEncodingConverter = longTextEncodingConverter;
			_logger = logger;

			_initialized = false;
		}

		public string AddPath(TransferPath transferPath)
		{
			if (!_initialized)
			{
				_initialized = true;
				_longTextEncodingConverter.StartListening();
				_downloadTapiBridge.TapiProgress += _longTextEncodingConverter.OnTapiProgress;
			}
			return _downloadTapiBridge.AddPath(transferPath);
		}

		public void WaitForTransferJob()
		{
			if (!_initialized)
			{
				return;
			}
			try
			{
				_downloadTapiBridge.WaitForTransferJob();
			}
			finally
			{
				try
				{
					_downloadTapiBridge.TapiProgress -= _longTextEncodingConverter.OnTapiProgress;
					_longTextEncodingConverter.StopListening();
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Error occurred when trying to stop LongText encoding conversion after TAPI client failure.");
				}
			}
			_longTextEncodingConverter.WaitForConversionCompletion();
		}

		public void Dispose()
		{
			_downloadTapiBridge?.Dispose();
			_longTextEncodingConverter?.Dispose();
		}
	}
}