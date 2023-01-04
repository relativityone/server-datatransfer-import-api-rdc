// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiRequestListener.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;

	using Relativity.DataExchange.Resources;
	using Relativity.Logging;
	using Relativity.Transfer;

	/// <summary>
	/// Represents an object that listens for Transfer API request events. This class cannot be inherited.
	/// </summary>
	internal sealed class TapiRequestListener : TapiListenerBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiRequestListener"/> class.
		/// </summary>
		/// <param name="logger">
		/// The Relativity logger instance.
		/// </param>
		/// <param name="context">
		/// The transfer context.
		/// </param>
		public TapiRequestListener(ILog logger, TransferContext context)
			: base(logger, context)
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
					this.Logger.LogInformation(Strings.TransferJobStartedMessage);
					break;

				case TransferRequestStatus.Ended:
					this.Logger.LogInformation(Strings.TransferJobEndedMessage);
					break;

				case TransferRequestStatus.EndedMaxRetry:
					this.Logger.LogInformation(Strings.TransferJobEndedMaxRetryMessage);
					break;

				case TransferRequestStatus.Canceled:
					this.Logger.LogInformation(Strings.TransferJobCanceledMessage);
					break;
			}
		}
	}
}