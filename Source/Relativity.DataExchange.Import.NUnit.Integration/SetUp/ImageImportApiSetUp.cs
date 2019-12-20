// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageImportApiSetUp.cs" company="Relativity ODA LLC">
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
	using System.Globalization;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework.Extensions;

	public class ImageImportApiSetUp : ImportApiSetUp<ImageImportBulkArtifactJob, ImageSettings>
	{
		public override void SetUpImportApi(Func<ImportAPI> importApiFunc, ImageSettings settings)
		{
			base.SetUpImportApi(importApiFunc, settings);

			// Attach native import specific events
			this.ImportJob.OnError += this.ImportJob_OnError;
			this.ImportJob.OnMessage += this.ImportJob_OnMessage;
		}

		public override void Execute(IDataReader dataReader)
		{
			dataReader.ThrowIfNull(nameof(dataReader));

			// Conversion to dataTable is temporary, after DataSource unification it will be not needed
			using (DataTable dataTable = new DataTable())
			{
				dataTable.Locale = CultureInfo.InvariantCulture;

				dataTable.Columns.Add("BatesNumber", typeof(string));
				dataTable.Columns.Add("DocumentIdentifier", typeof(string));
				dataTable.Columns.Add("FileLocation", typeof(string));

				while (dataReader.Read())
				{
					dataTable.Rows.Add(dataReader[0], dataReader[1], dataReader[2]);
				}

				this.ImportJob.SourceData.SourceData = dataTable;
				this.ImportJob.Execute();
			}

			Console.WriteLine("Import API elapsed time: {0}", this.TestJobResult.CompletedJobReport.EndTime - this.TestJobResult.CompletedJobReport.StartTime);
		}

		protected override ImageImportBulkArtifactJob CreateJobWithSettings(ImageSettings settings)
		{
			settings.ThrowIfNull(nameof(settings));
			var importJob = this.ImportApi.NewImageImportJob();

			settings.CopyTo(importJob.Settings);
			importJob.Settings.WebServiceURL = AssemblySetup.TestParameters.RelativityWebApiUrl.ToString();
			importJob.Settings.CaseArtifactId = AssemblySetup.TestParameters.WorkspaceId;
			return importJob;
		}
	}
}