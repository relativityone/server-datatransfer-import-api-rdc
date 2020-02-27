// ----------------------------------------------------------------------------
// <copyright file="ImportDataSourceToDataReaderAdapter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// This class is an adapter that converts ImportDataSource&lt;T&gt; to IDataReader.
	/// </summary>
	/// <typeparam name="T">Source of row data.</typeparam>
	public class ImportDataSourceToDataReaderAdapter<T> : DataReaderBase
	{
		private readonly IEnumerator<T> enumerator;

		private readonly Func<T, object>[] getters;
		private readonly string[] ordinalToName;
		private readonly Dictionary<string, int> nameToOrdinal;

		private bool isOpen = true;

		public ImportDataSourceToDataReaderAdapter(ImportDataSource<T> dataSource)
		{
			dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

			this.enumerator = dataSource.Rows.GetEnumerator();
			this.getters = dataSource.ValueGetters;
			this.ordinalToName = dataSource.FieldNames;
			this.nameToOrdinal = Enumerable.Range(0, this.ordinalToName.Length)
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "IAPI implementation requires that exception type to be thrown.")]
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
