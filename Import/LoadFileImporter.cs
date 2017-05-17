using System;
using System.IO;
using System.Text;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.CodeValidator;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import
{
	public class LoadFileImporter : LoadFileBase, ILoadFileImporter, IImportJobInitilizer
	{
		private readonly ImportContext _context;
		private readonly ITransferConfig _config;
		private readonly IImportBatchJobFactory _batchJobBatchJobFactory;
		private readonly IErrorContainer _errorContainer;
		private readonly IImportStatusManager _importStatusManager;

		public event EventHandler<ImportContext> Initialized;

		public LoadFileImporter(ImportContext context, ITransferConfig config, IImportBatchJobFactory batchJobBatchJobFactory, 
			IErrorContainer errorContainer, IImportStatusManager importStatusManager)
			: base(context.Args, context.Timezoneoffset, context.DoRetryLogic, context.AutoDetect,
				context.InitializeArtifactReader)
		{
			_context = context;
			_config = config;
			_batchJobBatchJobFactory = batchJobBatchJobFactory;
			_errorContainer = errorContainer;
			_importStatusManager = importStatusManager;

			Initialized += _importStatusManager.OnSetJobContext;
		}

		protected override bool UseTimeZoneOffset { get; }
		protected override Base GetSingleCodeValidator()
		{
			return new SingleImporter(_settings.CaseInfo, _codeManager);
		}

		protected override IArtifactReader GetArtifactReader()
		{
			return new LoadFileReader(_settings, false);
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
				AdvanceLine();
			}
			catch (Exception ex)
			{
				HandleFatalError(ex);
			}
		}

		private void SetUploadMode()
		{
			var stringBuilder = new StringBuilder("Metadata: Aspera");
			string fileMode = "not copied";
			if (_settings.CopyFilesToDocumentRepository && string.IsNullOrEmpty(_settings.NativeFilePathColumn))
			{
				fileMode = "Aspera";
			}
			stringBuilder.Append($"- Files: {fileMode}");
			_importStatusManager.RaiseTranserModeChangedEvent(this, stringBuilder.ToString());
		}

		private void RaiseStartEvents()
		{
			Initialized?.Invoke(this, _context);
			_importStatusManager.RaiseStartImportEvent(this);
		}

		private void PopulateJobContext()
		{
			_context.JobRunId = Guid.NewGuid();
			_context.TotalRecordCount = _artifactReader.CountRecords();
			RaiseStartEvents();
		}

		private void SendBatch(ImportBatchContext batchContext)
		{
			IImportBatchJob importBatchJob = _batchJobBatchJobFactory.Create(batchContext);
			importBatchJob.Run(batchContext);
		}

		private bool CanCreateBatch()
		{
			return _artifactReader.HasMoreRecords;
		}

		private ImportBatchContext CreateBatch()
		{
			int currentBatchCounter = 0;
			var importBatchContext = new ImportBatchContext(_config.ImportBatchSize);
			while (_artifactReader.HasMoreRecords && currentBatchCounter < _config.ImportBatchSize)
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
				ArtifactFieldCollection artifactFieldCollection = _artifactReader.ReadArtifact();
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
					_errorContainer.WriteError(CreateErrorLie(index, ex.Message));
				}
			}
			catch (PathTooLongException)
			{
				_errorContainer.WriteError(CreateErrorLie(index, BulkLoadFileImporter.ERROR_MESSAGE_FOLDER_NAME_TOO_LONG));
			}
			catch (kCura.Utility.ImporterExceptionBase impEx)
			{
				_errorContainer.WriteError(CreateErrorLie(index, impEx.Message));
			}
			catch (FileNotFoundException ex)
			{
				_errorContainer.WriteError(CreateErrorLie(index, ex.Message));
			}
			catch(Exception ex)
			{
				HandleFatalError(ex);
			}
		}

		private void HandleFatalError(Exception ex)
		{
			_importStatusManager.RaiseFatalErrorImportEvent(this, string.Empty, CurrentLineNumber, ex);
			_artifactReader.OnFatalErrorState();
		}

		private LineError CreateErrorLie(int index, string message)
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
