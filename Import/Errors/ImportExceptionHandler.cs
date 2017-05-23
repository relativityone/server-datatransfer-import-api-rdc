
using System;
using System.IO;
using kCura.WinEDDS.CodeValidator;
using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ImportExceptionHandler
	{
		private readonly IImportMetadata _metadataImporter;
		private readonly IImportStatusManager _importStatusManager;
		private readonly IErrorContainer _errorContainer;

		public ImportExceptionHandler(IImportStatusManager importStatusManager, IImportMetadata metadataImporter, IErrorContainer errorContainer)
		{
			_importStatusManager = importStatusManager;
			_metadataImporter = metadataImporter;
			_errorContainer = errorContainer;
		}

		public void HandleFatalError(Exception ex)
		{
			_importStatusManager.RaiseFatalErrorImportEvent(this, string.Empty, _metadataImporter.ArtifactReader.CurrentLineNumber, ex);
			_metadataImporter.ArtifactReader.OnFatalErrorState();
		}

		public void HandleRecordError(int recordIndex, string message)
		{
			_importStatusManager.RaiseErrorImportEvent(this, new LineError
			{
				ErrorType = ErrorType.client,
				Message = message
			});
			_errorContainer.WriteError(CreateErrorLine(recordIndex, message));
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

		public void SafeExec(Action action)
		{
			try
			{
				action();
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
				HandleRecordError(_metadataImporter.ArtifactReader.CurrentLineNumber, BulkLoadFileImporter.ERROR_MESSAGE_FOLDER_NAME_TOO_LONG);
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
				HandleFatalError(ex);
			}
		}
	}
}
