using System;
using System.Collections.Specialized;
using kCura.Windows.Process;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.CodeValidator;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Factories;
using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Core.Import.Status;
using kCura.WinEDDS.Service;
using Relativity;

namespace kCura.WinEDDS.Core.Import
{
	public class LoadFileImporter : BulkLoadFileImporter, IImportMetadata, IImporterSettings, IImporterManagers
	{
		private readonly ICancellationProvider _cancellationProvider;
		private readonly IErrorManager _errorManager;
		private readonly IImportJob _importJob;

		public LoadFileImporter(IImportJobFactory jobFactory, IImportStatusManager importStatusManager, ICancellationProvider cancellationProvider,
			IErrorManagerFactory errorManagerFactory, LoadFile args, Controller processController, Guid processId,
			int timezoneoffset, bool autoDetect, bool initializeUploaders, bool doRetryLogic, string bulkLoadFileFieldDelimiter, bool isCloudInstance,
			ExecutionSource executionSource = ExecutionSource.Unknown)
			: base(args, processController, timezoneoffset, autoDetect, initializeUploaders, processId,
				doRetryLogic, bulkLoadFileFieldDelimiter, isCloudInstance, true, executionSource)
		{
			_cancellationProvider = cancellationProvider;

			importStatusManager.EventOccurred += OnEventOccurred;
			importStatusManager.UpdateStatus += ImportStatusManagerOnUpdateStatus;
			importStatusManager.IoWarningOccurred += OnIoWarningOccurred;
			_importJob = jobFactory.Create(this, this, this, _cancellationProvider);
			_errorManager = errorManagerFactory.Create(this);

			processController.ExportErrorFileEvent += OnExportErrorFileEvent;
			processController.ExportServerErrorsEvent += OnExportServerErrorsEvent;
			processController.ExportErrorReportEvent += OnExportErrorReportEvent;
		}

		private void OnExportErrorReportEvent(string exportLocation)
		{
			_errorManager.ExportErrorReport(exportLocation);
		}

		private void OnExportServerErrorsEvent(string exportLocation)
		{
			_errorManager.ExportErrors(exportLocation, LoadFile.FilePath);
		}

		private void OnExportErrorFileEvent(string exportLocation)
		{
			_errorManager.ExportErrorFile(exportLocation);
		}

		#region IImporterSettings members

		public LoadFile LoadFile => _settings;
		public string RunId => _runID;
		public string DefaultDestinationFolderPath => _defaultDestinationFolderPath;
		public int KeyFieldId => _keyFieldID;
		public int OverlayArtifactID => _overlayArtifactID;
		public int FolderId => _folderID;
		public new bool LinkDataGridRecords => base.LinkDataGridRecords;
		public string BulkLoadFileFieldDelimiter => _bulkLoadFileFieldDelimiter;
		public int FilePathColumnIndex => _filePathColumnIndex;
		public ExecutionSource ExecutionSource => _executionSource;
		public ImportOverwriteType Overwrite => _overwrite;
		public ArtifactType ParentArtifactTypeID => (ArtifactType) base.ParentArtifactTypeID;
		public int DestinationFolderColumnIndex => _destinationFolderColumnIndex;

		#endregion //IImporterSettings members

		#region IImportMetadata members

		public IArtifactReader ArtifactReader => _artifactReader;

		public NameValueCollection ProcessedDocIdentifiers
		{
			get { return _processedDocumentIdentifiers; }

			set { _processedDocumentIdentifiers = value; }
		}

		public string PrepareFieldsAndExtractIdentityValue(FileMetadata fileMetadata)
		{
			return PrepareFieldCollectionAndExtractIdentityValue(fileMetadata.ArtifactFieldCollection);
		}

		public void ProcessDocumentMetadata(MetaDocument metaDocument)
		{
			ManageDocumentLine(metaDocument);
		}

