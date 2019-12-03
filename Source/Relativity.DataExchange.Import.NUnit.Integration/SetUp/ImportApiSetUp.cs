// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportApiSetUp.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	public abstract class ImportApiSetUp<TImportJob, TSettings> : IDisposable
		where TImportJob : IImportNotifier
		where TSettings : ImportSettingsBase
	{
		public ImportTestJobResult TestJobResult { get; private set; }

		protected ImportAPI ImportApi { get; private set; }

		protected TImportJob ImportJob { get; private set; }

		public virtual ImportApiSetUp<TImportJob, TSettings> SetUpImportApi(ImportAPI importApi, TSettings settings)
		{
			this.ImportApi = importApi;

			this.ImportJob = this.CreateJobWithSettings(settings);

			this.ImportJob.OnComplete += this.ImportJob_OnComplete;
			this.ImportJob.OnProgress += this.ImportJob_OnProgress;

			this.ImportJob.OnFatalException += this.ImportJob_OnFatalException;

			return this;
		}

		public void Dispose()
		{
			if (this.ImportJob != null)
			{
				this.ImportJob.OnComplete -= this.ImportJob_OnComplete;
				this.ImportJob.OnProgress -= this.ImportJob_OnProgress;
				this.ImportJob.OnFatalException -= this.ImportJob_OnFatalException;
			}
		}

		public abstract ImportApiSetUp<TImportJob, TSettings> Execute<T>(IEnumerable<T> importData);

		protected abstract TImportJob CreateJobWithSettings(TSettings settings);

		private void ImportJob_OnProgress(long completedRow)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.ProgressCompletedRows.Add(completedRow);
				Console.WriteLine("[Job Progress]: " + completedRow);
			}
		}

		private void ImportJob_OnComplete(JobReport jobReport)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.CompletedJobReport = jobReport;
				Console.WriteLine("[Job Complete]");
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
	}
}
