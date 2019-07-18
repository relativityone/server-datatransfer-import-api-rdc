namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	public class EmptyTapiBridge : ITapiBridge
	{
		private readonly ITapiBridge _tapiBridge;

		public EmptyTapiBridge(ITapiBridge tapiBridge)
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

		public event EventHandler<TapiLargeFileProgressEventArgs> TapiLargeFileProgress
		{
			add { _tapiBridge.TapiLargeFileProgress += value; }
			remove { _tapiBridge.TapiLargeFileProgress -= value; }
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

		public TapiClient Client => _tapiBridge.Client;

		public TapiTotals JobTotals => this._tapiBridge.JobTotals;

		public string AddPath(TransferPath transferPath)
		{
			return _tapiBridge.AddPath(transferPath);
		}

		public TapiTotals WaitForTransfers(string waitMessage, string successMessage, string errorMessage, bool keepJobAlive)
		{
			return _tapiBridge.WaitForTransfers(waitMessage, successMessage, errorMessage, keepJobAlive);
		}

		public void Disconnect()
		{
		}

		public void Dispose()
		{
			_tapiBridge.Dispose();
		}
	}
}