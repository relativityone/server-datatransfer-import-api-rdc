// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeImportExecutionContext.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Import.NUnit.Integration.JobExecutionContext
{
	using System;
	using System.Data;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Extensions;
	using Relativity.DataExchange.TestFramework.ImportDataSource;

	/// <summary>
	/// Execution context for native import tests.
	/// </summary>
	/// <remarks>Instances of that class can be used across AppDomains.</remarks>
	public class NativeImportExecutionContext : BaseExecutionContext<ImportBulkArtifactJob, Settings>
	{
		public void SetUpImportApi(IntegrationTestParameters parameters, ISettingsBuilder<Settings> settingsBuilder)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
			settingsBuilder = settingsBuilder ?? throw new ArgumentNullException(nameof(settingsBuilder));

			this.InitializeImportApiWithUserAndPassword(parameters, settingsBuilder.Build());
		}

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
			ImportBulkArtifactJob importJob = this.ImportApi.NewNativeDocumentImportJob();

			settings.CopyTo(importJob.Settings);
			importJob.Settings.WebServiceURL = AssemblySetup.TestParameters.RelativityWebApiUrl.ToString();
			importJob.Settings.CaseArtifactId = AssemblySetup.TestParameters.WorkspaceId;
			return importJob;
		}
	}
}
