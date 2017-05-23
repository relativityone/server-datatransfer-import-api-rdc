using System;
using System.IO;
using System.Text;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.CodeValidator;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportJob : IImportJob, IImportJobInitilizer
	{
		private readonly IImportMetadata _importer;
		private readonly ImportContext _context;
		private readonly ITransferConfig _config;
		private readonly IImportBatchJobFactory _batchJobBatchJobFactory;
		private readonly IErrorContainer _errorContainer;
		private readonly IImportStatusManager _importStatusManager;
		private readonly IImporterSettings _importerSettings;

		public event EventHandler<ImportContext> Initialized;

		public ImportJob(ITransferConfig config, IImportBatchJobFactory batchJobBatchJobFactory,
			IErrorContainer errorContainer, IImportStatusManager importStatusManager, IImportMetadata importer, IImporterSettings importerSettings)
		{
			_config = config;
			_batchJobBatchJobFactory = batchJobBatchJobFactory;
			_errorContainer = errorContainer;
			_importStatusManager = importStatusManager;
			_importer = importer;
			_importerSettings = importerSettings;

			_context = new ImportContext()
			{
				Settings = _importerSettings
			};

			Initialized += _importStatusManager.OnSetJobContext;
		}

		public object ReadFile(string path)
		{
			// TODO -> check continue flag
			try
			{
				if (InitializeProcess())
				{
					return false;
				}
				while (CanCreateBatch())
				{
					ImportBatchContext batchSetUp = CreateBatch();
					SendBatch(batchSetUp);
				}
			}
			catch (Exception ex)
			{
				HandleFatalError(ex);
			}
			finally
			{
				_importer.CleanUp();
			}
			return true;
		}

		private bool InitializeProcess()
		{
			
			PopulateJobContext();
			if (ValidateJobContext(_context))
			{
				return false;
			}
			SetUploadMode();
			while (HasToMoveRecordIndex())
			{
				_importer.ArtifactReader.AdvanceRecord();
			}
			return true;
		}

		private bool HasToMoveRecordIndex()
		{
			return _importer.ArtifactReader.HasMoreRecords 
				&& _importer.ArtifactReader.CurrentLineNumber < _importerSettings.LoadFile.StartLineNumber;
		}

		private void SetUploadMode()
		{
			string uploadModeDesc = UploadModeHelper.GetAsperaModeDescriptor(_importerSettings.LoadFile);
			_importStatusManager.RaiseTranserModeChangedEvent(this, uploadModeDesc);
		}

		private void RaiseStartEvents()
		{
			Initialized?.Invoke(this, _context);
			_importStatusManager.RaiseStartImportEvent(this);
		}

		private void PopulateJobContext()
		{
			_context.TotalRecordCount = _importer.ArtifactReader.CountRecords();
			RaiseStartEvents();
		}

		private bool ValidateJobContext(ImportContext context)
		{
			if (context.TotalRecordCount < 0)
			{
				_importStatusManager.RaiseStatusUpdateEvent(this, StatusUpdateType.Progress, "cancel import", _importer.ArtifactReader.CurrentLineNumber);
				return false;
			}
			return true;
		}

		private void SendBatch(ImportBatchContext batchContext)
		{
			IImportBatchJob importBatchJob = _batchJobBatchJobFactory.Create(batchContext, _importer, _importerSettings);
			importBatchJob.Run(batchContext);
		}

		private bool CanCreateBatch()
		{
			return _importer.ArtifactReader.HasMoreRecords;
		}

		private ImportBatchContext CreateBatch()
		{
			int currentBatchCounter = 0;
			var importBatchContext = new ImportBatchContext(_context, _config.ImportBatchSize);
			while (_importer.ArtifactReader.HasMoreRecords && currentBatchCounter < _config.ImportBatchSize)
			{
				++currentBatchCounter;
				ProcessBatchRecord(importBatchContext, currentBatchCounter);
			}
			return importBatchContext;
		}

		private void ProcessBatchRecord(ImportBatchContext importBatchContext, int index)
		{
			try
			{
				ArtifactFieldCollection artifactFieldCollection = _importer.ArtifactReader.ReadArtifact();
				importBatchContext.FileMetaDataHolder.Add(new FileMetadata
				{
					ArtifactFieldCollection = artifactFieldCollection,
					LineNumber = index
				});
			}
			catch (CodeCreationException ex)
			{
				if (ex.IsFatal)
				{
					HandleFatalError(ex);
				}
				else
				{
					HandleRecordError(index, ex.Message);
				}
			}
			catch (PathTooLongException)
			{
				HandleRecordError(index, BulkLoadFileImporter.ERROR_MESSAGE_FOLDER_NAME_TOO_LONG);
			}
			catch (kCura.Utility.ImporterExceptionBase ex)
			{
				HandleRecordError(index, ex.Message);
			}
			catch (FileNotFoundException ex)
			{
				HandleRecordError(index, ex.Message);
			}
			catch (Exception ex)
			{
				HandleFatalError(ex);
			}
		}

		private void HandleFatalError(Exception ex)
		{
			_importStatusManager.RaiseFatalErrorImportEvent(this, string.Empty, _importer.ArtifactReader.CurrentLineNumber, ex);
			_importer.ArtifactReader.OnFatalErrorState();
		}

		private void HandleRecordError(int recordIndex, string message)
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
	}
}
