// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiPathProgressListener.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
	using System;
	using System.IO;

	using Relativity.Transfer;

	/// <summary>
	/// Represents an object that listens for Transfer API path progress events.
	/// </summary>
	public sealed class TapiPathProgressListener : TapiListenerBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiPathProgressListener"/> class.
		/// </summary>
		/// <param name="log">
		/// The transfer log.
		/// </param>
		/// <param name="context">
		/// The transfer context.
		/// </param>
		public TapiPathProgressListener(ITransferLog log, TransferContext context)
			: base(log, context)
		{
		}

		/// <summary>
		/// Occurs when a file has finished transferring.
		/// </summary>
		public event EventHandler<TapiProgressEventArgs> ProgressEvent;

		/// <inheritdoc />
		protected override void OnLargeFileProgress(object sender, LargeFileProgressEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException(nameof(e));
			}

			base.OnLargeFileProgress(sender, e);
			if (e.TotalChunks > 0)
			{
				this.PublishStatusMessage(
					$"Large file transfer progress: {(int)(((double)e.ChunkNumber / (double)e.TotalChunks) * 100)}%.",
					e.Path.Order);
			}
		}

		/// <inheritdoc />
		protected override void OnTransferPathProgress(object sender, TransferPathProgressEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException(nameof(e));
			}

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
				e.Status,
				e.Path.Order,
				e.BytesTransferred,
				e.StartTime ?? DateTime.Now,
				e.EndTime ?? DateTime.Now);

			this.ProgressEvent?.Invoke(this, args);
		}
	}
}