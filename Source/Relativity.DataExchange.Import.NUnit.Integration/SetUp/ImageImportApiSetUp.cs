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
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Text;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Extensions;

	public class ImageImportApiSetUp : ImportApiSetUp<ImageImportBulkArtifactJob, ImageSettings>
	{
		public bool UseFileNames { get; set; } = false;

		public bool UseDefaultFieldNames { get; set; } = false;

		public override void SetUpImportApi(Func<ImportAPI> importApiFunc, ImageSettings settings)
		{
			base.SetUpImportApi(importApiFunc, settings);

			// Attach native import specific events
			this.ImportJob.OnError += this.ImportJob_OnError;
			this.ImportJob.OnMessage += this.ImportJob_OnMessage;
		}

		public override void Execute<T>(IEnumerable<T> importData)
		{
			if (!(importData is IEnumerable<ImageImportDto>))
			{
				throw new InvalidOperationException("Unsupported import data type");
			}

			// Convertion to dataTable is temporary, after DataSource unification it will be not needed
			using (DataTable dataTable = new DataTable())
			{
				dataTable.Locale = CultureInfo.InvariantCulture;

				dataTable.Columns.Add(this.UseDefaultFieldNames ? DefaultImageFieldNames.BatesNumber : "Bates_Number", typeof(string));
				dataTable.Columns.Add(this.UseDefaultFieldNames ? DefaultImageFieldNames.DocumentIdentifier : "Document_Identifier", typeof(string));
				dataTable.Columns.Add(this.UseDefaultFieldNames ? DefaultImageFieldNames.FileLocation : "File_Location", typeof(string));
				if (this.UseFileNames)
				{
					dataTable.Columns.Add(this.UseDefaultFieldNames ? DefaultImageFieldNames.FileName : "File_Name", typeof(string));
				}

				using (var dataReader = new EnumerableDataReader<T>(importData))
				{
					while (dataReader.Read())
					{
						if (this.UseFileNames)
						{
							dataTable.Rows.Add(dataReader.GetString(0), dataReader.GetString(1), dataReader.GetString(2), dataReader.GetString(3));
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