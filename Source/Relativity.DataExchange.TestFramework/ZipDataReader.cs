// ----------------------------------------------------------------------------
// <copyright file="ZipDataReader.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// This class combines a list of IEnumerables and converts that
	/// into IDataReader.
	/// </summary>
	public class ZipDataReader : DataReaderBase
	{
		private readonly List<IEnumerator> enumerators;

		private readonly List<string> ordinalToName;
		private readonly Dictionary<string, int> nameToOrdinal;

		private bool isOpen = true;

		public ZipDataReader()
		{
			this.enumerators = new List<IEnumerator>();
			this.ordinalToName = new List<string>();
			this.nameToOrdinal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		}

		public override int FieldCount => this.enumerators.Count;

		public override int Depth => 0;

		public override bool IsClosed => !this.isOpen;

		public override int RecordsAffected => 0;

		public override object this[int i] => this.enumerators[i].Current;

		public void Add(string name, IEnumerable enumerable)
		{
			name.ThrowIfNull(nameof(name));
			enumerable.ThrowIfNull(nameof(enumerable));

			this.nameToOrdinal.Add(name, this.ordinalToName.Count);
			this.ordinalToName.Add(name);
			this.enumerators.Add(enumerable.GetEnumerator());
		}

		public override bool Read()
		{
			return this.enumerators.All(p => p.MoveNext());
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
			return this.enumerators[i].GetType().GenericTypeArguments.FirstOrDefault() ?? typeof(object);
		}

		public override void Close()
		{
			foreach (IEnumerator enumerator in this.enumerators)
			{
				(enumerator as IDisposable)?.Dispose();
			}

			this.isOpen = false;
		}

		public override bool NextResult()
		{
			return false;
		}
	}
}
