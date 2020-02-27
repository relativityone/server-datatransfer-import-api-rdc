// <copyright file="IdentifierValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System;
	using System.Collections;

	[Serializable]
	public class IdentifierValueSource : IFieldValueSource
	{
		public IdentifierValueSource()
			: this("IAPI-test")
		{
		}

		public IdentifierValueSource(string identifierPrefix)
		{
			this.IdentifierPrefix = identifierPrefix;
		}

		public string IdentifierPrefix { get; }

		public IEnumerable CreateValuesEnumerator()
		{
			for (int i = 0; ; i++)
			{
				yield return this.GetName(i);
			}
		}

		public string GetName(int index) => $"{this.IdentifierPrefix} - {index}";
	}
}
