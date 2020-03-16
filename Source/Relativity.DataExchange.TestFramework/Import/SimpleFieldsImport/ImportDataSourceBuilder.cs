// <copyright file="ImportDataSourceBuilder.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;

	public sealed class ImportDataSourceBuilder : MarshalByRefObject
	{
		private readonly List<string> fieldNames;
		private readonly List<IFieldValueSource> fieldValuesSource;
		private readonly List<IEnumerator> enumerators;

		private bool canCopy = true;

		public ImportDataSourceBuilder()
		{
			this.fieldNames = new List<string>();
			this.fieldValuesSource = new List<IFieldValueSource>();
			this.enumerators = new List<IEnumerator>();
		}

		public static ImportDataSourceBuilder New()
		{
			return new ImportDataSourceBuilder();
		}

		public ImportDataSourceBuilder AddField(string fieldName, IEnumerable enumerable)
		{
			fieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
			enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));

			this.canCopy = false;

			this.fieldNames.Add(fieldName);
			this.enumerators.Add(enumerable.GetEnumerator());

			return this;
		}

		public ImportDataSourceBuilder AddField(string fieldName, IFieldValueSource fieldValueSource)
		{
			fieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
			fieldValueSource = fieldValueSource ?? throw new ArgumentNullException(nameof(fieldValueSource));

			IEnumerable enumerable = fieldValueSource.CreateValuesEnumerator();

			this.fieldNames.Add(fieldName);
			this.enumerators.Add(enumerable.GetEnumerator());
			this.fieldValuesSource.Add(fieldValueSource);

			return this;
		}

		/// <summary>
		/// It copies current builder instance into <paramref name="destinationDataSourceBuilder"/>.
		/// That method can be called only if all fields were added using <see cref="IFieldValueSource"/>.
		/// </summary>
		/// <param name="destinationDataSourceBuilder">Destination builder.</param>
		/// <param name="newIdentifierPrefix">Prefix which will be added to identifier field values.</param>
		public void CopyTo(ImportDataSourceBuilder destinationDataSourceBuilder, string newIdentifierPrefix)
		{
			destinationDataSourceBuilder = destinationDataSourceBuilder ?? throw new ArgumentNullException(nameof(destinationDataSourceBuilder));
			newIdentifierPrefix = newIdentifierPrefix ?? throw new ArgumentNullException(nameof(newIdentifierPrefix));

			if (!this.canCopy)
			{
				throw new InvalidOperationException("Cannot copy this builder to the destination builder, because at least one enumerator was added directly.");
			}

			for (int i = 0; i < this.fieldNames.Count; i++)
			{
				IFieldValueSource fieldsValueSource = this.fieldValuesSource[i];
				IFieldValueSource newFieldsValuesSource = fieldsValueSource is IFieldValueSourceWithPrefix fieldValueSourceWithPrefix ? fieldValueSourceWithPrefix.CreateFieldValueSourceWithPrefix(newIdentifierPrefix) : fieldsValueSource;

				destinationDataSourceBuilder.AddField(this.fieldNames[i], newFieldsValuesSource);
			}
		}

		public ImportDataSource<object[]> Build(int count)
		{
			return this.Build(index => index < count);
		}

		public ImportDataSource<object[]> Build()
		{
			return this.Build(_ => true);
		}

		private static Func<object[], object> CreateValueGetter(int index) => row => row[index];

		private ImportDataSource<object[]> Build(Func<int, bool> predicate)
		{
			return new ImportDataSource<object[]>(this.EnumerateRows(predicate), this.fieldNames.ToArray(), this.CreateValueGetters());
		}

		private IEnumerable<object[]> EnumerateRows(Func<int, bool> predicate)
		{
			int length = this.enumerators.Count;
			for (int rowIndex = 0; predicate(rowIndex); rowIndex++)
			{
				var result = new object[length];
				for (int i = 0; i < length; i++)
				{
					if (!this.enumerators[i].MoveNext())
					{
						yield break;
					}

					result[i] = this.enumerators[i].Current;
				}

				yield return result;
			}
		}

		private Func<object[], object>[] CreateValueGetters() =>
			Enumerable
				.Range(0, this.enumerators.Count)
				.Select(CreateValueGetter)
				.ToArray();
	}
}
