// -----------------------------------------------------------------------------------------------------
// <copyright file="ParallelNativeImportExecutionContext.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Import.NUnit.LoadTests.JobExecutionContext
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.Import.NUnit.Integration.JobExecutionContext;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.ImportDataSource;

	public class ParallelNativeImportExecutionContext : IDisposable, IImportApiSetup<Settings>
	{
		/// <summary>
		/// Arbitrary number that limits the max ImportAPI instance count.
		/// </summary>
		private const int MaxInstanceLimit = 32;

		private readonly List<NativeImportExecutionContext> nativeImportExecutionContexts = new List<NativeImportExecutionContext>();
		private readonly List<AppDomain> appDomains = new List<AppDomain>();

		/// <summary>
		/// Gets or sets the aggregation of all import api instance reports.
		/// </summary>
		public ImportTestJobResult TestJobResult { get; protected set; } = new ImportTestJobResult();

		public int CompletedTotalRowsCountFromReport => this.nativeImportExecutionContexts.Sum(context => context.TestJobResult.JobReportTotalRows);

		public IEnumerable<Exception> FatalExceptionsFromReport => this.nativeImportExecutionContexts.Select(context => context.TestJobResult.FatalException);

		public int ErrorRowsCountFromReport => this.nativeImportExecutionContexts.Sum(context => context.TestJobResult.JobReportErrorsCount);

		public ParallelNativeImportExecutionContext ConfigureImportApiInstanceCount(int importApiInstanceCount)
		{
			ValidateInstanceCount(importApiInstanceCount);
			this.nativeImportExecutionContexts.Clear();
			for (int index = 0; index < importApiInstanceCount; index++)
			{
				this.InitializeNewExecutionContextInNewAppDomain(index);
			}

			return this;
		}

		public void SetUpImportApi(Func<ImportAPI> importApiFactory, Settings settings)
		{
			throw new InvalidOperationException("Cannot pass import settings across app domains. Use another overload of this method instead.");
		}

		public void SetUpImportApi(Func<ImportAPI> importApiFactory, ISettingsBuilder<Settings> settingsBuilder)
		{
			this.TestJobResult = new ImportTestJobResult();

			this.ValidateImportApiInstanceInitialization();
			IntegrationTestParameters testParameters = IntegrationTestHelper.ReadIntegrationTestParameters();
			for (int index = 0; index < this.nativeImportExecutionContexts.Count; index++)
			{
				NativeImportExecutionContext context = this.nativeImportExecutionContexts[index];
				context.SetUpImportApi(testParameters, settingsBuilder);
			}
		}

		public async Task<ImportTestJobResult> ExecuteAsync(ImportDataSourceBuilder dataSourceBuilder, int documentCountPerIapiInstance)
		{
			dataSourceBuilder = dataSourceBuilder ?? throw new ArgumentNullException(nameof(dataSourceBuilder));

			var importTasks = new Task<ImportTestJobResult>[this.nativeImportExecutionContexts.Count];
			for (int index = 0; index < this.nativeImportExecutionContexts.Count; index++)
			{
				AppDomain appDomain = this.appDomains[index];
				NativeImportExecutionContext contextInAppDomain = this.nativeImportExecutionContexts[index];

				ImportDataSource<object[]> sourceDataInAppDomain = CreateImportDataSourceInAppDomain(dataSourceBuilder, documentCountPerIapiInstance, appDomain);

				importTasks[index] = Task.Factory.StartNew(
					() => contextInAppDomain.Execute(sourceDataInAppDomain),
					TaskCreationOptions.LongRunning);
			}

			ImportTestJobResult[] testResults = await Task.WhenAll(importTasks).ConfigureAwait(false);

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
				if (this.nativeImportExecutionContexts != null)
				{
					foreach (NativeImportExecutionContext context in this.nativeImportExecutionContexts)
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

		private static void ValidateInstanceCount(int importApiInstanceCount)
		{
			if (importApiInstanceCount < 0 && importApiInstanceCount > MaxInstanceLimit)
			{
				throw new ArgumentException(
					$"{importApiInstanceCount} parameter should be positive number, not greater than {MaxInstanceLimit} limit");
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ConfigureImportApiInstanceCount", Justification = "It is a method name")]
		private void ValidateImportApiInstanceInitialization()
		{
			if (this.nativeImportExecutionContexts == null || this.nativeImportExecutionContexts.Count == 0)
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
			var executionContext = CreateInstanceAndUnwrap<NativeImportExecutionContext>(appDomain);
			this.nativeImportExecutionContexts.Add(executionContext);
		}
	}
}
