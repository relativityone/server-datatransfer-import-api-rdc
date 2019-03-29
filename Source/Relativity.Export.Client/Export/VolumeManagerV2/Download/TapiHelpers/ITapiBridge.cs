namespace Relativity.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using Relativity.Transfer;
	using Relativity.Import.Export.Transfer;

	public interface ITapiBridge : IDisposable
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
		void Disconnect();
	}
}