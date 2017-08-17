using System;
using Castle.Windsor;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import.Factories;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Statistics;
using kCura.WinEDDS.Core.Import.Status;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportJob : IImportJob, IImportJobInitilizer
	{
		private readonly IImportMetadata _importer;
		private readonly ImportContext _context;
		private readonly ITransferConfig _config;
		private readonly IImportBatchJobFactory _batchJobBatchJobFactory;
		private readonly IImportStatusManager _importStatusManager;
		private readonly IImporterSettings _importerSettings;
		private readonly IImportExceptionHandlerExec _importExceptionHandlerExec;
		private readonly ICancellationProvider _cancellationProvider;
		private readonly IJobFinishStatisticsHandler _jobFinishStatisticsHandler;
		private readonly ILog _log;
		private readonly IWindsorContainer _container;

		public event EventHandler<ImportContext> Initialized;

		public ImportJob(ITransferConfig config, IImportBatchJobFactory batchJobBatchJobFactory, IImportStatusManager importStatusManager, 
			IImportMetadata importer, IImporterSettings importerSettings, IImportExceptionHandlerExec importExceptionHandlerExec,
			ICancellationProvider cancellationProvider, IJobFinishStatisticsHandler jobFinishStatisticsHandler, ILog log, IWindsorContainer container)
		{
			_config = config;
			_batchJobBatchJobFactory = batchJobBatchJobFactory;
			_importStatusManager = importStatusManager;
			_importer = importer;
			_importerSettings = importerSettings;
			_importExceptionHandlerExec = importExceptionHandlerExec;
			_cancellationProvider = cancellationProvider;
			_jobFinishStatisticsHandler = jobFinishStatisticsHandler;
			_log = log;
			_container = container;

			_context = new ImportContext
			{
				Settings = _importerSettings
			};
			Initialized += _importStatusManager.OnSetJobContext;
		}

		public object ReadFile(string path)
		{
			_log.LogInformation($"Import job started for '{path}' file.", path);
			var returnValue =  _importExceptionHandlerExec.TryCatchExec<bool?>(
			() =>
				{
					try
					{
						if (!InitializeProcess())
						{
							_log.LogInformation("Import job initialization process failed!");
							return false;
						}
						while (CanCreateBatch())
						{
							ImportBatchContext batchSetUp = CreateBatch();
							SendBatch(batchSetUp);
						}
						_jobFinishStatisticsHandler.RaiseJobFinishedEvent("Import process finished");
						return true;
					}
					catch (OperationCanceledException )
					{
						_jobFinishStatisticsHandler.RaiseJobFinishedEvent("Import process cancelled");
					}
					finally
					{
						_importStatusManager.RaiseEndImportEvent(this);
						_container.Dispose();
					}
					return true;
				}, false, _importer.CleanUp);
			_log.LogInformation($"Import job finished for '{path}' file.", path);
			return returnValue;
		}

		private bool CanCreateBatch()
		{
			return _importer.ArtifactReader.HasMoreRecords;
		}

		private bool InitializeProcess()
		{
			_log.LogInformation("Import job process initialization started");
			PopulateJobContext();
			if (!ValidateJobContext(_context))
			{
				return false;
			}
			InitProcess(_context);
			while (HasToMoveRecordIndex())
			{
				_importer.ArtifactReader.AdvanceRecord();
			}
			_cancellationProvider.ThrowIfCancellationRequested();
			return true;
		}

		private bool HasToMoveRecordIndex()
		{
			return _importer.ArtifactReader.HasMoreRecords
					&& _importer.ArtifactReader.CurrentLineNumber < _importerSettings.LoadFile.StartLineNumber;
		}

		private void InitProcess(ImportContext importContext)
		{
			string uploadModeDesc = UploadModeHelper.GetAsperaModeDescriptor(_importerSettings.LoadFile);
			_importStatusManager.RaiseTransferModeChangedEvent(this, uploadModeDesc);

			_importer.InitializeJob(importContext);
			_importStatusManager.RaiseStatusUpdateEvent(this, StatusUpdateType.ResetStartTime, BulkLoadFileImporter.RestartTimeEventMsg, 0);
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
				_importStatusManager.RaiseStatusUpdateEvent(this, StatusUpdateType.Progress, BulkLoadFileImporter.CancelEventMsg, 
					_importer.ArtifactReader.CurrentLineNumber);
				return false;
			}
			return true;
		}

		private void SendBatch(ImportBatchContext batchContext)
		{
			if (batchContext.FileMetaDataHolder.Count > 0)
			{
				IImportBatchJob importBatchJob = _batchJobBatchJobFactory.Create(batchContext);
				importBatchJob.Run(batchContext);
			}
		}

		private ImportBatchContext CreateBatch()
		{
			//That's not great but we need to quickly fix the issue
			int updateStats = Math.Max(_config.ImportBatchSize / 10, _config.ImportBatchSize);

			_log.LogInformation($"New batch creation started at record {_importer.ArtifactReader.CurrentLineNumber}");
			var currentBatchCounter = 0;
			var importBatchContext = new ImportBatchContext(_context, _config.ImportBatchSize);
			while (CanProcessNextRecord(currentBatchCounter))
			{
				_cancellationProvider.ThrowIfCancellationRequested();
				++currentBatchCounter;

				_importStatusManager.RaiseStatusUpdateEvent(this, StatusUpdateType.Count, string.Empty, _importer.ArtifactReader.CurrentLineNumber);

				_importExceptionHandlerExec.TryCatchExecNonFatal(() =>
					ProcessBatchRecord(importBatchContext)
				);

				if ((_importer.ArtifactReader.CurrentLineNumber - 1) % updateStats == 0)
				{
					_importStatusManager.RaiseCustomStatusUpdateEvent(this, StatusUpdateType.Progress,
						$"Read {_importer.ArtifactReader.CurrentLineNumber -1 } records", ProgressStep());
				}
			}

			_importStatusManager.RaiseCustomStatusUpdateEvent(this, StatusUpdateType.Progress,
						$"Read {_importer.ArtifactReader.CurrentLineNumber - 1 } records", ProgressStep());

			_log.LogInformation($"New batch of size: {importBatchContext.FileMetaDataHolder.Count} created");
			return importBatchContext;
		}

		private int ProgressStep()
		{
			return (int)((_importer.ArtifactReader.CurrentLineNumber - 2 ) / _config.ImportBatchSize) * _config.ImportBatchSize;
		}

		private bool CanProcessNextRecord(int currentBatchCounter)
		{
			return _importer.ArtifactReader.HasMoreRecords && currentBatchCounter < _config.ImportBatchSize;
		}

		private void ProcessBatchRecord(ImportBatchContext importBatchContext)
		{
			ArtifactFieldCollection artifactFieldCollection = _importer.ArtifactReader.ReadArtifact();
			importBatchContext.FileMetaDataHolder.Add(new FileMetadata
			{
				ArtifactFieldCollection = artifactFieldCollection,
				LineNumber = _importer.ArtifactReader.CurrentLineNumber
			});
		}
	}
}
