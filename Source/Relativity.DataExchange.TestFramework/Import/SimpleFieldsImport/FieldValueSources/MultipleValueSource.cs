// <copyright file="MultipleValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	[Serializable]
	public class MultipleValueSource : IFieldValueSource
	{
		public MultipleValueSource(IEnumerable<string> values)
		{
			this.Values = values;
		}

		public IEnumerable<string> Values { get; }

		public IEnumerable CreateValuesEnumerator()
		{
			foreach (var value in Values)
			{
				yield return value;
			}
		}
	}
}
