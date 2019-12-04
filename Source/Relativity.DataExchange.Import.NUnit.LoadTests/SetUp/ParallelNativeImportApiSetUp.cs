// -----------------------------------------------------------------------------------------------------
// <copyright file="ParallelNativeImportApiSetUp.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Import.NUnit.LoadTests.SetUp
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.Import.NUnit.Integration.SetUp;

	public class ParallelNativeImportApiSetUp : ImportApiSetUp<ImportBulkArtifactJob, Settings>
	{
		/// <summary>
		/// Arbitrary number that limits the max count of ImportAPI instance count.
		/// </summary>
		private const int MaxInstanceLimit = 32;

		private List<NativeImportApiSetUp> nativeImportApiSetUpList;

		private int numberOfDocumentsToImport;

		/// <summary>
		/// Gets or sets the aggregation of all import api instance reports.
		/// </summary>
		public override ImportTestJobResult TestJobResult { get; protected set; } = new ImportTestJobResult();

		public ParallelNativeImportApiSetUp ConfigureImportApiInstanceAndDocCounts(int importApiInstanceCount, int documentCountToImport)
		{
			this.ValidateInstanceCountAndDocCountParams(importApiInstanceCount, documentCountToImport);

			this.nativeImportApiSetUpList = Enumerable.Repeat(new NativeImportApiSetUp(), importApiInstanceCount).ToList();
			this.numberOfDocumentsToImport = documentCountToImport;
			return this;
		}

		public override void SetUpImportApi(Func<ImportAPI> importApiFunc, Settings settings)
		{
			this.TestJobResult = new ImportTestJobResult();

			this.ValidateImportApiInstanceInitialization();

			this.nativeImportApiSetUpList.ForEach(importApiSetUp => importApiSetUp.SetUpImportApi(importApiFunc, settings));
		}

		public override void Execute<T>(IEnumerable<T> importData)
		{
			var tasks = new List<Task>();
			int batchSize = this.numberOfDocumentsToImport / this.nativeImportApiSetUpList.Count;

			IList<IEnumerable<T>> batches = this.CreateBatchesFrom(importData, batchSize).ToList();

			for (int index = 0; index < this.nativeImportApiSetUpList.Count; index++)
			{
				NativeImportApiSetUp nativeSetUp = this.nativeImportApiSetUpList[index];
				IEnumerable<T> batch = batches[index];

				tasks.Add(Task.Run(() => nativeSetUp.Execute(batch)));
			}

			Task.WaitAll(tasks.ToArray());

			this.nativeImportApiSetUpList.ForEach(
				importApiSetUp =>
					{
						this.TestJobResult.ProgressCompletedRows.AddRange(importApiSetUp.TestJobResult.ProgressCompletedRows);
						this.TestJobResult.JobFatalExceptions.AddRange(importApiSetUp.TestJobResult.JobFatalExceptions);
						this.TestJobResult.ErrorRows.AddRange(importApiSetUp.TestJobResult.ErrorRows);
						this.TestJobResult.JobMessages.AddRange(importApiSetUp.TestJobResult.JobMessages);

						importApiSetUp.TestJobResult.CompletedJobReport.ErrorRows.ToList().ForEach(
							errorRow => this.TestJobResult.CompletedJobReport.ErrorRows.Add(errorRow));
					});
		}

		/// <summary>
		/// We cannot perform aggregation in Execute method as TotalRows public property has internal property setter.
		/// </summary>
		/// <returns>Return sum of the processed documents by each instance of import api.</returns>
		public int GetCompletedTotalDocCountFromReport()
		{
			return this.nativeImportApiSetUpList
				.Select(importApiSetUp => importApiSetUp.TestJobResult.CompletedJobReport.TotalRows).Sum();
		}

		/// <summary>
		/// We cannot perform aggregation in Execute method as FatalException public property has internal property setter.
		/// </summary>
		/// <returns>Return list of the potential fatal exceptions thrown by any import api instance.</returns>
		public List<Exception> GetFatalExceptionsFromReport()
		{
			return this.nativeImportApiSetUpList
				.Select(importApiSetUp => importApiSetUp.TestJobResult.CompletedJobReport.FatalException).ToList();
		}

		public override void Dispose()
		{
			this.nativeImportApiSetUpList?.ForEach(setUp => setUp.Dispose());
		}

		protected override ImportBulkArtifactJob CreateJobWithSettings(Settings settings)
		{
			throw new NotImplementedException($"This method is not supported in {typeof(ParallelNativeImportApiSetUp)} class and should be never called!");
		}

		private void ValidateInstanceCountAndDocCountParams(int importApiInstanceCount, int documentCountToImport)
		{
			if (importApiInstanceCount < 0 && importApiInstanceCount > MaxInstanceLimit)
			{
				throw new Exception(
					$"{importApiInstanceCount} parameter should be positive number, not greater than {MaxInstanceLimit} limit");
			}

			if (documentCountToImport <= 0)
			{
				throw new Exception($"{documentCountToImport} parameter should be positive number");
			}
		}

		private void ValidateImportApiInstanceInitialization()
		{
			if (this.nativeImportApiSetUpList == null || this.nativeImportApiSetUpList.Count == 0)
			{
				throw new Exception(
					$"ImportAPI instance list has not been initialized. Please call {nameof(this.ConfigureImportApiInstanceAndDocCounts)} method before!");
			}
		}

		private IEnumerable<IEnumerable<T>> CreateBatchesFrom<T>(IEnumerable<T> objects, int batchSize)
		{
			var batch = new List<T>(batchSize);
			foreach (T item in objects)
			{
				batch.Add(item);
				if (batch.Count == batchSize)
				{
					yield return batch;
					batch = new List<T>(batchSize);
				}
			}

			if (batch.Count > 0)
			{
				yield return batch;
			}
		}
	}
}
