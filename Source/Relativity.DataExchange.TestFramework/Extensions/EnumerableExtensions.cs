// ----------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Text;

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
	}
}
