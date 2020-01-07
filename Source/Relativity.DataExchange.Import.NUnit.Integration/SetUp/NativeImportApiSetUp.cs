// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeImportApiSetUp.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Import.NUnit.Integration.SetUp
{
	using System;
	using System.Data;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Extensions;
	using Relativity.Logging;

	public class NativeImportApiSetUp : ImportApiSetUp<ImportBulkArtifactJob, Settings>
	{
		private readonly ILog logger;

		public NativeImportApiSetUp()
		{
			this.logger = IntegrationTestHelper.Logger;
		}

		public override void SetUpImportApi(Func<ImportAPI> importApiFunc, Settings settings)
		{
			base.SetUpImportApi(importApiFunc, settings);

			// Attach native import specific events
			this.ImportJob.OnError += this.ImportJob_OnError;
			this.ImportJob.OnMessage += this.ImportJob_OnMessage;
		}

		public override void Execute(IDataReader dataReader)
		{
			this.ImportJob.SourceData.SourceData = dataReader;
			this.ImportJob.Execute();

			TimeSpan elapsedTime = this.TestJobResult.CompletedJobReport.EndTime - this.TestJobResult.CompletedJobReport.StartTime;
			this.logger.LogInformation("Import API elapsed time: {elapsedTime}", elapsedTime);
		}

		protected override ImportBulkArtifactJob CreateJobWithSettings(Settings settings)
		{
			settings.ThrowIfNull(nameof(settings));
			ImportBulkArtifactJob importJob = this.ImportApi.NewNativeDocumentImportJob();

			settings.CopyTo(importJob.Settings);
			importJob.Settings.WebServiceURL = AssemblySetup.TestParameters.RelativityWebApiUrl.ToString();
			importJob.Settings.CaseArtifactId = AssemblySetup.TestParameters.WorkspaceId;
			return importJob;
		}
	}
}
