using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Mail;
using System.ServiceModel.Activities;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.CodeValidator;

namespace kCura.WinEDDS.Core.Import
{
	public class LoadFileImporter : LoadFileBase, ILoadFileImporter
	{
		private readonly ITransferConfig _config;
		private readonly IImportBatchJobFactory _batchJobBatchJobFactory;
		private readonly IArtifactReader _artReader;

		public LoadFileImporter(ImportContext context, ITransferConfig config, IImportBatchJobFactory batchJobBatchJobFactory, IArtifactReader artifactReader) 
			: base(context.Args, context.Timezoneoffset, context.DoRetryLogic, context.AutoDetect,
				context.InitializeArtifactReader)
		{
			_config = config;
			_batchJobBatchJobFactory = batchJobBatchJobFactory;
			_artifactReader = artifactReader;
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
				// Read First Line
				AdvanceLine();
			}
			catch (Exception ex)
			{
				// TODO!!!!
				Console.WriteLine(ex.Message);
			}
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
			// Here should go equivalent code to exception handling in BulkLoadFileImporter
			catch (Exception ex)
			{
				// TODO!!!!
				Console.WriteLine(ex.Message);
			}
		}
	}
}
