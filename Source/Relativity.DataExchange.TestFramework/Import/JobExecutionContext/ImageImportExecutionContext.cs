// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageImportExecutionContext.cs" company="Relativity ODA LLC">
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
	using System.Globalization;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework.Extensions;

	/// <summary>
	/// Execution context for image import tests.
	/// </summary>
	/// <remarks>Instances of that class can be used across AppDomains.</remarks>
	public class ImageImportExecutionContext : BaseExecutionContext<ImageImportBulkArtifactJob, ImageSettings>
	{
		public bool UseDataTableSource { get; set; } = false;

		public bool UseFileNames { get; set; } = false;

		public bool UseDefaultFieldNames { get; set; } = false;

		public override void SetUpImportApi(Func<ImportAPI> importApiFactory, ImageSettings settings)
		{
			base.SetUpImportApi(importApiFactory, settings);

			// Attach native import specific events
			this.ImportJob.OnError += this.ImportJobOnError;
			this.ImportJob.OnMessage += this.ImportJobOnMessage;
		}

		public override ImportTestJobResult Execute(IDataReader dataReader)
		{
			dataReader.ThrowIfNull(nameof(dataReader));
			if (this.UseDataTableSource)
			{
				this.ExecuteObsoleteDataSourceType(dataReader);
			}
			else
			{
				this.ImportJob.SourceData.Reader = dataReader;
				this.ImportJob.Execute();
			}

			Console.WriteLine(
				"Import API elapsed time: {0}",
				this.TestJobResult.EndTime - this.TestJobResult.StartTime);

			return this.TestJobResult;
		}

		protected override ImageImportBulkArtifactJob CreateJobWithSettings(ImageSettings settings)
		{
			settings.ThrowIfNull(nameof(settings));
			var importJob = this.ImportApi.NewImageImportJob();

			settings.CopyTo(importJob.Settings);

			IntegrationTestParameters testParameters = IntegrationTestHelper.IntegrationTestParameters;
			importJob.Settings.WebServiceURL = testParameters.RelativityWebApiUrl.ToString();
			importJob.Settings.CaseArtifactId = testParameters.WorkspaceId;

			return importJob;
		}

		private void ExecuteObsoleteDataSourceType(IDataReader dataReader)
		{
			using (DataTable dataTable = new DataTable())
			{
				dataTable.Locale = CultureInfo.InvariantCulture;

				// properties from derived class come first in dataReader
				if (this.UseFileNames)
				{
					dataTable.Columns.Add(
						this.UseDefaultFieldNames ? DefaultImageFieldNames.FileName : "File_Name",
						typeof(string));
				}

				dataTable.Columns.Add(
					this.UseDefaultFieldNames ? DefaultImageFieldNames.BatesNumber : "Bates_Number",
					typeof(string));
				dataTable.Columns.Add(
					this.UseDefaultFieldNames ? DefaultImageFieldNames.DocumentIdentifier : "Document_Identifier",
					typeof(string));
				dataTable.Columns.Add(
					this.UseDefaultFieldNames ? DefaultImageFieldNames.FileLocation : "File_Location",
					typeof(string));

				while (dataReader.Read())
				{
					if (this.UseFileNames)
					{
						dataTable.Rows.Add(
							dataReader.GetString(0),
							dataReader.GetString(1),
							dataReader.GetString(2),
							dataReader.GetString(3));
					}
					else
					{
						dataTable.Rows.Add(dataReader.GetString(0), dataReader.GetString(1), dataReader.GetString(2));
					}
				}

				this.ImportJob.SourceData.SourceData = dataTable;
				this.ImportJob.Execute();
			}
		}
	}
}