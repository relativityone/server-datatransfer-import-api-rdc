// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferStatisticsListener.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Globalization;

    using kCura.WinEDDS.TApi.Resources;

    using Relativity.Logging;
    using Relativity.Transfer;

    /// <summary>
    /// Listens for transfer statistics events.
    /// </summary>
    public class TransferStatisticsListener : TransferListenerBase
    {
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
        public event EventHandler<TransferStatisticsAvailableEventArgs> StatisticsEvent = delegate { };


        /// <inheritdoc />
        protected override void OnTransferStatisticsEvent(object sender, TransferStatisticsEventArgs e)
        {
            var progressMessage = string.Format(
                CultureInfo.CurrentCulture,
                Strings.ProgressMessage,
                e.Statistics.TotalTransferredFiles,
                e.Statistics.TotalFiles,
                e.Statistics.Progress);
            this.RaiseStatusMessage(progressMessage);
            this.RaiseStatisticsEvent(e.Statistics);
        }

        /// <summary>
        /// The raise statistics event.
        /// </summary>
        /// <param name="statistics">
        /// The statistics.
        /// </param>
        private void RaiseStatisticsEvent(ITransferStatistics statistics)
        {
            this.StatisticsEvent.Invoke(this, new TransferStatisticsAvailableEventArgs(statistics));
        }
    }
}
