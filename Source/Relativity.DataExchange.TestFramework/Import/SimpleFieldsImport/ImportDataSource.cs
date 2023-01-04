// <copyright file="ImportDataSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport
{
	using System;
	using System.Collections.Generic;

	public class ImportDataSource<T> : MarshalByRefObject
	{
		public ImportDataSource(IEnumerable<T> enumerable, string[] fieldNames, Func<T, object>[] getters)
		{
			fieldNames = fieldNames ?? throw new ArgumentNullException(nameof(fieldNames));

			this.Rows = enumerable;
			this.FieldNames = fieldNames;
			this.ValueGetters = getters;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "It is a class used in tests only.")]
		public string[] FieldNames { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "It is a class used in tests only.")]
		public Func<T, object>[] ValueGetters { get; }

		public IEnumerable<T> Rows { get; }
	}
}
