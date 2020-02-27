// -----------------------------------------------------------------------------------------------------
// <copyright file="BaseExecutionContext.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.TestFramework.Import.JobExecutionContext
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Runtime.Remoting;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Extensions;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.Logging;

	/// <summary>
	/// Execution context for import tests.
	/// </summary>
	/// <remarks>Instances of that class can be used across AppDomains.</remarks>
	/// <typeparam name="TImportJob">Type of the job to test.</typeparam>
	/// <typeparam name="TSettings">Type of settings used for import job.</typeparam>
	public abstract class BaseExecutionContext<TImportJob, TSettings> : MarshalByRefObject, IDisposable, IImportApiSetup<TSettings>
		where TImportJob : IImportNotifier
		where TSettings : ImportSettingsBase
	{
		private readonly object testJobResultLock = new object();
		private bool isDisposed;

		protected BaseExecutionContext()
		{
			this.Logger = IntegrationTestHelper.Logger;
		}

		~BaseExecutionContext()
		{
			this.Dispose(false);
		}

		public ImportTestJobResult TestJobResult { get; protected set; } = new ImportTestJobResult();

		protected ILog Logger { get; }

		protected ImportAPI ImportApi { get; private set; }

		protected TImportJob ImportJob { get; private set; }

		public virtual void SetUpImportApi(Func<ImportAPI> importApiFactory, TSettings settings)
		{
			importApiFactory = importApiFactory ?? throw new ArgumentNullException(nameof(importApiFactory));

			this.TestJobResult = new ImportTestJobResult();
			this.ImportApi = importApiFactory();

			this.ImportJob = this.CreateJobWithSettings(settings);

			this.ImportJob.OnComplete += this.ImportJobOnComplete;
			this.ImportJob.OnProgress += this.ImportJobOnProgress;
			this.ImportJob.OnFatalException += this.ImportJobOnFatalException;
		}

		public virtual void SetUpImportApi(Func<ImportAPI> importApiFactory, ISettingsBuilder<TSettings> settingsBuilder)
		{
			importApiFactory = importApiFactory ?? throw new ArgumentNullException(nameof(importApiFactory));
			settingsBuilder = settingsBuilder ?? throw new ArgumentNullException(nameof(settingsBuilder));

			this.SetUpImportApi(importApiFactory, settingsBuilder.Build());
		}

		public ImportTestJobResult Execute<T>(IEnumerable<T> importData) => this.Execute(importData.AsImportDataSource());

		public ImportTestJobResult Execute<T>(ImportDataSource<T> importData)
		{
			using (var dataReader = new ImportDataSourceToDataReaderAdapter<T>(importData))
			{
				return this.Execute(dataReader);
			}
		}

		public abstract ImportTestJobResult Execute(IDataReader dataReader);

		/// <summary>
		/// Obtains a lifetime service object to control the lifetime policy for this instance.
		/// </summary>
		/// <returns>null, so instances in a AppDomain won't be garbage collected without explicit call to Dispose method.</returns>
		public override object InitializeLifetimeService()
		{
			return null;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected abstract TImportJob CreateJobWithSettings(TSettings settings);

		protected virtual void ImportJobOnError(IDictionary row)
		{
			lock (this.testJobResultLock)
			{
				this.TestJobResult.ErrorRows.Add(row);
				string rowMetaData = string.Join(",", row.Keys.Cast<object>().Select(key => $"{key} {row[key]}"));

				this.Logger.LogError("Job Error Metadata: {rowMetaData}" + rowMetaData);
			}
		}

		protected virtual void ImportJobOnMessage(Status status)
		{
			lock (this.testJobResultLock)
			{
				if (status != null)
				{
					this.TestJobResult.AddMessage(status.Message);
					this.Logger.LogDebug("Job Message: {message}", status.Message);
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.isDisposed)
			{
				return;
			}

			if (disposing)
			{
				if (this.ImportJob != null)
				{
					this.ImportJob.OnComplete -= this.ImportJobOnComplete;
					this.ImportJob.OnProgress -= this.ImportJobOnProgress;
					this.ImportJob.OnFatalException -= this.ImportJobOnFatalException;
				}
			}

			this.DisconnectFromRemoteObject();
			this.isDisposed = true;
		}

		private void ImportJobOnProgress(long completedRow)
		{
			lock (this.testJobResultLock)
			{
				this.TestJobResult.NumberOfCompletedRows++;
			}

			this.Logger.LogDebug("Job Progress: {completedRow}", completedRow);
		}

		private void ImportJobOnComplete(JobReport jobReport)
		{
			lock (this.testJobResultLock)
			{
				this.TestJobResult.CompletedJobReport = jobReport;
			}

			this.Logger.LogInformation("Job Complete");
		}

		private void ImportJobOnFatalException(JobReport jobReport)
		{
			lock (this.testJobResultLock)
			{
				this.TestJobResult.JobFatalExceptions.Add(jobReport.FatalException);
			}

			this.Logger.LogError(jobReport.FatalException, "Job Fatal Exception");
		}

		private void DisconnectFromRemoteObject()
		{
			RemotingServices.Disconnect(this);
		}
	}
}
