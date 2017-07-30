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
        public TransferStatisticsListener(ILog log)
            : base(log)
        {
        }

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
            var key = e.Request.TransferId.Value;
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
                .FromSeconds(this.totalStatistics.Sum(x => x.Value.Statistics.TransferTimeSeconds))
                .Ticks;
            var args = new TapiStatisticsEventArgs(
                totalTransferredBytes,
                totalTransferredFiles,
                Convert.ToInt64(totalTransferTicks));
            this.StatisticsEvent.Invoke(this, args);
            var progressMessage = string.Format(
                CultureInfo.CurrentCulture,
                "Transferring {0}/{1} - {2:0.00}% - Rate: {3:0.00} Mbps",
                e.Statistics.TotalTransferredFiles,
                e.Statistics.TotalFiles,
                e.Statistics.Progress,
                e.Statistics.TransferRateMbps);
            this.TransferLog.LogInformation(progressMessage);
        }
    }
}