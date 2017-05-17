using System;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ErrorReporter : IErrorContainer, IErrorReporter
	{
		private const string _MAX_ERROR_COUNT_REACHED_MESSAGE = "Maximum number of errors for display reached.  Export errors to view full list.";

		private readonly ITransferConfig _transferConfig;

		private int _errorCounter;

		private readonly object _syncObject = new object();

		public ErrorReporter(ITransferConfig transferConfig)
		{
			_transferConfig = transferConfig;
		}

		public event EventHandler<LineError> ErrorOccurred;

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
			if (threadSafeErrorCount < _transferConfig.DefaultMaximumErrorCount)
			{
				ErrorOccurred?.Invoke(this, lineError);
			}
			else if (threadSafeErrorCount == _transferConfig.DefaultMaximumErrorCount)
			{
				var moreToBeFoundMessage = new LineError
				{
					Message = _MAX_ERROR_COUNT_REACHED_MESSAGE
				};
				ErrorOccurred?.Invoke(this, moreToBeFoundMessage);
			}
		}
	}
}