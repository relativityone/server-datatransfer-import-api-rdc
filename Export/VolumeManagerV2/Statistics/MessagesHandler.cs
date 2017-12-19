using kCura.Windows.Process;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class MessagesHandler : IMessagesHandler
	{
		private TapiBridgeBase _tapiBridge;

		private readonly IStatus _status;
		private readonly ILog _logger;

		public MessagesHandler(IStatus status, ILog logger)
		{
			_status = status;
			_logger = logger;
		}

		public void Attach(TapiBridgeBase tapiBridge)
		{
			_tapiBridge = tapiBridge;
			_tapiBridge.TapiErrorMessage += OnErrorMessage;
			_tapiBridge.TapiStatusMessage += OnStatusMessage;
			_tapiBridge.TapiWarningMessage += OnWarningMessage;
			_tapiBridge.TapiFatalError += OnFatalError;
		}

		private void OnErrorMessage(object sender, TapiMessageEventArgs e)
		{
			//TODO waiting for REL-187625 and REL-187623
			_logger.LogError(e.Message);
			_status.WriteError(e.Message);
		}

		private void OnStatusMessage(object sender, TapiMessageEventArgs e)
		{
			_logger.LogInformation(e.Message);
			_status.WriteStatusLine(EventType.Status, e.Message, false);
		}

		private void OnWarningMessage(object sender, TapiMessageEventArgs e)
		{
			_logger.LogWarning(e.Message);
			_status.WriteWarning(e.Message);
		}

		private void OnFatalError(object sender, TapiMessageEventArgs e)
		{
			//TODO waiting for REL-187625 and REL-187623
			_logger.LogError(e.Message);
			_status.WriteError(e.Message);
		}

		public void Detach()
		{
			_tapiBridge.TapiErrorMessage -= OnErrorMessage;
			_tapiBridge.TapiStatusMessage -= OnStatusMessage;
			_tapiBridge.TapiWarningMessage -= OnWarningMessage;
			_tapiBridge.TapiFatalError -= OnFatalError;
		}
	}
}