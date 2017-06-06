using System;
using kCura.Utility;
using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ServerErrorFile : IServerErrorFile
	{
		private readonly IErrorContainer _errorContainer;
		private readonly IImportStatusManager _importStatusManager;

		public ServerErrorFile(IErrorContainer errorContainer, IImportStatusManager importStatusManager)
		{
			_errorContainer = errorContainer;
			_importStatusManager = importStatusManager;
		}

		public void HandleServerErrors(GenericCsvReader reader)
		{
			if (reader == null)
			{
				const string message = "There was an error while attempting to retrieve the errors from the server.";
				_importStatusManager.RaiseFatalErrorImportEvent(this, message, -1, new Exception(message));
				return;
			}
			try
			{
				reader.IoWarningEvent += OnIoWarningEvent;

				var line = reader.ReadLine();

				while (line != null)
				{
					var lineError = new LineError
					{
						LineNumber = int.Parse(line[0]),
						Message = line[1],
						Identifier = line[2],
						ErrorType = ErrorType.server
					};
					_errorContainer.WriteError(lineError);

					line = reader.ReadLine();
				}
			}
			finally
			{
				reader.IoWarningEvent -= OnIoWarningEvent;
				reader.Close();
			}
		}

		private void OnIoWarningEvent(RobustIoReporter.IoWarningEventArgs ioWarningEventArgs)
		{
			_importStatusManager.RaiseIoWarningEvent(this, ioWarningEventArgs.WaitTime, (int) ioWarningEventArgs.CurrentLineNumber, ioWarningEventArgs.Exception);
		}
	}
}