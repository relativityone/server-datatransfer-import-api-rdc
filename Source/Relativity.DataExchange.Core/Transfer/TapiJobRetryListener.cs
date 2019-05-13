// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiJobRetryListener.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System.Globalization;

	using Relativity.DataExchange.Resources;
	using Relativity.Transfer;

	/// <summary>
	/// Represents an object that listens for Transfer API job retry events. This class cannot be inherited.
	/// </summary>
	internal sealed class TapiJobRetryListener : TapiListenerBase
	{
		/// <summary>
		/// The max retry count.
		/// </summary>
		private readonly int maxRetryCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiJobRetryListener"/> class.
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
		public TapiJobRetryListener(ITransferLog log, int maxRetryCount, TransferContext context)
			: base(log, context)
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
			this.PublishStatusMessage(message, TapiConstants.NoLineNumber);
		}
	}
}
