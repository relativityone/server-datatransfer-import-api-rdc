// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferStatisticsListener.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.Linq;

    using kCura.WinEDDS.TApi.Resources;

    using Relativity.Logging;
    using Relativity.Transfer;

    /// <summary>
    /// Listens for transfer statistics events.
    /// </summary>
    public class TransferStatisticsListener : TransferListenerBase
    {
        /// <summary>
        /// The total statistics for all jobs.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, TransferStatisticsEventArgs> totalStatistics =
            new ConcurrentDictionary<Guid, TransferStatisticsEventArgs>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferStatisticsListener"/> class. 
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="context">
        /// The transfer context.
        /// </param>
        public TransferStatisticsListener(ILog log, TransferContext context)
            : base(log, context)
        {
        }

        /// <summary>
        /// The statistics event.
        /// </summary>
        public event EventHandler<TapiStatisticsEventArgs> StatisticsEvent = delegate { };


        /// <inheritdoc />
        protected override void OnTransferStatisticsEvent(object sender, TransferStatisticsEventArgs e)
        {
            var key = e.Request.JobId.Value;
            if (!this.totalStatistics.ContainsKey(key))
            {
                this.totalStatistics.TryAdd(key, e);
            }
            else
            {
                this.totalStatistics[key] = e;
            }

            const int TicksPerSecond = 10000000;
            var totalTransferredBytes = this.totalStatistics.Sum(x => x.Value.Statistics.TotalTransferredBytes);
            var totalTransferredFiles = this.totalStatistics.Sum(x => x.Value.Statistics.TotalTransferredFiles);
            var totalTransferTicks = TimeSpan
                .FromSeconds(this.totalStatistics.Sum(x => x.Value.Statistics.TransferTimeSeconds)).Ticks;
            var args = new TapiStatisticsEventArgs(totalTransferredBytes, totalTransferredFiles, totalTransferTicks);
            this.StatisticsEvent.Invoke(this, args);
            var totalSeconds = totalTransferTicks / TicksPerSecond;
            if (totalSeconds > 0)
            {
                var aggregateDataRate = totalTransferredBytes / totalSeconds;
                var aggregateMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "WinEDDS aggregate statistics: {0}/sec",
                    ToFileSize(aggregateDataRate));
                this.TransferLog.LogInformation(aggregateMessage);
            }

            var jobMessage = string.Format(
                CultureInfo.CurrentCulture,
                "TAPI job {0} statistics - Files: {1}/{2} - Progress: {3:0.00}% - Rate: {4:0.00}/sec",
                e.Request.JobId,
                e.Statistics.TotalTransferredFiles,
                e.Statistics.TotalFiles,
                e.Statistics.Progress,
                ToFileSize((e.Statistics.TransferRateMbps * 1000000) / 8.0));
            this.TransferLog.LogInformation(jobMessage);
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