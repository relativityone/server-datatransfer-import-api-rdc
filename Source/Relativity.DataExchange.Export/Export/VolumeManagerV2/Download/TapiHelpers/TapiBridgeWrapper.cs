﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using global::Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using global::Relativity.Transfer;

	using Relativity.DataExchange.Transfer;

	public class TapiBridgeWrapper : ITapiBridgeWrapper
	{
		private readonly TapiBridgeBase2 _tapiBridge;

		public TapiBridgeWrapper(TapiBridgeBase2 tapiBridge)
		{
			_tapiBridge = tapiBridge;
		}

		public event EventHandler<TapiMessageEventArgs> TapiStatusMessage
		{
			add { _tapiBridge.TapiStatusMessage += value; }
			remove { _tapiBridge.TapiStatusMessage -= value; }
		}

		public event EventHandler<TapiMessageEventArgs> TapiErrorMessage
		{
			add { _tapiBridge.TapiErrorMessage += value; }
			remove { _tapiBridge.TapiErrorMessage -= value; }
		}

		public event EventHandler<TapiMessageEventArgs> TapiWarningMessage
		{
			add { _tapiBridge.TapiWarningMessage += value; }
			remove { _tapiBridge.TapiWarningMessage -= value; }
		}

		public event EventHandler<TapiClientEventArgs> TapiClientChanged
		{
			add { _tapiBridge.TapiClientChanged += value; }
			remove { _tapiBridge.TapiClientChanged -= value; }
		}

		public event EventHandler<TapiProgressEventArgs> TapiProgress
		{
			add { _tapiBridge.TapiProgress += value; }
			remove { _tapiBridge.TapiProgress -= value; }
		}

		public event EventHandler<TapiStatisticsEventArgs> TapiStatistics
		{
			add { _tapiBridge.TapiStatistics += value; }
			remove { _tapiBridge.TapiStatistics -= value; }
		}

		public event EventHandler<TapiMessageEventArgs> TapiFatalError
		{
			add { _tapiBridge.TapiFatalError += value; }
			remove { _tapiBridge.TapiFatalError -= value; }
		}

		public TapiClient ClientType => _tapiBridge.Client;

		public string AddPath(TransferPath transferPath)
		{
			return _tapiBridge.AddPath(transferPath);
		}

		public void WaitForTransferJob()
		{
			_tapiBridge.WaitForTransferJob();
		}

		public void Dispose()
		{
			_tapiBridge.Dispose();
		}
	}
}