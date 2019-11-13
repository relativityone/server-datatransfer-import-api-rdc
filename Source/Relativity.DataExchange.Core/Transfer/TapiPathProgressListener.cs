// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiPathProgressListener.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.IO;

	using Relativity.Logging;
	using Relativity.Transfer;

	/// <summary>
	/// Represents an object that listens for Transfer API path progress events. This class cannot be inherited.
	/// </summary>
	internal sealed class TapiPathProgressListener : TapiListenerBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiPathProgressListener"/> class.
		/// </summary>
		/// <param name="logger">
		/// The Relativity logger instance.
		/// </param>
		/// <param name="context">
		/// The transfer context.
		/// </param>
		public TapiPathProgressListener(ILog logger, TransferContext context)
			: base(logger, context)
		{
		}

		/// <summary>
		/// Occurs when a file has finished transferring.
		/// </summary>
		public event EventHandler<TapiProgressEventArgs> ProgressEvent;

		/// <summary>
		/// Occurs when a large file is transferring.
		/// </summary>
		public event EventHandler<TapiLargeFileProgressEventArgs> LargeFileProgressEvent;

		/// <inheritdoc />
		protected override void OnLargeFileProgress(object sender, LargeFileProgressEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException(nameof(e));
			}

			base.OnLargeFileProgress(sender, e);
			if (e.Progress > 0)
			{
				string filename = e.Path.Direction == TransferDirection.Upload ? Path.GetFileName(e.Path.SourcePath) : e.Path.TargetFileName;
				this.PublishStatusMessage($"Large file transfer progress: {filename} {e.Progress:00.00}%.", e.Path.Order);
				TapiLargeFileProgressEventArgs args = new TapiLargeFileProgressEventArgs(
					e.Path,
					e.TotalBytes,
					e.TotalTransferredBytes,
					e.Progress);
				this.LargeFileProgressEvent?.Invoke(this, args);
			}
		}

		/// <inheritdoc />
		protected override void OnTransferPathProgress(object sender, TransferPathProgressEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException(nameof(e));
			}

			// Guard against null timestamps.
			var args = new TapiProgressEventArgs(
				Path.GetFileName(e.TargetFile),
				e.TargetFile,
				e.Completed,
				e.Status == TransferPathStatus.Successful,
				e.Path.Order,
				e.BytesTransferred,
				e.StartTime ?? DateTime.Now,
				e.EndTime ?? DateTime.Now);
			this.ProgressEvent?.Invoke(this, args);
		}
	}
}