using System;
using System.Collections.Specialized;
using System.IO;
using kCura.Windows.Process;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.CodeValidator;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;
using Relativity;

namespace kCura.WinEDDS.Core.Import
{
	public class LoadFileImporter : BulkLoadFileImporter, IImportMetadata, IImporterSettings
	{

		private readonly ILoadFileImporter _importJob;

		public LoadFileImporter(IImportJobFactory jobFactory, ITransferConfig config, IErrorContainer errorContainer,
			IImportStatusManager importStatusManager, LoadFile args, Controller processController, Guid processId, int timezoneoffset,
			bool autoDetect, bool initializeUploaders, bool doRetryLogic, string bulkLoadFileFieldDelimiter, bool isCloudInstance, ExecutionSource executionSource = ExecutionSource.Unknown)
			: base(args, processController, timezoneoffset, autoDetect, initializeUploaders, processId,
				doRetryLogic, bulkLoadFileFieldDelimiter, isCloudInstance, true, executionSource)
		{
			_importJob = jobFactory.Create(config, importStatusManager, this, this, errorContainer);
		}

		#region IImporterSettings members

		public LoadFile LoadFile => _settings;
		public string RunId => _runID;
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

		public MetadataFilesInfo InitMetadataProcess()
		{
			DeleteFiles();
			OpenFileWriters();

			return new MetadataFilesInfo()
			{
				CodeFilePath = new FileMetadata()
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
				},
			};
		}

		public void SubmitMetadataProcess()
		{
			CloseFileWriters();
		}

		#endregion //IImportMetadata members

		#region Overridden Members

		protected override bool UseTimeZoneOffset { get; }

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

		#endregion //Overridden Members
	}
}
