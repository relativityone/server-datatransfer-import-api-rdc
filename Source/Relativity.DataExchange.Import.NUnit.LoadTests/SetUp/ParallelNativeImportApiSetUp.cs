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
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.Import.NUnit.Integration.SetUp;
	using Relativity.DataExchange.TestFramework;

	public class ParallelNativeImportApiSetUp : ImportApiSetUp<ImportBulkArtifactJob, Settings>
	{
		/// <summary>
		/// Arbitrary number that limits the max ImportAPI instance count.
		/// </summary>
		private const int MaxInstanceLimit = 32;

		private readonly List<NativeImportApiSetUp> nativeImportApiSetUpList = new List<NativeImportApiSetUp>();

		private int numberOfDocumentsToImport;

		/// <summary>
		/// Gets or sets the aggregation of all import api instance reports.
		/// </summary>
		public override ImportTestJobResult TestJobResult { get; protected set; } = new ImportTestJobResult();

		public static IEnumerable<object[]> DataReaderToEnumerable(IDataReader dataReader)
		{
			while (dataReader.Read())
			{
				var values = new object[dataReader.FieldCount];
				dataReader.GetValues(values);
				yield return values;
			}
		}

		public ParallelNativeImportApiSetUp ConfigureImportApiInstanceAndDocCounts(int importApiInstanceCount, int documentCountToImport)
		{
			this.ValidateInstanceCountAndDocCountParams(importApiInstanceCount, documentCountToImport);

			this.nativeImportApiSetUpList.Clear();
			for (int index = 0; index < importApiInstanceCount; index++)
			{
				this.nativeImportApiSetUpList.Add(new NativeImportApiSetUp());
			}

			this.numberOfDocumentsToImport = documentCountToImport;
			return this;
		}

		public override void SetUpImportApi(Func<ImportAPI> importApiFunc, Settings settings)
		{
			this.TestJobResult = new ImportTestJobResult();

			this.ValidateImportApiInstanceInitialization();

			this.nativeImportApiSetUpList.ForEach(importApiSetUp => importApiSetUp.SetUpImportApi(importApiFunc, settings));
		}

		public override void Execute(IDataReader dataReader)
		{
			IEnumerable<object[]> importData = DataReaderToEnumerable(dataReader);
			Func<object[], object>[] getters = Enumerable.Range(0, dataReader.FieldCount)
				.Select(p => (Func<object[], object>)(obj => obj[p])).ToArray();
			string[] names = Enumerable.Range(0, dataReader.FieldCount).Select(dataReader.GetName).ToArray();

			this.Execute(importData, getters, names);
		}

		public void Execute(IEnumerable<object[]> importData, Func<object[], object>[] getters, string[] names)
		{
			int batchSize = this.numberOfDocumentsToImport / this.nativeImportApiSetUpList.Count;
			IList<IEnumerable<object[]>> batches = CreateBatchesFrom(importData, batchSize).ToList();
			var tasks = new Task[this.nativeImportApiSetUpList.Count];

			for (int index = 0; index < this.nativeImportApiSetUpList.Count; index++)
			{
				NativeImportApiSetUp nativeSetUp = this.nativeImportApiSetUpList[index];
				IEnumerable<object[]> batch = batches[index];

				tasks[index] = Task.Run(() =>
					{
						using (var dataReader = new EnumerableDataReader<object[]>(batch, getters, names))
						{
							nativeSetUp.Execute(dataReader);
						}
					});
			}

			Task.WaitAll(tasks);

			this.nativeImportApiSetUpList.ForEach(
				importApiSetUp =>
					{
						this.TestJobResult.ProgressCompletedRows.AddRange(importApiSetUp.TestJobResult.ProgressCompletedRows);
						this.TestJobResult.JobFatalExceptions.AddRange(importApiSetUp.TestJobResult.JobFatalExceptions);
						this.TestJobResult.ErrorRows.AddRange(importApiSetUp.TestJobResult.ErrorRows);
						this.TestJobResult.JobMessages.AddRange(importApiSetUp.TestJobResult.JobMessages);
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

		/// <summary>
		/// Extracted to be consistent with other JobResult class properties aggregations.
		/// </summary>
		/// <returns>Return list of the potential fatal exceptions thrown by any import api instance.</returns>
		public IEnumerable<JobReport.RowError> GetErrorRowsFromReport()
		{
			return this.nativeImportApiSetUpList
				.SelectMany(importApiSetUp => importApiSetUp.TestJobResult.CompletedJobReport.ErrorRows);
		}

		public override void Dispose()
		{
			if (this.nativeImportApiSetUpList != null)
			{
				foreach (var setUp in this.nativeImportApiSetUpList)
				{
					setUp.Dispose();
				}
			}
		}

		protected override ImportBulkArtifactJob CreateJobWithSettings(Settings settings)
		{
			throw new NotImplementedException($"This method is not supported in {typeof(ParallelNativeImportApiSetUp)} class and should be never called!");
		}

		private void ValidateInstanceCountAndDocCountParams(int importApiInstanceCount, int documentCountToImport)
		{
			if (importApiInstanceCount < 0 && importApiInstanceCount > MaxInstanceLimit)
			{
				throw new ArgumentException(
					$"{importApiInstanceCount} parameter should be positive number, not greater than {MaxInstanceLimit} limit");
			}

			if (documentCountToImport <= 0)
			{
				throw new ArgumentException($"{documentCountToImport} parameter should be positive number");
			}
		}

		private static IEnumerable<IEnumerable<T>> CreateBatchesFrom<T>(IEnumerable<T> objects, int batchSize)
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

		private void ValidateImportApiInstanceInitialization()
		{
			if (this.nativeImportApiSetUpList == null || this.nativeImportApiSetUpList.Count == 0)
			{
				throw new InvalidOperationException(
					$"ImportAPI instance list has not been initialized. Please call {nameof(this.ConfigureImportApiInstanceAndDocCounts)} method before!");
			}
		}
	}
}
