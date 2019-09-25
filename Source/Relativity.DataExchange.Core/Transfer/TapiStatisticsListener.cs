// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiStatisticsListener.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Collections.Concurrent;
	using System.Globalization;
	using System.Linq;

	using Relativity.Transfer;

	/// <summary>
	/// Represents an object that listens for Transfer API statistics events.
	/// </summary>
	internal sealed class TapiStatisticsListener : TapiListenerBase
	{
		/// <summary>
		/// Number of bytes in a megabit. Used for units conversion.
		/// </summary>
		private const double BytesPerMegabit = 125_000.0;

		/// <summary>
		/// The thread synchronization root backing.
		/// </summary>
		private static readonly object SyncRoot = new object();

		/// <summary>
		/// The total statistics for all jobs.
		/// </summary>
		private readonly ConcurrentDictionary<Guid, TransferStatisticsEventArgs> totalStatistics =
			new ConcurrentDictionary<Guid, TransferStatisticsEventArgs>();

		/// <summary>
		/// The logging timestamp.
		/// </summary>
		private DateTime? logTimestamp;

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiStatisticsListener"/> class.
		/// </summary>
		/// <param name="log">
		/// The transfer log.
		/// </param>
		/// <param name="context">
		/// The transfer context.
		/// </param>
		public TapiStatisticsListener(ITransferLog log, TransferContext context)
			: base(log, context)
		{
		}

		/// <summary>
		/// The statistics event.
		/// </summary>
		public event EventHandler<TapiStatisticsEventArgs> StatisticsEvent;

		/// <summary>
		/// Gets or sets the log timestamp.
		/// </summary>
		/// <value>
		/// The <see cref="DateTime"/> value.
		/// </value>
		private DateTime? LogTimestamp
		{
			get
			{
				lock (SyncRoot)
				{
					return this.logTimestamp;
				}
			}

			set
			{
				lock (SyncRoot)
				{
					this.logTimestamp = value;
				}
			}
		}

		/// <inheritdoc />
		protected override void OnTransferStatisticsEvent(object sender, TransferStatisticsEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException(nameof(e));
			}

			var key = e.Request.JobId.Value;
			if (!this.totalStatistics.ContainsKey(key))
			{
				this.totalStatistics.TryAdd(key, e);
			}
			else
			{
				this.totalStatistics[key] = e;
			}

			var totalTransferredBytes = this.totalStatistics.Sum(x => x.Value.Statistics.TotalTransferredBytes);
			var totalTransferredFiles = this.totalStatistics.Sum(x => x.Value.Statistics.TotalTransferredFiles);
			var totalTransferTicks = TimeSpan
				.FromSeconds(this.totalStatistics.Sum(x => x.Value.Statistics.TransferTimeSeconds)).Ticks;
			var transferRateBytesPerSecond = e.Statistics.TransferRateMbps * BytesPerMegabit;
			var args = new TapiStatisticsEventArgs(totalTransferredBytes, totalTransferredFiles, totalTransferTicks, transferRateBytesPerSecond);
			this.StatisticsEvent?.Invoke(this, args);

			// Be careful with overly aggressive statistics logging.
			const int LogStatisticsRateSeconds = 15;
			var logTimestampCopy = this.LogTimestamp;
			var logStatistics = logTimestampCopy == null
								|| !((DateTime.Now - logTimestampCopy.Value).TotalSeconds <= LogStatisticsRateSeconds);
			if (!logStatistics)
			{
				return;
			}

			var totalSeconds = totalTransferTicks / TimeSpan.TicksPerSecond;
			if (totalSeconds > 0)
			{
				var aggregateDataRate = totalTransferredBytes / totalSeconds;
				var aggregateMessage = string.Format(
					CultureInfo.CurrentCulture,
					"WinEDDS aggregate statistics: {0}/sec",
					ToFileSize(aggregateDataRate));
				this.TransferLog.LogTransferInformation(e.Request, aggregateMessage);
			}

			var jobMessage = string.Format(
				CultureInfo.CurrentCulture,
				"WinEDDS job {0} statistics - Files: {1}/{2} - Progress: {3:0.00}% - Rate: {4:0.00}/sec",
				e.Request.JobId,
				e.Statistics.TotalTransferredFiles,
				e.Statistics.TotalRequestFiles,
				e.Statistics.Progress,
				ToFileSize(transferRateBytesPerSecond));
			this.TransferLog.LogTransferInformation(e.Request, jobMessage);
			this.LogTimestamp = DateTime.Now;
		}

		/// <summary>
		/// Converts the byte size into a standard file size.
		/// </summary>
		/// <param name="value">
		/// The value.
		/// </param>
		/// <returns>
		/// The file size.
		/// </returns>
		private static string ToFileSize(double value)
		{
			var prefix = string.Empty;
			var k = value <= 0 ? 0 : (int)Math.Floor(System.Math.Log(value, 1000.0));
			switch (k)
			{
				case 0:
					prefix = string.Empty;
					break;

				case 1:
					prefix = "K";
					break;

				case 2:
					prefix = "M";
					break;

				case 3:
					prefix = "G";
					break;

				case 4:
					prefix = "T";
					break;

				case 5:
					prefix = "P";
					break;
			}

			return (value / Math.Pow(1000, k)).ToString("N2") + $" {prefix}B";
		}
	}
}