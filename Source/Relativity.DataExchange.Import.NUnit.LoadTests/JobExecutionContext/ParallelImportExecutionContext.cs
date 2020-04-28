// // ----------------------------------------------------------------------------
// <copyright file="ParallelImportExecutionContext.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.LoadTests.JobExecutionContext
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;

	public class ParallelImportExecutionContext<TExecutionContext, TSettings> : IDisposable, IImportApiSetup<TSettings>
		where TSettings : ImportSettingsBase
		where TExecutionContext : IExecutionContext, IImportApiSetup<TSettings>, new()
	{
		/// <summary>
		/// Arbitrary number that limits the max ImportAPI instance count.
		/// </summary>
		private const int MaxInstanceLimit = 32;

		private readonly List<TExecutionContext> importExecutionContexts = new List<TExecutionContext>();
		private readonly List<AppDomain> appDomains = new List<AppDomain>();

		/// <summary>
		/// Gets or sets the aggregation of all import api instance reports.
		/// </summary>
		public ImportTestJobResult TestJobResult { get; protected set; } = new ImportTestJobResult();

		public int CompletedTotalRowsCountFromReport => this.importExecutionContexts.Sum(context => context.TestJobResult.JobReportTotalRows);

		public IEnumerable<Exception> FatalExceptionsFromReport => this.importExecutionContexts.Select(context => context.TestJobResult.FatalException);

		public int ErrorRowsCountFromReport => this.importExecutionContexts.Sum(context => context.TestJobResult.JobReportErrorsCount);

		public ParallelImportExecutionContext<TExecutionContext, TSettings> ConfigureImportApiInstanceCount(int importApiInstanceCount)
		{
			ValidateInstanceCount(importApiInstanceCount);
			this.importExecutionContexts.Clear();
			for (int index = 0; index < importApiInstanceCount; index++)
			{
				this.InitializeNewExecutionContextInNewAppDomain(index);
			}

			return this;
		}

		public void SetUpImportApi(Func<ImportAPI> importApiFactory, TSettings settings)
		{
			throw new InvalidOperationException("Cannot pass import settings across app domains. Use another overload of this method instead.");
		}

		public void SetUpImportApi(Func<ImportAPI> importApiFactory, ISettingsBuilder<TSettings> settingsBuilder) => this.SetUpImportApi(settingsBuilder);

		public void SetUpImportApi(IntegrationTestParameters parameters, ISettingsBuilder<TSettings> settingsBuilder) => this.SetUpImportApi(settingsBuilder);

		public async Task<ImportTestJobResult> ExecuteAsync(ImportDataSourceBuilder dataSourceBuilder, int documentCountPerIApiInstance)
		{
			dataSourceBuilder = dataSourceBuilder ?? throw new ArgumentNullException(nameof(dataSourceBuilder));

			PerformanceDataCollector.Instance.StartMeasureTime();

			var importTasks = new Task<ImportTestJobResult>[this.importExecutionContexts.Count];
			for (int index = 0; index < this.importExecutionContexts.Count; index++)
			{
				AppDomain appDomain = this.appDomains[index];
				TExecutionContext contextInAppDomain = this.importExecutionContexts[index];

				ImportDataSource<object[]> sourceDataInAppDomain = CreateImportDataSourceInAppDomain(dataSourceBuilder, documentCountPerIApiInstance, appDomain);

				importTasks[index] = Task.Factory.StartNew(
					() => contextInAppDomain.Execute(sourceDataInAppDomain),
					TaskCreationOptions.LongRunning);
			}

			ImportTestJobResult[] testResults = await Task.WhenAll(importTasks).ConfigureAwait(false);

			PerformanceDataCollector.Instance.StopMeasureTime();
			foreach (ImportTestJobResult testResult in testResults)
			{
				this.TestJobResult.NumberOfCompletedRows += testResult.NumberOfCompletedRows;
				this.TestJobResult.JobFatalExceptions.AddRange(testResult.JobFatalExceptions);
				this.TestJobResult.ErrorRows.AddRange(testResult.ErrorRows);
				this.TestJobResult.NumberOfJobMessages += testResult.NumberOfJobMessages;
			}

			return this.TestJobResult;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.importExecutionContexts != null)
				{
					foreach (TExecutionContext context in this.importExecutionContexts)
					{
						context.Dispose();
					}
				}

				foreach (var appDomain in this.appDomains)
				{
					AppDomain.Unload(appDomain);
				}
			}
		}

		private static ImportDataSource<object[]> CreateImportDataSourceInAppDomain(
			ImportDataSourceBuilder dataSourceBuilder,
			int documentCountPerImportApiInstance,
			AppDomain appDomain)
		{
			var dataSourceBuilderInAppDomain = CreateInstanceAndUnwrap<ImportDataSourceBuilder>(appDomain);
			dataSourceBuilder.CopyTo(dataSourceBuilderInAppDomain, appDomain.FriendlyName);

			return dataSourceBuilderInAppDomain.Build(documentCountPerImportApiInstance);
		}

		private static void ValidateInstanceCount(int importApiInstanceCount)
		{
			if (importApiInstanceCount < 0 || importApiInstanceCount > MaxInstanceLimit)
			{
				throw new ArgumentException(
					$"{importApiInstanceCount} parameter should be positive number, not greater than {MaxInstanceLimit} limit");
			}
		}

		private static T CreateInstanceAndUnwrap<T>(AppDomain appDomain)
		{
			string typeName = typeof(T).FullName;

			if (typeName == null)
			{
				throw new InvalidOperationException($"Unsupported type: {typeof(T)} passed as a generic parameter.");
			}

			return (T)appDomain.CreateInstanceAndUnwrap(
				typeof(T).Assembly.FullName,
				typeName);
		}

		private void SetUpImportApi(ISettingsBuilder<TSettings> settingsBuilder)
		{
			this.TestJobResult = new ImportTestJobResult();

			this.ValidateImportApiInstanceInitialization();
			IntegrationTestParameters testParameters = IntegrationTestHelper.IntegrationTestParameters;
			foreach (var context in this.importExecutionContexts)
			{
				context.SetUpImportApi(testParameters, settingsBuilder);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ConfigureImportApiInstanceCount", Justification = "It is a method name")]
		private void ValidateImportApiInstanceInitialization()
		{
			if (this.importExecutionContexts == null || this.importExecutionContexts.Count == 0)
			{
				throw new InvalidOperationException(
					$"ImportAPI instance list has not been initialized. Please call {nameof(this.ConfigureImportApiInstanceCount)} method first.");
			}
		}

		private void InitializeNewExecutionContextInNewAppDomain(int index)
		{
			var domainSetup = new System.AppDomainSetup
			{
				ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
			};
			AppDomain appDomain = AppDomain.CreateDomain(
				$"Parallel-Import-Test-{index}",
				AppDomain.CurrentDomain.Evidence,
				domainSetup);
			this.appDomains.Add(appDomain);
			var appDomainSetup = CreateInstanceAndUnwrap<AppDomainSetup>(appDomain);
			appDomainSetup.SetupAppDomain(AssemblySetup.TestParameters);
			var executionContext = CreateInstanceAndUnwrap<TExecutionContext>(appDomain);
			this.importExecutionContexts.Add(executionContext);
		}
	}
}