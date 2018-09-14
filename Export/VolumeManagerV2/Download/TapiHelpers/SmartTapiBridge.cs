using System;
using System.Threading;
using kCura.WinEDDS.TApi;
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
		private readonly ITapiBridgeWrapperFactory _tapiBridgeWrapperFactory;
		private readonly CancellationToken _token;
		private readonly object _lockToken = new object();
		private readonly TimeSpan _tapiBridgeExportTransferWaitingTime;

		public event EventHandler<TapiMessageEventArgs> TapiStatusMessage;

		public event EventHandler<TapiMessageEventArgs> TapiErrorMessage;

		public event EventHandler<TapiMessageEventArgs> TapiWarningMessage;

		public event EventHandler<TapiClientEventArgs> TapiClientChanged;

		public event EventHandler<TapiProgressEventArgs> TapiProgress;

		public event EventHandler<TapiStatisticsEventArgs> TapiStatistics;

		public event EventHandler<TapiMessageEventArgs> TapiFatalError;

		public SmartTapiBridge(IExportConfig exportConfig, ITapiBridgeWrapperFactory tapiBridgeWrapperFactory,
			CancellationToken token)
		{
			_tapiBridgeWrapperFactory = tapiBridgeWrapperFactory;
			_token = token;
			_tapiBridgeExportTransferWaitingTime =
				TimeSpan.FromSeconds(exportConfig.TapiBridgeExportTransferWaitingTimeInSeconds);
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
			if (e.DidTransferSucceed)
			{
				lock (_lockToken)
				{
					_downloadedFilesCounter++;
					if (_downloadedFilesCounter == _downloadRequestCounter)
					{
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
			lock (_lockToken)
			{
				if (_downloadRequestCounter != _downloadedFilesCounter)
				{
					_autoResetEvent.Reset();
				}
			}

			if(!_autoResetEvent.WaitOne(_tapiBridgeExportTransferWaitingTime, _token))
			{
				_tapiBridge.WaitForTransferJob();
				RemoveTapiBridge();
			}

			_totalFilesDownloadedUsingTapiBridge += _downloadedFilesCounter;
			_downloadedFilesCounter = 0;
			_downloadRequestCounter = 0;

			if (_totalFilesDownloadedUsingTapiBridge > 10000)
			{
				RemoveTapiBridge();
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