		public MetadataFilesInfo InitNewMetadataProcess()
		{
			_outputFileWriter = new OutputFileWriter();
			OpenFileWriters();

			return new MetadataFilesInfo
			{
				CodeFilePath = new FileMetadata
				{
					FullFilePath = _outputCodeFilePath,
					FileGuid = Guid.NewGuid().ToString()
				},
				NativeFilePath = new FileMetadata
				{
					FullFilePath = _outputFileWriter.OutputNativeFilePath,
					FileGuid = Guid.NewGuid().ToString()
				},
				DataGridFilePath = new FileMetadata
				{
					FullFilePath = _outputFileWriter.OutputDataGridFilePath,
					FileGuid = Guid.NewGuid().ToString()
				},
				ObjectFilePath = new FileMetadata
				{
					FullFilePath = _outputObjectFilePath,
					FileGuid = Guid.NewGuid().ToString()
				}
			};
		}

		public void EndMetadataProcess()
		{
			CloseFileWriters();
		}

		public long CurrentMetadataFileStreamLength()
		{
			return _outputFileWriter.CombinedStreamLength;
		}

		public void InitializeJob(ImportContext importContext)
		{
			_recordCount = importContext.TotalRecordCount;
			InitializeFolderManagement();
		}

		public void CleanUp()
		{
			_artifactReader.Close();
			CleanupTempTables();
		}

		#endregion //IImportMetadata members

		#region IImporterManagers members

		public new IBulkImportManager BulkImportManager => new DocumentBulkImportManager(base.BulkImportManager);
		public FolderManager FolderManager => _folderManager;

		#endregion

		#region Overridden Members

		protected override bool UseTimeZoneOffset { get; }

		public override bool HasErrors => _errorManager.HasErrors;

		protected override Base GetSingleCodeValidator()
		{
			return new SingleImporter(_settings.CaseInfo, _codeManager);
		}

		protected override IArtifactReader GetArtifactReader()
		{
			return new LoadFileReader(_settings, false);
		}

		public override object ReadFile(string path)
		{
			_processedDocumentIdentifiers = new NameValueCollection();

			return _importJob.ReadFile(path);
		}

		protected override void _processController_HaltProcessEvent(Guid processId)
		{
			base._processController_HaltProcessEvent(processId);
			_cancellationProvider.Cancel();
		}

		/// <summary>
		///     We must override this method and make it empty so the ExportServerErrorsEvent event will call method from
		///     LoadFileImporter only
		/// </summary>
		/// <param name="exportLocation"></param>
		protected override void _processController_ExportServerErrors(string exportLocation)
		{
		}

		#endregion //Overridden Members

		private void OnEventOccurred(object sender, ImportEventArgs importEventArgs)
		{
			switch (importEventArgs.EventType)
			{
				case ImportEventType.Start:
					OnStartFileImport();
					break;
				case ImportEventType.End:
					OnEndFileImport(RunId);
					break;
				case ImportEventType.FatalError:
					OnFatalError(importEventArgs.Message, importEventArgs.Exception, importEventArgs.JobRunId);
					break;
				case ImportEventType.Error:
					OnReportErrorEvent(importEventArgs.LineError.ToHashtable());
					break;
				case ImportEventType.TransferModeChanged:
					OnUploadModeChangeEvent(importEventArgs.Message, true /*Aspera*/);
					break;
			}
		}

		private void ImportStatusManagerOnUpdateStatus(object sender, ImportStatusUpdateEventArgs statusUpdateEventArgs)
		{
			OnStatusMessage(new StatusEventArgs(GetEventType(statusUpdateEventArgs.Type), Math.Max(statusUpdateEventArgs.LineNumber - 1, 0),
				Math.Max(_recordCount, 0), statusUpdateEventArgs.Message, Statistics.ToDictionary()));
		}

		private void OnIoWarningOccurred(object sender, IoWarningEventArgs ioWarningEventArgs)
		{
			RaiseIoWarning(ioWarningEventArgs);
		}

		private EventType GetEventType(StatusUpdateType statusUpdateType)
		{
			switch (statusUpdateType)
			{
				case StatusUpdateType.Count:
					return EventType.Count;
				case StatusUpdateType.End:
					return EventType.End;
				case StatusUpdateType.Error:
					return EventType.Error;
				case StatusUpdateType.Progress:
					return EventType.Progress;
				case StatusUpdateType.Update:
					return EventType.Status;
				case StatusUpdateType.Warning:
					return EventType.Warning;
				case StatusUpdateType.ResetStartTime:
					return EventType.ResetStartTime;
			}
			throw new ArgumentException($"Unexpected event type {nameof(statusUpdateType)} passed to the method!");
		}
	}
}