﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	/// <summary>
	/// Reports an error to the progress stream for each attempted transfer.
	/// Used as a default <see cref="ITapiBridge"/> implementation when one
	/// cannot normally be created.
	/// </summary>
	public class ErrorReportingTapiBridge : ITapiBridge
	{
		private readonly TapiTotals tapiTotals = new TapiTotals();

		public TapiClient Client => TapiClient.None;

		public TapiTotals JobTotals => this.tapiTotals;

		public long TotalJobCompletedFileTransfers => 0;

		public event EventHandler<TapiClientEventArgs> TapiClientChanged;
		public event EventHandler<TapiMessageEventArgs> TapiErrorMessage;
		public event EventHandler<TapiMessageEventArgs> TapiFatalError;
		public event EventHandler<TapiProgressEventArgs> TapiProgress;
		public event EventHandler<TapiStatisticsEventArgs> TapiStatistics;
		public event EventHandler<TapiMessageEventArgs> TapiStatusMessage;
		public event EventHandler<TapiMessageEventArgs> TapiWarningMessage;
		public event EventHandler<TapiLargeFileProgressEventArgs> TapiLargeFileProgress;

		public string AddPath(TransferPath transferPath)
		{
			this.tapiTotals.IncrementTotalFileTransferRequests();
			var progressArgs = new TapiProgressEventArgs(
				!string.IsNullOrEmpty(transferPath.TargetFileName)
					? transferPath.TargetFileName
					: System.IO.Path.GetFileName(transferPath.SourcePath),
				false,
				false,
				transferPath.Order,
				transferPath.Bytes,
				DateTime.Now,
				DateTime.Now);

			this.TapiProgress?.Invoke(this, progressArgs);
			return transferPath.TargetFileName;
		}

		public void Disconnect()
		{
		}

		public void Dispose()
		{
		}

		public TapiTotals WaitForTransfers(string startMessage, string successMessage, string errorMessage, bool optimized)
		{
			return this.tapiTotals;
		}
	}
}