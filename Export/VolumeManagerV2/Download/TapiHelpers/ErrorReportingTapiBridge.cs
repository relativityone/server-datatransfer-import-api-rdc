using System;
using kCura.WinEDDS.TApi;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	/// <summary>
	/// Reports an error to the progress stream for each attempted transfer.
	/// Used as a default <see cref="ITapiBridge"/> implementation when one
	/// cannot normally be created.
	/// </summary>
	public class ErrorReportingTapiBridge : ITapiBridge
	{
		// TODO: Set this to correct client
		public TapiClient ClientType => TapiClient.None;

		public event EventHandler<TapiClientEventArgs> TapiClientChanged;
		public event EventHandler<TapiMessageEventArgs> TapiErrorMessage;
		public event EventHandler<TapiMessageEventArgs> TapiFatalError;
		public event EventHandler<TapiProgressEventArgs> TapiProgress;
		public event EventHandler<TapiStatisticsEventArgs> TapiStatistics;
		public event EventHandler<TapiMessageEventArgs> TapiStatusMessage;
		public event EventHandler<TapiMessageEventArgs> TapiWarningMessage;

		public string AddPath(TransferPath transferPath)
		{
			var progressArgs = new TapiProgressEventArgs(
				!string.IsNullOrEmpty(transferPath.TargetFileName)
					? transferPath.TargetFileName
					: System.IO.Path.GetFileName(transferPath.SourcePath),
				false,
				TransferPathStatus.BadPathError,
				transferPath.Order,
				transferPath.Bytes,
				DateTime.UtcNow,
				DateTime.UtcNow);

			TapiProgress?.Invoke(this, progressArgs);

			return transferPath.TargetFileName;
		}

		public void Disconnect()
		{
		}

		public void Dispose()
		{
		}

		public void WaitForTransferJob()
		{
		}
	}
}
