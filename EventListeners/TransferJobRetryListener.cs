// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferJobRetryListener.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System.Globalization;

    using kCura.WinEDDS.TApi.Resources;

    using Relativity.Logging;
    using Relativity.Transfer;

    /// <summary>
    /// The transfer job retry listener.
    /// </summary>
    public class TransferJobRetryListener : TransferListenerBase
    {
        /// <summary>
        /// The max retry count.
        /// </summary>
        private readonly int maxRetryCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferJobRetryListener"/> class.
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="maxRetryCount">
        /// The max retry count.
        /// </param>
        public TransferJobRetryListener(ILog log, int maxRetryCount) : base(log)
        {
            this.maxRetryCount = maxRetryCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferJobRetryListener"/> class.
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="maxRetryCount">
        /// The max retry count.
        /// </param>
        /// <param name="context">
        /// The transfer context.
        /// </param>
        public TransferJobRetryListener(ILog log, int maxRetryCount, TransferContext context) : base(log, context)
        {
            this.maxRetryCount = maxRetryCount;
        }

        /// <inheritdoc />
        protected override void OnTransferJobRetryEvent(object sender, TransferJobRetryEventArgs e)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                Strings.RetryJobMessage,
                e.Count,
                this.maxRetryCount);
            this.RaiseStatusMessage(message, 0);
        }
    }
}
