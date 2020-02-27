// ----------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;

	public static class EnumerableExtensions
	{
		public static IEnumerable<TResult> Zip<T1, T2, T3, TResult>(
			this IEnumerable<T1> source,
			IEnumerable<T2> second,
			IEnumerable<T3> third,
			Func<T1, T2, T3, TResult> func)
		{
			using (var e1 = source.GetEnumerator())
			using (var e2 = second.GetEnumerator())
			using (var e3 = third.GetEnumerator())
			{
				while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
				{
					yield return func(e1.Current, e2.Current, e3.Current);
				}
			}
		}

		public static IEnumerable<TResult> Zip<T1, T2, T3, T4, TResult>(
			this IEnumerable<T1> source,
			IEnumerable<T2> second,
			IEnumerable<T3> third,
			IEnumerable<T4> fourth,
			Func<T1, T2, T3, T4, TResult> func)
		{
			using (var e1 = source.GetEnumerator())
			using (var e2 = second.GetEnumerator())
			using (var e3 = third.GetEnumerator())
			using (var e4 = fourth.GetEnumerator())
			{
				while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext() && e4.MoveNext())
				{
					yield return func(e1.Current, e2.Current, e3.Current, e4.Current);
				}
			}
		}

		public static IEnumerable<string> RandomUniqueBatch(this IEnumerable<string> source, int maxBatchSize, char separator)
		{
			var random = new Random(42);
			using (var e = source.GetEnumerator())
			{
				var builder = new StringBuilder();
				var hashSet = new HashSet<string>(CollationStringComparer.SQL_Latin1_General_CP1_CI_AS);
				int batchSize = random.Next(1, maxBatchSize);
				int i = 0;
				while (e.MoveNext())
				{
					if (!hashSet.Add(e.Current))
					{
						continue;
					}

					builder.Append(e.Current);
					++i;
					if (i == batchSize)
					{
						yield return builder.ToString();
						builder.Clear();
						hashSet.Clear();
						batchSize = random.Next(1, maxBatchSize);
						i = 0;
					}
					else
					{
						builder.Append(separator);
					}
				}

				if (i != 0)
				{
					yield return builder.ToString();
				}
			}
		}

		public static ImportDataSource<T> AsImportDataSource<T>(this IEnumerable<T> enumerable)
		{
			enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
			return DtoBasedDataSourceConverter.ConvertDtoCollectionToImportDataSource(enumerable);
		}

		public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int batchSize)
		{
			var batch = new List<T>(batchSize);
			foreach (var item in items)
			{
				batch.Add(item);

				if (batch.Count == batchSize)
				{
					yield return batch;
					batch = new List<T>(batchSize);
				}
			}

			if (batch.Any())
			{
				yield return batch;
			}
		}
	}
}
