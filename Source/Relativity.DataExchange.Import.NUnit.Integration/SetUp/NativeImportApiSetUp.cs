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
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Extensions;

	public class NativeImportApiSetUp : ImportApiSetUp<ImportBulkArtifactJob, Settings>
	{
		public override void SetUpImportApi(Func<ImportAPI> importApiFunc, Settings settings)
		{
			base.SetUpImportApi(importApiFunc, settings);

			// Attach native import specific events
			this.ImportJob.OnError += this.ImportJob_OnError;
			this.ImportJob.OnMessage += this.ImportJob_OnMessage;
		}

		public override void Execute<T>(IEnumerable<T> importData)
		{
			using (var dataReader = new EnumerableDataReader<T>(importData))
			{
				this.ImportJob.SourceData.SourceData = dataReader;
				this.ImportJob.Execute();
			}

			Console.WriteLine(
				"Import API elapsed time: {0}",
				this.TestJobResult.CompletedJobReport.EndTime - this.TestJobResult.CompletedJobReport.StartTime);
		}

		protected override ImportBulkArtifactJob CreateJobWithSettings(Settings settings)
		{
			settings.ThrowIfNull(nameof(settings));

			var importJob = this.ImportApi.NewNativeDocumentImportJob();

			if (importJob == null)
			{
				throw new Exception($"{nameof(this.ImportJob)} property has not been initialized!");
			}

			settings.CopyTo(importJob.Settings);
			importJob.Settings.WebServiceURL = AssemblySetup.TestParameters.RelativityWebApiUrl.ToString();
			importJob.Settings.CaseArtifactId = AssemblySetup.TestParameters.WorkspaceId;
			return importJob;
		}

		private void ImportJob_OnError(IDictionary row)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.ErrorRows.Add(row);
				StringBuilder rowMetaData = new StringBuilder();
				foreach (string key in row.Keys)
				{
					if (rowMetaData.Length > 0)
					{
						rowMetaData.Append(",");
					}

					rowMetaData.AppendFormat("{0}={1}", key, row[key]);
				}

				Console.WriteLine("[Job Error Metadata]: " + rowMetaData);
			}
		}

		private void ImportJob_OnMessage(Status status)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.JobMessages.Add(status.Message);
				Console.WriteLine("[Job Message]: " + status.Message);
			}
		}
	}
}
