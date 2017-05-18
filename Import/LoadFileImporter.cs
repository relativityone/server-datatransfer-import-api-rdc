using System;
using System.Collections.Specialized;
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

		public LoadFileImporter(ITransferConfig config, IImportBatchJobFactory batchJobBatchJobFactory, IErrorContainer errorContainer, 
			IImportStatusManager importStatusManager, LoadFile args, Controller processController, Guid processID, int timezoneoffset, 
			bool autoDetect, bool initializeUploaders, bool doRetryLogic, string bulkLoadFileFieldDelimiter, bool isCloudInstance, 
			ExecutionSource executionSource = ExecutionSource.Unknown) 
			: base(args, processController, timezoneoffset, autoDetect, initializeUploaders, processID, 
				  doRetryLogic, bulkLoadFileFieldDelimiter, isCloudInstance, true, executionSource)
		{
			
			_importJob = new ImportJob(config, batchJobBatchJobFactory, errorContainer, importStatusManager, this, this);
		}

		#region IImporterSettings members

		public LoadFile LoadFile => _settings;

		public string RunId => _runID;

		#endregion //IImporterSettings members

		#region IImportMetadata members

		public IArtifactReader ArtifactReader => _artifactReader;

		public string PrepareFieldsAndExtractIdentityValue(FileMetadata fileMetadata)
		{
			return PrepareFieldCollectionAndExtractIdentityValue(fileMetadata.ArtifactFieldCollection);
		}

		public NameValueCollection ProcessedDocIdentifiers
		{
			get { return _processedDocumentIdentifiers; }

			set { _processedDocumentIdentifiers = value; }
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
