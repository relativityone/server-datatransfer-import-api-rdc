namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using kCura.WinEDDS;

	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class MessagesHandler : IMessagesHandler
	{
		private readonly IStatus _status;
		private readonly ILog _logger;

		public MessagesHandler(IStatus status, ILog logger)
		{
			_status = status.ThrowIfNull(nameof(status));
			_logger = logger.ThrowIfNull(nameof(logger));
		}

		public void Attach(ITapiBridge tapiBridge)
		{
			// Note: this is executed from multiple tasks but is thread-safe.
			tapiBridge.TapiErrorMessage += this.OnErrorMessage;
			tapiBridge.TapiStatusMessage += this.OnStatusMessage;
			tapiBridge.TapiWarningMessage += this.OnWarningMessage;
			tapiBridge.TapiFatalError += this.OnFatalError;
		}

		public void Detach(ITapiBridge tapiBridge)
		{
			// Note: this is executed from multiple tasks but is thread-safe.
			tapiBridge.TapiErrorMessage -= this.OnErrorMessage;
			tapiBridge.TapiStatusMessage -= this.OnStatusMessage;
			tapiBridge.TapiWarningMessage -= this.OnWarningMessage;
			tapiBridge.TapiFatalError -= this.OnFatalError;
		}

		private void OnErrorMessage(object sender, TapiMessageEventArgs e)
		{
			_logger.LogError(e.Message);
			_status.WriteError(e.Message);
		}

		private void OnStatusMessage(object sender, TapiMessageEventArgs e)
		{
			_logger.LogInformation(e.Message);
			_status.WriteStatusLine(EventType2.Status, e.Message, false);
		}

		private void OnWarningMessage(object sender, TapiMessageEventArgs e)
		{
			_logger.LogWarning(e.Message);
			_status.WriteWarning(e.Message);
		}

		private void OnFatalError(object sender, TapiMessageEventArgs e)
		{
			_logger.LogError(e.Message);
			_status.WriteError(e.Message);
		}
	}
}