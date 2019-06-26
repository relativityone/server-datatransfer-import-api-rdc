﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiPathProgressListener.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.IO;

	using Relativity.Transfer;

	/// <summary>
	/// Represents an object that listens for Transfer API path progress events. This class cannot be inherited.
	/// </summary>
	internal sealed class TapiPathProgressListener : TapiListenerBase
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
				this.PublishStatusMessage($"Large file transfer progress: {e.Progress:00.00}%.", e.Path.Order);
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
				!string.IsNullOrEmpty(e.Path.TargetFileName)
					? e.Path.TargetFileName
					: Path.GetFileName(e.Path.SourcePath),
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