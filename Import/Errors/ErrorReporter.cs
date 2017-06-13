using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ErrorReporter : IErrorContainer
	{
		private const string _MAX_ERROR_COUNT_REACHED_MESSAGE = "Maximum number of errors for display reached.  Export errors to view full list.";

		private readonly ITransferConfig _transferConfig;
		private readonly IImportStatusManager _importStatusManager;

		private int _errorCounter;

		private readonly object _syncObject = new object();

		public ErrorReporter(ITransferConfig transferConfig, IImportStatusManager importStatusManager)
		{
			_transferConfig = transferConfig;
			_importStatusManager = importStatusManager;
		}


		public void WriteError(LineError lineError)
		{
			int threadSafeErrorCount;
			lock (_syncObject)
			{
				_errorCounter++;
				threadSafeErrorCount = _errorCounter;
			}

			RaiseErrorEvent(lineError, threadSafeErrorCount);
		}

		public bool HasErrors()
		{
			return _errorCounter > 0;
		}

		private void RaiseErrorEvent(LineError lineError, int threadSafeErrorCount)
		{
			string lineErrorMessage = lineError.Message;
			if (threadSafeErrorCount < _transferConfig.DefaultMaximumErrorCount)
			{
				_importStatusManager.RaiseErrorImportEvent(this, lineError);
			}
			else if (threadSafeErrorCount == _transferConfig.DefaultMaximumErrorCount)
			{
				lineErrorMessage = _MAX_ERROR_COUNT_REACHED_MESSAGE;
				var moreToBeFoundMessage = new LineError
				{
					Message = lineErrorMessage
				};
				_importStatusManager.RaiseErrorImportEvent(this, moreToBeFoundMessage);
			}
			_importStatusManager.RaiseStatusUpdateEvent(this, StatusUpdateType.Error, lineErrorMessage, lineError.LineNumber);
		}
	}
}