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
	using System.Text;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework;

	public abstract class NativeImportJobTestBase : ImportJobTestBase
	{
		protected ImportBulkArtifactJob ImportJob { get; set; }

		[SetUp]
		public void SetupNative()
		{
			this.ImportJob = null;
		}

		[TearDown]
		public void TeardownNative()
		{
			if (this.ImportJob != null)
			{
				this.ImportJob.OnError -= this.ImportJob_OnError;
				this.ImportJob.OnFatalException -= this.ImportJob_OnFatalException;
				this.ImportJob.OnMessage -= this.ImportJob_OnMessage;
				this.ImportJob.OnComplete -= this.ImportJob_OnComplete;
				this.ImportJob.OnProgress -= this.ImportJob_OnProgress;
			}
		}

		protected void WhenExecutingTheJob<T>(IEnumerable<T> importData)
		{
			using (var dataReader = new EnumerableDataReader<T>(importData))
			{
				this.ImportJob.SourceData.SourceData = dataReader;
				this.ImportJob.Execute();
			}

			Console.WriteLine("Import API elapsed time: {0}", this.TestJobResult.CompletedJobReport.EndTime - this.TestJobResult.CompletedJobReport.StartTime);
		}

		protected void GivenDefaultNativeDocumentImportJob()
		{
			this.ImportJob = this.ImportAPI.NewNativeDocumentImportJob();
			this.ImportJob.Settings.WebServiceURL = AssemblySetup.TestParameters.RelativityWebApiUrl.ToString();
			this.ImportJob.Settings.CaseArtifactId = AssemblySetup.TestParameters.WorkspaceId;

			this.ImportJob.Settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			this.ImportJob.Settings.OverwriteMode = OverwriteModeEnum.Append;

			this.ImportJob.Settings.ExtractedTextFieldContainsFilePath = false;
			this.ImportJob.Settings.ExtractedTextEncoding = Encoding.Unicode;

			this.ImportJob.OnError += this.ImportJob_OnError;
			this.ImportJob.OnFatalException += this.ImportJob_OnFatalException;
			this.ImportJob.OnMessage += this.ImportJob_OnMessage;
			this.ImportJob.OnComplete += this.ImportJob_OnComplete;
			this.ImportJob.OnProgress += this.ImportJob_OnProgress;
		}

		protected void GiveNativeFilePathSourceDocumentImportJob()
		{
			this.GivenDefaultNativeDocumentImportJob();

			this.ImportJob.Settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
			this.ImportJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;

			this.ImportJob.Settings.OIFileIdMapped = true;
			this.ImportJob.Settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
			this.ImportJob.Settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;

			this.ImportJob.Settings.FileSizeMapped = true;
			this.ImportJob.Settings.FileSizeColumn = WellKnownFields.NativeFileSize;
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
