
using System;
using System.IO;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.CodeValidator;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportJob : ILoadFileImporter, IImportJobInitilizer
	{
		private readonly IImportMetadata _importer;
		private readonly ImportContext _context;
		private readonly ITransferConfig _config;
		private readonly IImportBatchJobFactory _batchJobBatchJobFactory;
		private readonly IErrorContainer _errorContainer;
		private readonly IImportStatusManager _importStatusManager;

		public event EventHandler<ImportContext> Initialized;

		public ImportJob(ITransferConfig config, IImportBatchJobFactory batchJobBatchJobFactory,
			IErrorContainer errorContainer, IImportStatusManager importStatusManager, IImportMetadata importer, IImporterSettings importerSettings)
		{
			_config = config;
			_batchJobBatchJobFactory = batchJobBatchJobFactory;
			_errorContainer = errorContainer;
			_importStatusManager = importStatusManager;
			_importer = importer;

			_context = new ImportContext()
			{
				Settings = importerSettings
			};

			Initialized += _importStatusManager.OnSetJobContext;
		}

		public object ReadFile(string path)
		{
			// TODO -> check continue flag
			InitializeProcess();
			while (CanCreateBatch())
			{
				ImportBatchContext batchSetUp = CreateBatch();
				SendBatch(batchSetUp);
			}
			return true;
		}

		private void InitializeProcess()
		{
			try
			{
				PopulateJobContext();
				SetUploadMode();
				// Read First Header Line
				_importer.ArtifactReader.AdvanceRecord();
			}
			catch (Exception ex)
			{
				HandleFatalError(ex);
			}
		}

		private void SetUploadMode()
		{
			//var stringBuilder = new StringBuilder("Metadata: Aspera");
			//string fileMode = "not copied";
			//if (_settings.CopyFilesToDocumentRepository && string.IsNullOrEmpty(_settings.NativeFilePathColumn))
			//{
			//	fileMode = "Aspera";
			//}
			//stringBuilder.Append($"- Files: {fileMode}");
			//_importStatusManager.RaiseTranserModeChangedEvent(this, stringBuilder.ToString());
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

		private void SendBatch(ImportBatchContext batchContext)
		{
			IImportBatchJob importBatchJob = _batchJobBatchJobFactory.Create(batchContext);
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
					_errorContainer.WriteError(CreateErrorLine(index, ex.Message));
				}
			}
			catch (PathTooLongException)
			{
				_errorContainer.WriteError(CreateErrorLine(index,BulkLoadFileImporter.ERROR_MESSAGE_FOLDER_NAME_TOO_LONG));
			}
			catch (kCura.Utility.ImporterExceptionBase impEx)
			{
				_errorContainer.WriteError(CreateErrorLine(index, impEx.Message));
			}
			catch (FileNotFoundException ex)
			{
				_errorContainer.WriteError(CreateErrorLine(index, ex.Message));
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
