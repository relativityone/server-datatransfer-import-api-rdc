// <copyright file="OneValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System;
	using System.Collections;

	[Serializable]
	public class OneValueSource : IFieldValueSource
	{
		public OneValueSource(string value)
		{
			this.Value = value;
		}

		public string Value { get; }

		public IEnumerable CreateValuesEnumerator()
		{
			while (true)
			{
				yield return Value;
			}
		}
	}
}
