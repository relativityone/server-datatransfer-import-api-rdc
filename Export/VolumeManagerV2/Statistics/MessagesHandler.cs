using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class MessagesHandler : IMessagesHandler
	{
		private ITapiBridge _tapiBridge;

		private readonly IStatus _status;
		private readonly ILog _logger;

		public MessagesHandler(IStatus status, ILog logger)
		{
			_status = status;
			_logger = logger;
		}

		public void Attach(ITapiBridge tapiBridge)
		{
			_tapiBridge = tapiBridge;
			_tapiBridge.TapiErrorMessage += OnErrorMessage;
			_tapiBridge.TapiStatusMessage += OnStatusMessage;
			_tapiBridge.TapiWarningMessage += OnWarningMessage;
			_tapiBridge.TapiFatalError += OnFatalError;
		}

		private void OnErrorMessage(object sender, TapiMessageEventArgs e)
		{
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