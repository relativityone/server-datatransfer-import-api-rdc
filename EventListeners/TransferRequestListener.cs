// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferRequestListener.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using kCura.WinEDDS.TApi.Resources;

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
            // Note: due to RDC's coupling of messages to lines, sending all messages to just the transfer log.        
            switch (e.Status)
            {
                case TransferRequestStatus.Started:
                    this.TransferLog.LogInformation(Strings.TransferJobStartedMessage);
                    break;

                case TransferRequestStatus.Ended:
                    this.TransferLog.LogInformation(Strings.TransferJobEndedMessage);
                    break;

                case TransferRequestStatus.EndedMaxRetry:
                    this.TransferLog.LogInformation(Strings.TransferJobEndedMaxRetryMessage);
                    break;

                case TransferRequestStatus.Canceled:
                    this.TransferLog.LogInformation(Strings.TransferJobCanceledMessage);
                    break;
            }
        }
    }
}