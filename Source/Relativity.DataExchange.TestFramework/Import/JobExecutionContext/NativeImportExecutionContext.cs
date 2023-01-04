// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeImportExecutionContext.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.TestFramework.Import.JobExecutionContext
{
	using System;
	using System.Data;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Extensions;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;

	/// <summary>
	/// Execution context for native import tests.
	/// </summary>
	/// <remarks>Instances of that class can be used across AppDomains.</remarks>
	public class NativeImportExecutionContext : BaseExecutionContext<ImportBulkArtifactJob, Settings>
	{
		public override void SetUpImportApi(Func<ImportAPI> importApiFactory, Settings settings)
		{
			base.SetUpImportApi(importApiFactory, settings);

			// Attach native import specific events
			this.ImportJob.OnError += this.ImportJobOnError;
			this.ImportJob.OnMessage += this.ImportJobOnMessage;
		}

		public override ImportTestJobResult Execute(IDataReader dataReader)
		{
			this.ImportJob.SourceData.SourceData = dataReader;
			this.ImportJob.Execute();

			TimeSpan elapsedTime = this.TestJobResult.EndTime - this.TestJobResult.StartTime;
			this.Logger.LogInformation("Import API elapsed time: {elapsedTime}", elapsedTime);

			return this.TestJobResult;
		}

		protected override ImportBulkArtifactJob CreateJobWithSettings(Settings settings)
		{
			settings.ThrowIfNull(nameof(settings));
			ImportBulkArtifactJob importJob = settings.ArtifactTypeId == (int)ArtifactType.Document | settings.ArtifactTypeId == 0
				? this.ImportApi.NewNativeDocumentImportJob()
				: this.ImportApi.NewObjectImportJob(settings.ArtifactTypeId);

			settings.CopyTo(importJob.Settings);

			IntegrationTestParameters testParameters = IntegrationTestHelper.IntegrationTestParameters;
			importJob.Settings.WebServiceURL = testParameters.RelativityWebApiUrl.ToString();
			importJob.Settings.CaseArtifactId = testParameters.WorkspaceId;
			return importJob;
		}
	}
}
