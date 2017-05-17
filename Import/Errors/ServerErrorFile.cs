using kCura.Utility;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ServerErrorFile
	{
		private readonly IErrorContainer _errorContainer;

		public ServerErrorFile(IErrorContainer errorContainer)
		{
			_errorContainer = errorContainer;
		}

		public void HandleServerErrors(GenericCsvReader reader)
		{
			if (reader == null)
			{
				const string message = "There was an error while attempting to retrieve the errors from the server.";
				//TODO raise fatal error
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

					//TODO raise status event

					line = reader.ReadLine();
				}
			}
			finally
			{
				reader.IoWarningEvent -= OnIoWarningEvent;
			}
		}

		private void OnIoWarningEvent(RobustIoReporter.IoWarningEventArgs ioWarningEventArgs)
		{
			//TODO
		}
	}
}