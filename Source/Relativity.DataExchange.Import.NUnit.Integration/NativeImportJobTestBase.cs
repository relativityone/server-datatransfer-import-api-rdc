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
	using System.Data;
	using System.Data.Common;
	using System.Globalization;
	using System.Text;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework;

	public abstract class NativeImportJobTestBase : ImportJobTestBase
	{
		private ImportBulkArtifactJob importJob;

		[SetUp]
		public void SetupNative()
		{
			this.importJob = null;
		}

		[TearDown]
		public void TeardownNative()
		{
			if (this.importJob != null)
			{
				this.importJob.OnError -= this.ImportJob_OnError;
				this.importJob.OnFatalException -= this.ImportJob_OnFatalException;
				this.importJob.OnMessage -= this.ImportJob_OnMessage;
				this.importJob.OnComplete -= this.ImportJob_OnComplete;
				this.importJob.OnProgress -= this.ImportJob_OnProgress;
			}
		}

		protected static DataTable GivenDefaultNativeTable()
		{
			var table = new DataTable { Locale = CultureInfo.InvariantCulture };
			table.Columns.Add(WellKnownFields.ControlNumber, typeof(string));
			table.Columns.Add(WellKnownFields.FilePath, typeof(string));
			return table;
		}

		protected DbDataReader GivenDbDataReaderForNativeTable(DataTable table, List<string> files)
		{
			foreach (string file in files)
			{
				this.GivenTheDatasetPathToImport(table, file);
			}

			return table.CreateDataReader();
		}

		protected void WhenExecutingTheJob(IDataReader reader)
		{
			this.importJob.SourceData.SourceData = reader;
			this.importJob.Execute();
			Console.WriteLine("Import API elapsed time: {0}", this.TestJobResult.CompletedJobReport.EndTime - this.TestJobResult.CompletedJobReport.StartTime);
		}

		protected void GivenDefaultNativeDocumentImportJob()
		{
			this.importJob = this.ImportAPI.NewNativeDocumentImportJob();
			this.importJob.Settings.WebServiceURL = AssemblySetup.TestParameters.RelativityWebApiUrl.ToString();
			this.importJob.Settings.CaseArtifactId = AssemblySetup.TestParameters.WorkspaceId;
			this.importJob.Settings.ArtifactTypeId = (int)ArtifactType.Document;
			this.importJob.Settings.ExtractedTextFieldContainsFilePath = false;
			this.importJob.Settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
			this.importJob.Settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			this.importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			this.importJob.Settings.OverwriteMode = OverwriteModeEnum.Append;
			this.importJob.Settings.OIFileIdMapped = true;
			this.importJob.Settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
			this.importJob.Settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;
			this.importJob.Settings.ExtractedTextEncoding = Encoding.Unicode;
			this.importJob.Settings.FileSizeMapped = true;
			this.importJob.Settings.FileSizeColumn = WellKnownFields.NativeFileSize;

			this.importJob.OnError += this.ImportJob_OnError;
			this.importJob.OnFatalException += this.ImportJob_OnFatalException;
			this.importJob.OnMessage += this.ImportJob_OnMessage;
			this.importJob.OnComplete += this.ImportJob_OnComplete;
			this.importJob.OnProgress += this.ImportJob_OnProgress;
		}

		private void ImportJob_OnComplete(JobReport jobReport)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.CompletedJobReport = jobReport;
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
