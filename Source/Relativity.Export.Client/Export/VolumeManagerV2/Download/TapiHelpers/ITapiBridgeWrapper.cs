﻿using System;
using Relativity.Import.Export.Transfer;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface ITapiBridgeWrapper : IDisposable
	{
		event EventHandler<TapiMessageEventArgs> TapiStatusMessage;
		event EventHandler<TapiMessageEventArgs> TapiErrorMessage;
		event EventHandler<TapiMessageEventArgs> TapiWarningMessage;
		event EventHandler<TapiClientEventArgs> TapiClientChanged;
		event EventHandler<TapiProgressEventArgs> TapiProgress;
		event EventHandler<TapiStatisticsEventArgs> TapiStatistics;
		event EventHandler<TapiMessageEventArgs> TapiFatalError;
		TapiClient ClientType { get; }

		string AddPath(TransferPath transferPath);
		void WaitForTransferJob();
	}
}