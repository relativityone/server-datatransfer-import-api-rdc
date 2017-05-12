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
		private readonly IImportJobBatchFactory _jobBatchJobFactory;

		public LoadFileImporter(LoadFile args, int timezoneoffset, bool doRetryLogic, bool autoDetect, ITransferConfig config, 
			IImportJobBatchFactory jobBatchJobFactory) : base(args, timezoneoffset, doRetryLogic, autoDetect)
		{
			_config = config;
			_jobBatchJobFactory = jobBatchJobFactory;
		}

		public LoadFileImporter(LoadFile args, int timezoneoffset, bool doRetryLogic, bool autoDetect, bool initializeArtifactReader, ITransferConfig config,
			IImportJobBatchFactory jobBatchJobFactory) : base(args, timezoneoffset, doRetryLogic, autoDetect, initializeArtifactReader)
		{
			_config = config;
			_jobBatchJobFactory = jobBatchJobFactory;
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
			IImportBatchJob importBatchJob = _jobBatchJobFactory.Create(batchContext);
			importBatchJob.Run(batchContext);
		}

		private bool CanCreateBatch()
		{
			return _artifactReader.HasMoreRecords;
		}

		private ImportBatchContext CreateBatch()
		{
			int currentBatchCounter = 0;
			var importBatchContext = new ImportBatchContext();
			while (_artifactReader.HasMoreRecords && currentBatchCounter > _config.ImportBatchSize - 1)
			{
				++currentBatchCounter;
				ProcessBatchRecord(importBatchContext);
			}
			return importBatchContext;
		}

		private void ProcessBatchRecord(ImportBatchContext importBatchContext)
		{
			try
			{
				importBatchContext.ArtifactFields.Add(_artifactReader.ReadArtifact());
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
