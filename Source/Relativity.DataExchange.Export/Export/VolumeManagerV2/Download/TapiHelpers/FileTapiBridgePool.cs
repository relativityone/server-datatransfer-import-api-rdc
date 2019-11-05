namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using Relativity.DataExchange.Transfer;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Resources;
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
		private readonly bool _forceCreateTransferClient;
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
			: this(
				factory,
				tapiObjectService,
				downloadProgressManager,
				filesStatistics,
				messageHandler,
				transferClientHandler,
				logger,
				forceCreateTransferClient: true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileTapiBridgePool"/> class. This is used exclusively for testing.
		/// </summary>
		/// <param name="factory">
		/// The factory used to create transfer bridge parameters.
		/// </param>
		/// <param name="tapiObjectService">
		/// The transfer object service.
		/// </param>
		/// <param name="downloadProgressManager">
		/// The object that manages download progress.
		/// </param>
		/// <param name="filesStatistics">
		/// The files statistics instance.
		/// </param>
		/// <param name="messageHandler">
		/// The object that handles common export messages.
		/// </param>
		/// <param name="transferClientHandler">
		/// The object that handles transfer client changes.
		/// </param>
		/// <param name="logger">
		/// The logger instance.
		/// </param>
		/// <param name="forceCreateTransferClient">
		/// <see langword="true" /> to force create the transfer client and raise the client change event immediately ; otherwise, <see langword="false" /> for deferred construction.
		/// </param>
		internal FileTapiBridgePool(
			TapiBridgeParametersFactory factory,
			ITapiObjectService tapiObjectService,
			DownloadProgressManager downloadProgressManager,
			FilesStatistics filesStatistics,
			IMessagesHandler messageHandler,
			ITransferClientHandler transferClientHandler,
			ILog logger,
			bool forceCreateTransferClient)
		{
			_tapiBridgeParametersFactory = factory.ThrowIfNull(nameof(factory));
			_tapiObjectService = tapiObjectService.ThrowIfNull(nameof(tapiObjectService));
			_downloadProgressManager = downloadProgressManager.ThrowIfNull(nameof(downloadProgressManager));
			_filesStatistics = filesStatistics.ThrowIfNull(nameof(filesStatistics));
			_messageHandler = messageHandler.ThrowIfNull(nameof(messageHandler));
			_transferClientHandler = transferClientHandler.ThrowIfNull(nameof(transferClientHandler));
			_logger = logger.ThrowIfNull(nameof(logger));
			_forceCreateTransferClient = forceCreateTransferClient;
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

			// Only bypassed for unit tests because the bridge isn't mocked.
			if (_forceCreateTransferClient)
			{
				bridge.CreateTransferClient();
			}

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

				List<Exception> exceptions = new List<Exception>();
				foreach (IDownloadTapiBridge bridge in bridges)
				{
					try
					{
						bridge?.Dispose();
					}
					catch (Exception e)
					{
						exceptions.Add(e);
					}
				}

				_fileTapiBridges.Clear();
				_nullSettingsTapiBridge = null;
				if (exceptions.Count > 0)
				{
					AggregateException exception = new AggregateException(
						ExportStrings.TransferPoolDisposeExceptionMessage,
						exceptions);
					_logger.LogError(
						exception,
						"The transfer bridge pool failed to dispose one or more transfer bridges.");
					throw exception;
				}
			}

			_disposed = true;
		}

		private IDownloadTapiBridge GetPooledBridge(IRelativityFileShareSettings settings)
		{
			IDownloadTapiBridge bridge =
				_fileTapiBridges.TryGetValue(settings, out IDownloadTapiBridge entry) ? entry : null;
			return bridge;
		}
	}
}