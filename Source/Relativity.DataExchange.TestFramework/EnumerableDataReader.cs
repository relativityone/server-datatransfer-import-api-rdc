// ----------------------------------------------------------------------------
// <copyright file="EnumerableDataReader.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;

	public class EnumerableDataReader<T> : DataReaderBase
	{
		private readonly IEnumerator<T> enumerator;

		private readonly Func<T, object>[] getters;
		private readonly string[] ordinalToName;
		private readonly Dictionary<string, int> nameToOrdinal;

		private bool isOpen = true;

		public EnumerableDataReader(IEnumerable<T> enumerable)
		{
			this.enumerator = enumerable.GetEnumerator();
			PropertyInfo[] properties = typeof(T).GetProperties();
			this.getters = properties
				.Select(p => Delegate.CreateDelegate(typeof(Func<T, object>), p.GetMethod))
				.Cast<Func<T, object>>()
				.ToArray();
			this.ordinalToName = properties
				.Select(p => (p.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute)?.DisplayName ?? p.Name)
				.ToArray();
			this.nameToOrdinal = Enumerable.Range(0, properties.Length)
				.ToDictionary(this.GetName, q => q, StringComparer.OrdinalIgnoreCase);
		}

		public override int FieldCount => this.getters.Length;

		public override int Depth => 0;

		public override bool IsClosed => !this.isOpen;

		public override int RecordsAffected => 0;

		public override object this[int i] => this.getters[i](this.enumerator.Current);

		public override bool Read()
		{
			return this.enumerator.MoveNext();
		}

		public override string GetName(int i)
		{
			return this.ordinalToName[i];
		}

		public override int GetOrdinal(string name)
		{
			if (!this.nameToOrdinal.TryGetValue(name, out int value))
			{
				throw new IndexOutOfRangeException($"Column '{name}' does not belong to the reader.");
			}

			return value;
		}

		public override Type GetFieldType(int i)
		{
			return this.getters[i].Method.ReturnType;
		}

		public override void Close()
		{
			this.enumerator.Dispose();
			this.isOpen = false;
		}

		public override bool NextResult()
		{
			return false;
		}
	}
}
