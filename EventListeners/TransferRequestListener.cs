// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferRequestListener.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using kCura.WinEDDS.TApi.Resources;

    using Relativity.Logging;
    using Relativity.Transfer;

    /// <summary>
    /// Listens for transfer request events.
    /// </summary>
    public class TransferRequestListener : TransferListenerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferRequestListener"/> class. 
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        public TransferRequestListener(ILog log)
            : base(log)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferRequestListener"/> class. 
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="context">
        /// The transfer context.
        /// </param>
        public TransferRequestListener(ILog log, TransferContext context)
            : base(log, context)
        {
        }

        /// <inheritdoc />
        protected override void OnTransferRequestEvent(object sender, TransferRequestEventArgs e)
        {
            switch (e.Status)
            {
                case TransferRequestStatus.Started:
                    this.RaiseStatusMessage(Strings.TransferJobStartedMessage);
                    break;

                case TransferRequestStatus.Ended:
                    this.RaiseStatusMessage(Strings.TransferJobEndedMessage);
                    break;

                case TransferRequestStatus.EndedMaxRetry:
                    this.RaiseStatusMessage(Strings.TransferJobEndedMaxRetryMessage);
                    break;

                case TransferRequestStatus.Canceled:
                    this.RaiseStatusMessage(Strings.TransferJobCanceledMessage);
                    break;
            }
        }
    }
}
