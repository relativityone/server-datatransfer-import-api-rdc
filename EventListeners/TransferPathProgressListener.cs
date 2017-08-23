// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferPathProgressListener.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.IO;

    using Relativity.Transfer;

    /// <summary>
    /// Listens for transfer path progress events.
    /// </summary>
    public class TransferPathProgressListener : TransferListenerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferPathProgressListener"/> class. 
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="context">
        /// The transfer context.
        /// </param>
        public TransferPathProgressListener(ILog log, TransferContext context)
            : base(log, context)
        {
        }

        /// <summary>
        /// Occurs when a file has finished transferring.
        /// </summary>
        public event EventHandler<TapiProgressEventArgs> ProgressEvent = delegate { };

        /// <inheritdoc />
        protected override void OnLargeFileProgress(object sender, LargeFileProgressEventArgs e)
        {
            base.OnLargeFileProgress(sender, e);
            this.RaiseStatusMessage($"Trip {e.ChunkNumber} of {e.TotalChunks}", e.Path.Order);
        }

        /// <inheritdoc />
        protected override void OnTransferPathProgress(object sender, TransferPathProgressEventArgs e)
        {
            if (e.Status != TransferPathStatus.Successful)
            {
                return;
            }

            // Guard against null timestamps.
            var args = new TapiProgressEventArgs(
                !string.IsNullOrEmpty(e.Path.TargetFileName)
                    ? e.Path.TargetFileName
                    : Path.GetFileName(e.Path.SourcePath),
                e.Status == TransferPathStatus.Successful,
                e.Path.Order,
                e.BytesTransferred,
                e.StartTime ?? DateTime.Now,
                e.EndTime ?? DateTime.Now);
            this.ProgressEvent.Invoke(this, args);
        }
    }
}