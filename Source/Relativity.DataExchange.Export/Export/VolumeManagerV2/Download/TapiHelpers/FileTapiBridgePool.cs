namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using Relativity.DataExchange.Transfer;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.Logging;

	/// <summary>
	/// Represents a class object used to dynamically create and manage pooled transfer bridges.
	/// Implements the <see cref="Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers.IFileTapiBridgePool" />
	/// </summary>
	/// <seealso cref="Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers.IFileTapiBridgePool" />
	public class FileTapiBridgePool : IFileTapiBridgePool
	{
		private readonly object _syncRoot = new object();
		private readonly IDictionary<IRelativityFileShareSettings, IDownloadTapiBridge> _fileTapiBridges =
			new Dictionary<IRelativityFileShareSettings, IDownloadTapiBridge>();
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly ITapiObjectService _tapiObjectService;
		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly FilesStatistics _filesStatistics;
		private readonly IMessagesHandler _messageHandler;
		private readonly ITransferClientHandler _transferClientHandler;
		private readonly ILog _logger;
		private IDownloadTapiBridge _nullSettingsTapiBridge;
		private bool _disposed;

		public FileTapiBridgePool(
			TapiBridgeParametersFactory factory,
			ITapiObjectService tapiObjectService,
			DownloadProgressManager downloadProgressManager,
			FilesStatistics filesStatistics,
			IMessagesHandler messageHandler,
			ITransferClientHandler transferClientHandler,
			ILog logger)
		{
			_tapiBridgeParametersFactory = factory.ThrowIfNull(nameof(factory));
			_tapiObjectService = tapiObjectService.ThrowIfNull(nameof(tapiObjectService));
			_downloadProgressManager = downloadProgressManager.ThrowIfNull(nameof(downloadProgressManager));
			_filesStatistics = filesStatistics.ThrowIfNull(nameof(filesStatistics));
			_messageHandler = messageHandler.ThrowIfNull(nameof(messageHandler));
			_transferClientHandler = transferClientHandler.ThrowIfNull(nameof(transferClientHandler));
			_logger = logger.ThrowIfNull(nameof(logger));
		}

		public int Count
		{
			get
			{
				lock (_syncRoot)
				{
					return _fileTapiBridges.Count;
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IDownloadTapiBridge Request(IRelativityFileShareSettings settings, CancellationToken token)
		{
			lock (_syncRoot)
			{
				// When an artifact cannot be mapped to a file share, a special bridge is used to guarantee success.
				if (settings == null)
				{
					// Passing in null is 100% supported.
					return _nullSettingsTapiBridge
					       ?? (_nullSettingsTapiBridge = this.CreateTapiBridge(null, token));
				}

				IDownloadTapiBridge bridge =
					this.GetPooledBridge(settings) ?? this.CreatePooledBridge(settings, token);
				return bridge;
			}
		}

		private IDownloadTapiBridge CreatePooledBridge(IRelativityFileShareSettings settings, CancellationToken token)
		{
			IDownloadTapiBridge bridge = this.CreateTapiBridge(settings, token);
			_fileTapiBridges[settings] = bridge;
			return bridge;			
		}

		private IDownloadTapiBridge CreateTapiBridge(IRelativityFileShareSettings settings, CancellationToken token)
		{
			ITapiBridgeFactory tapiBridgeFactory = new FilesTapiBridgeFactory(
				_tapiBridgeParametersFactory,
				_tapiObjectService,
				_logger,
				settings,
				token);
			ITapiBridge bridge = tapiBridgeFactory.Create();
			IProgressHandler progressHandler = new FileDownloadProgressHandler(_downloadProgressManager, _logger);
			IDownloadTapiBridge downloadTapiBridgeForFiles = new DownloadTapiBridgeForFiles(
				bridge,
				progressHandler,
				_messageHandler,
				_filesStatistics,
				_transferClientHandler,
				_logger);
			return downloadTapiBridgeForFiles;
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing">
		/// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
		/// </param>
		private void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				List<IDownloadTapiBridge> bridges = new List<IDownloadTapiBridge>(_fileTapiBridges.Values);
				if (_nullSettingsTapiBridge != null)
				{
					bridges.Add(_nullSettingsTapiBridge);
				}

				foreach (IDownloadTapiBridge bridge in bridges)
				{
					TryDisposeTapiBridge(bridge);
				}

				_fileTapiBridges.Clear();
				_nullSettingsTapiBridge = null;
			}

			_disposed = true;
		}

		private IDownloadTapiBridge GetPooledBridge(IRelativityFileShareSettings settings)
		{
			IDownloadTapiBridge bridge =
				_fileTapiBridges.TryGetValue(settings, out IDownloadTapiBridge entry) ? entry : null;
			return bridge;
		}

		private void TryDisposeTapiBridge(IDownloadTapiBridge bridge)
		{
			try
			{
				bridge?.Dispose();
			}
			catch (Exception e)
			{
				if (ExceptionHelper.IsFatalException(e))
				{
					throw;
				}

				// We do not want to crash container disposal
				_logger.LogError(e, "Failed to dispose Tapi Bridge.");
			}
		}
	}
}