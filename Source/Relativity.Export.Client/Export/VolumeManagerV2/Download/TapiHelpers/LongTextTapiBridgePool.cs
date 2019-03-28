using System;
using System.Collections.Generic;
using System.Threading;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class LongTextTapiBridgePool : ILongTextTapiBridgePool
	{
		private readonly List<IDownloadTapiBridge> _longTextTapiBridges;

		private readonly ILongTextTapiBridgeFactory _factory;
		private readonly ILog _logger;

		public LongTextTapiBridgePool(ILongTextTapiBridgeFactory factory, ILog logger)
		{
			_factory = factory;
			_logger = logger;

			_longTextTapiBridges = new List<IDownloadTapiBridge>();
		}

		public IDownloadTapiBridge Request(CancellationToken token)
		{
			IDownloadTapiBridge longTextTapiBridge = _factory.Create(token);
			_longTextTapiBridges.Add(longTextTapiBridge);
			return longTextTapiBridge;
		}

		public void Release(IDownloadTapiBridge bridge)
		{
			_longTextTapiBridges.Remove(bridge);
			TryDisposeTapiBridge(bridge);
		}

		public void Dispose()
		{
			foreach (IDownloadTapiBridge bridge in _longTextTapiBridges)
			{
				TryDisposeTapiBridge(bridge);
			}
		}

		private void TryDisposeTapiBridge(IDownloadTapiBridge bridge)
		{
			try
			{
				bridge?.Dispose();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to dispose Tapi Bridge.");
				// We do not want to crash container disposal
			}
		}
	}
}