using System;
using System.Threading;
using Relativity.Import.Export.Transfer;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class SmartTapiBridge : ITapiBridge
	{
		private int _downloadedFilesCounter;
		private int _downloadRequestCounter;
		private int _totalFilesDownloadedUsingTapiBridge;
		private ITapiBridgeWrapper _tapiBridge;
		private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true);
		private readonly CancellationToken _token;
		private readonly int _maximumFilesForTapiBridge;
		private readonly ITapiBridgeWrapperFactory _tapiBridgeWrapperFactory;
		private readonly ILog _logger;
		private readonly object _lockToken = new object();
		private readonly TimeSpan _tapiBridgeExportTransferWaitingTime;

		public event EventHandler<TapiMessageEventArgs> TapiStatusMessage;

		public event EventHandler<TapiMessageEventArgs> TapiErrorMessage;

		public event EventHandler<TapiMessageEventArgs> TapiWarningMessage;

		public event EventHandler<TapiClientEventArgs> TapiClientChanged;

		public event EventHandler<TapiProgressEventArgs> TapiProgress;

		public event EventHandler<TapiStatisticsEventArgs> TapiStatistics;

		public event EventHandler<TapiMessageEventArgs> TapiFatalError;

		public SmartTapiBridge(
			IExportConfig exportConfig,
			ITapiBridgeWrapperFactory tapiBridgeWrapperFactory,
			ILog logger,
			CancellationToken token)
		{
			_tapiBridgeWrapperFactory = tapiBridgeWrapperFactory;
			_logger = logger;
			_token = token;
			_tapiBridgeExportTransferWaitingTime =
				TimeSpan.FromSeconds(exportConfig.TapiBridgeExportTransferWaitingTimeInSeconds);
			_maximumFilesForTapiBridge = exportConfig.MaximumFilesForTapiBridge;
		}

		public TapiClient ClientType => _tapiBridge?.ClientType ?? TapiClient.None;

		public string AddPath(TransferPath transferPath)
		{
			if (_tapiBridge == null)
			{
				CreateTapiBridge();
			}

			_downloadRequestCounter++;
			return _tapiBridge.AddPath(transferPath);
		}

		private void RemoveTapiBridge()
		{
			if (_tapiBridge == null)
			{
				return;
			}

			_tapiBridge.TapiProgress -= OnTapiBridgeProgress;
			_tapiBridge.TapiClientChanged -= OnTapiBridgeClientChanged;
			_tapiBridge.TapiErrorMessage -= OnTapiBridgeErrorMessage;
			_tapiBridge.TapiFatalError -= OnTapiBridgeFatalError;
			_tapiBridge.TapiStatistics -= OnTapiBridgeStatistics;
			_tapiBridge.TapiStatusMessage -= OnTapiBridgeStatusMessage;
			_tapiBridge.TapiWarningMessage -= OnTapiBridgeWarningMessage;
			_tapiBridge.Dispose();
			_tapiBridge = null;
		}

		private void CreateTapiBridge()
		{
			_tapiBridge = _tapiBridgeWrapperFactory.Create();
			_tapiBridge.TapiProgress += OnTapiBridgeProgress;
			_tapiBridge.TapiClientChanged += OnTapiBridgeClientChanged;
			_tapiBridge.TapiErrorMessage += OnTapiBridgeErrorMessage;
			_tapiBridge.TapiFatalError += OnTapiBridgeFatalError;
			_tapiBridge.TapiStatistics += OnTapiBridgeStatistics;
			_tapiBridge.TapiStatusMessage += OnTapiBridgeStatusMessage;
			_tapiBridge.TapiWarningMessage += OnTapiBridgeWarningMessage;
			_totalFilesDownloadedUsingTapiBridge = 0;
		}

		private void OnTapiBridgeProgress(object sender, TapiProgressEventArgs e)
		{
			lock (_lockToken)
			{
				if (e.DidTransferSucceed)
				{
					_downloadedFilesCounter++;
					if (_downloadedFilesCounter == _downloadRequestCounter)
					{
						// The handle only gets released when ALL files are transferred.
						_autoResetEvent.Set();
					}
				}
			}

			TapiProgress?.Invoke(sender, e);
		}

		private void OnTapiBridgeWarningMessage(object sender, TapiMessageEventArgs e)
		{
			TapiWarningMessage?.Invoke(sender, e);
		}

		private void OnTapiBridgeStatusMessage(object sender, TapiMessageEventArgs e)
		{
			TapiStatusMessage?.Invoke(sender, e);
		}

		private void OnTapiBridgeStatistics(object sender, TapiStatisticsEventArgs e)
		{
			TapiStatistics?.Invoke(sender, e);
		}

		private void OnTapiBridgeFatalError(object sender, TapiMessageEventArgs e)
		{
			TapiFatalError?.Invoke(sender, e);
		}

		private void OnTapiBridgeErrorMessage(object sender, TapiMessageEventArgs e)
		{
			TapiErrorMessage?.Invoke(sender, e);
		}

		private void OnTapiBridgeClientChanged(object sender, TapiClientEventArgs e)
		{
			TapiClientChanged?.Invoke(sender, e);
		}

		public void WaitForTransferJob()
		{
			if (_tapiBridge == null)
			{
				throw new InvalidOperationException(
					"This operation cannot be performed because an unexpected failure occurred waiting for all file transfers to complete. " +
					"Try again. If the problem persists, please contact your system administrator for assistance.");
			}

			lock (_lockToken)
			{
				if (_downloadRequestCounter != _downloadedFilesCounter)
				{
					_autoResetEvent.Reset();
				}
			}

			if (!_autoResetEvent.WaitOne(_tapiBridgeExportTransferWaitingTime, _token))
			{
				_logger.LogInformation("The TAPI bridge WaitOne call exceeded the maximum transfer timeout {TimeoutSeconds} seconds.",
					_tapiBridgeExportTransferWaitingTime.TotalSeconds);
				_tapiBridge.WaitForTransferJob();
				RemoveTapiBridge();
			}

			lock (_lockToken)
			{
				_totalFilesDownloadedUsingTapiBridge += _downloadedFilesCounter;
				_downloadedFilesCounter = 0;
				_downloadRequestCounter = 0;
				if (_totalFilesDownloadedUsingTapiBridge < _maximumFilesForTapiBridge)
				{
					_logger.LogInformation("All export files have been successfully downloaded.");
					return;
				}
			}

			if (_tapiBridge != null)
			{
				_logger.LogInformation("Awaiting for all remaining files to download.");
				_tapiBridge.WaitForTransferJob();
				RemoveTapiBridge();
			}
			else
			{
				_logger.LogWarning(
					"There are remaining files to download but the TAPI bridge is null. Assuming these files failed to transfer.");
			}
		}

		public void Disconnect()
		{
			RemoveTapiBridge();
		}

		public void Dispose()
		{
			RemoveTapiBridge();
		}
	}
}