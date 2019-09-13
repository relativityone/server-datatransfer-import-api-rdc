namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using Relativity.Logging;

	public class LongTextTapiBridgePool : ILongTextTapiBridgePool
	{
		private readonly object _syncRoot = new object();
		private readonly List<IDownloadTapiBridge> _longTextTapiBridges;
		private readonly ILongTextDownloadTapiBridgeFactory _factory;
		private readonly ILog _logger;

		public LongTextTapiBridgePool(ILongTextDownloadTapiBridgeFactory factory, ILog logger)
		{
			_factory = factory;
			_logger = logger;
			_longTextTapiBridges = new List<IDownloadTapiBridge>();
		}

		/// <summary>
		/// Gets the total number of transfer bridges.
		/// </summary>
		/// <value>
		/// The transfer bridge count.
		/// </value>
		public int Count
		{
			get
			{
				lock (_syncRoot)
				{
					return _longTextTapiBridges.Count;
				}
			}
		}

		public IDownloadTapiBridge Request(CancellationToken token)
		{
			IDownloadTapiBridge longTextTapiBridge = _factory.Create(token);
			lock (_syncRoot)
			{
				_longTextTapiBridges.Add(longTextTapiBridge);
			}

			return longTextTapiBridge;
		}

		public void Release(IDownloadTapiBridge bridge)
		{
			lock (_syncRoot)
			{
				_longTextTapiBridges.Remove(bridge);
			}

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