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
	using System.Collections;
	using System.Data;
	using System.Linq;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Logging;

	public abstract class ImportApiSetUp<TImportJob, TSettings> : IDisposable
		where TImportJob : IImportNotifier
		where TSettings : ImportSettingsBase
	{
		private readonly ILog logger;

		protected ImportApiSetUp()
		{
			this.logger = IntegrationTestHelper.Logger;
		}

		public virtual ImportTestJobResult TestJobResult { get; protected set; } = new ImportTestJobResult();

		protected ImportAPI ImportApi { get; private set; }

		protected TImportJob ImportJob { get; private set; }

		public virtual void SetUpImportApi(Func<ImportAPI> importApiFunc, TSettings settings)
		{
			this.TestJobResult = new ImportTestJobResult();
			this.ImportApi = importApiFunc.Invoke();

			this.ImportJob = this.CreateJobWithSettings(settings);

			this.ImportJob.OnComplete += this.ImportJob_OnComplete;
			this.ImportJob.OnProgress += this.ImportJob_OnProgress;
			this.ImportJob.OnFatalException += this.ImportJob_OnFatalException;
		}

		public virtual void Dispose()
		{
			if (this.ImportJob != null)
			{
				this.ImportJob.OnComplete -= this.ImportJob_OnComplete;
				this.ImportJob.OnProgress -= this.ImportJob_OnProgress;
				this.ImportJob.OnFatalException -= this.ImportJob_OnFatalException;
			}
		}

		public abstract void Execute(IDataReader dataReader);

		protected abstract TImportJob CreateJobWithSettings(TSettings settings);

		protected virtual void ImportJob_OnError(IDictionary row)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.ErrorRows.Add(row);
				string rowMetaData = string.Join(",", row.Keys.Cast<object>().Select(key => $"{key} {row[key]}"));

				this.logger.LogError("Job Error Metadata: {rowMetaData}" + rowMetaData);
			}
		}

		protected virtual void ImportJob_OnMessage(Status status)
		{
			lock (this.TestJobResult)
			{
				if (status != null)
				{
					this.TestJobResult.AddMessage(status.Message);
					this.logger.LogDebug("Job Message: {message}", status.Message);
				}
			}
		}

		private void ImportJob_OnProgress(long completedRow)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.NumberOfCompletedRows++;
			}

			this.logger.LogInformation("Job Progress: {completedRow}", completedRow);
		}

		private void ImportJob_OnComplete(JobReport jobReport)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.CompletedJobReport = jobReport;
			}

			this.logger.LogInformation("Job Complete");
		}

		private void ImportJob_OnFatalException(JobReport jobReport)
		{
			lock (this.TestJobResult)
			{
				this.TestJobResult.JobFatalExceptions.Add(jobReport.FatalException);
			}

			this.logger.LogError(jobReport.FatalException, "Job Fatal Exception");
		}
	}
}
