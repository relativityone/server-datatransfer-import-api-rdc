// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeImportJobTestBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using Castle.Core.Internal;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework;

	public abstract class NativeImportJobTestBase : ImportJobTestBase
	{
		protected Dictionary<string, ImportBulkArtifactJob> ImportJobsDict { get; } = new Dictionary<string, ImportBulkArtifactJob>();

		protected ImportBulkArtifactJob ImportJob
		{
			get
			{
				return this.ImportJobsDict.Values.FirstOrDefault();
			}
		}

		[TearDown]
		public void TeardownNative()
		{
			foreach (var importBulkArtifactJob in this.ImportJobsDict.Values)
			{
				if (importBulkArtifactJob != null)
				{
					importBulkArtifactJob.OnError -= this.ImportJob_OnError;
					importBulkArtifactJob.OnFatalException -= this.ImportJob_OnFatalException;
					importBulkArtifactJob.OnMessage -= this.ImportJob_OnMessage;
					importBulkArtifactJob.OnComplete -= this.ImportJob_OnComplete;
					importBulkArtifactJob.OnProgress -= this.ImportJob_OnProgress;
				}
			}

			this.ImportJobsDict.Clear();
		}

		protected void WhenExecutingTheJob<T>(IEnumerable<T> importData, ImportBulkArtifactJob importJob)
		{
			if (importJob == null)
			{
				importJob = this.ImportJobsDict.Values.First();
			}

			using (var dataReader = new EnumerableDataReader<T>(importData))
			{
				importJob.SourceData.SourceData = dataReader;
				importJob.Execute();
			}

			if (this.ImportJobsDict.Count == 1)
			{
				Console.WriteLine(
					"Import API elapsed time: {0}",
					this.TestJobResult.CompletedJobReports.First().EndTime - this.TestJobResult.CompletedJobReports.First().StartTime);
			}
		}

		protected void WhenExecutingTheJob<T>(IEnumerable<T> importData)
		{
			this.WhenExecutingTheJob(importData, null);
		}

		protected void GivenDefaultNativeDocumentImportJob(string clientId, ImportAPI importApi)
		{
			if (this.ImportJobsDict.ContainsKey(clientId))
			{
				throw new ArgumentException($"Client id {clientId} has been already registered!");
			}

			if (importApi == null)
			{
				importApi = this.ImportAPIInstancesDict.Values.First();
			}

			var importJob = importApi.NewNativeDocumentImportJob();
			importJob.Settings.WebServiceURL = AssemblySetup.TestParameters.RelativityWebApiUrl.ToString();
			importJob.Settings.CaseArtifactId = AssemblySetup.TestParameters.WorkspaceId;

			importJob.Settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			importJob.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;

			importJob.OnError += this.ImportJob_OnError;
			importJob.OnFatalException += this.ImportJob_OnFatalException;
			importJob.OnMessage += this.ImportJob_OnMessage;
			importJob.OnComplete += this.ImportJob_OnComplete;
			importJob.OnProgress += this.ImportJob_OnProgress;

			this.ImportJobsDict[clientId] = importJob;
		}

		protected void GivenDefaultNativeDocumentImportJob()
		{
			this.GivenDefaultNativeDocumentImportJob(string.Empty, null);
		}

		protected void GiveNativeFilePathSourceDocumentImportJob(string clientId, ImportAPI importApi)
		{
			this.GivenDefaultNativeDocumentImportJob(clientId, importApi);

			var importJob = this.ImportJobsDict[clientId];

			importJob.Settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
			importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;

			importJob.Settings.OIFileIdMapped = true;
			importJob.Settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
			importJob.Settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;

			importJob.Settings.FileSizeMapped = true;
			importJob.Settings.FileSizeColumn = WellKnownFields.NativeFileSize;

			importJob.Settings.ApplicationName = clientId;
		}

		protected void GiveNativeFilePathSourceDocumentImportJob()
		{
			this.GiveNativeFilePathSourceDocumentImportJob(string.Empty, null);
		}

		private void ImportJob_OnComplete(JobReport jobReport)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.CompletedJobReports.Add(jobReport);
				Console.WriteLine("[Job Complete]");
			}
		}

		private void ImportJob_OnProgress(long completedRow)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.ProgressCompletedRows.Add(completedRow);
				Console.WriteLine("[Job Progress]: " + completedRow);
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

		private void ImportJob_OnFatalException(JobReport jobReport)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.JobFatalExceptions.Add(jobReport.FatalException);
				Console.WriteLine("[Job Fatal Exception]: " + jobReport.FatalException);
			}
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
	}
}
