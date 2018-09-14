using System;
using System.Threading;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class LongTextTapiBridgePool : ILongTextTapiBridgePool
	{
		private IDownloadTapiBridge _longTextTapiBridge;

		private readonly ILongTextTapiBridgeFactory _factory;
		private readonly ILog _logger;

		public LongTextTapiBridgePool(ILongTextTapiBridgeFactory factory, ILog logger)
		{
			_factory = factory;
			_logger = logger;
		}

		public IDownloadTapiBridge Request(CancellationToken token)
		{
			if (_longTextTapiBridge != null)
			{
				return _longTextTapiBridge;
			}

			_longTextTapiBridge = _factory.Create(token);

			return _longTextTapiBridge;
		}

		public void Release(IDownloadTapiBridge bridge)
		{
			//Do nothing as long text tapi bridge is going through web mode
		}

		public void Dispose()
		{
			try
			{
				_longTextTapiBridge?.Dispose();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to dispose Tapi Bridge.");
				// We do not want to crash container disposal
			}
		}
	}
}