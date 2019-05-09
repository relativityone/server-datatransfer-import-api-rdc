// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiRequestListener.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
	using System;

	using Relativity.Import.Export.Resources;
	using Relativity.Transfer;

	/// <summary>
	/// Represents an object that listens for Transfer API request events. This class cannot be inherited.
	/// </summary>
	internal sealed class TapiRequestListener : TapiListenerBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiRequestListener"/> class.
		/// </summary>
		/// <param name="log">
		/// The transfer log.
		/// </param>
		/// <param name="context">
		/// The transfer context.
		/// </param>
		public TapiRequestListener(ITransferLog log, TransferContext context)
			: base(log, context)
		{
		}

		/// <inheritdoc />
		protected override void OnTransferRequestEvent(object sender, TransferRequestEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException(nameof(e));
			}

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