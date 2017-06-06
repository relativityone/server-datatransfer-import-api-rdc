
using System;
using System.IO;
using kCura.WinEDDS.CodeValidator;
using kCura.WinEDDS.Core.Import.Status;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ImportExceptionHandlerExec : IImportExceptionHandlerExec
	{
		private readonly IImportMetadata _metadataImporter;
		private readonly IImportStatusManager _importStatusManager;
		private readonly IErrorContainer _errorContainer;
		private readonly ILog _log;

		public ImportExceptionHandlerExec(IImportStatusManager importStatusManager, IImportMetadata metadataImporter, IErrorContainer errorContainer, ILog log)
		{
			_importStatusManager = importStatusManager;
			_metadataImporter = metadataImporter;
			_errorContainer = errorContainer;
			_log = log;
		}

		public void TryCatchExec(Action executeAction, Action finalizeAction = null)
		{
			TryCatchExec(() =>
			{
				executeAction();
				return true;
			}, (bool?)true, finalizeAction);
		}

		public T TryCatchExec<T>(Func<T> executeAction, T defaultRetValue = default(T), Action finalizeAction = null)
		{
			try
			{
				return executeAction();
			}
			catch (CodeCreationException ex)
			{
				if (ex.IsFatal)
				{
					HandleFatalError(ex);
				}
				else
				{
					HandleRecordError(_metadataImporter.ArtifactReader.CurrentLineNumber, ex.Message);
				}
			}
			catch (PathTooLongException)
			{
				HandleRecordError(_metadataImporter.ArtifactReader.CurrentLineNumber,
					BulkLoadFileImporter.ERROR_MESSAGE_FOLDER_NAME_TOO_LONG);
			}
			catch (kCura.Utility.ImporterExceptionBase ex)
			{
				HandleRecordError(_metadataImporter.ArtifactReader.CurrentLineNumber, ex.Message);
			}
			catch (FileNotFoundException ex)
			{
				HandleRecordError(_metadataImporter.ArtifactReader.CurrentLineNumber, ex.Message);
			}
			catch (Exception ex)
			{
				if (ex is OperationCanceledException)
				{
					// Cancellation will be handled at the top process level
					_log.LogInformation(LogMessages.UserImportCancelMessage);
					throw;
				}
				HandleFatalError(ex);
			}
			finally
			{
				finalizeAction?.Invoke();
			}
			return defaultRetValue;
		}

		public void IgnoreOnExceptionExec<TException>(Action action, Func<bool> condition = null)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				if (ex is TException)
				{
					if (condition != null && condition())
					{
						// Ignore error
					}
				}
				throw;
			}
		}

		private void HandleFatalError(Exception ex)
		{
			_importStatusManager.RaiseFatalErrorImportEvent(this, string.Empty, _metadataImporter.ArtifactReader.CurrentLineNumber, ex);
			_metadataImporter.ArtifactReader.OnFatalErrorState();
		}

		private void HandleRecordError(int recordIndex, string message)
		{
			_importStatusManager.RaiseErrorImportEvent(this, new LineError
			{
				ErrorType = ErrorType.client,
				Message = message
			});
			_errorContainer.WriteError(CreateErrorLine(recordIndex, message));
			_log.LogError(message);
		}
		private LineError CreateErrorLine(int index, string message)
		{
			return new LineError()
			{
				ErrorType = ErrorType.client,
				LineNumber = index,
				Message = message
			};
		}
	}
}
