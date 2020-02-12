// <copyright file="IdentifierSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.ImportDataSource.FieldValueSources
{
	using System;
	using System.Collections;

	[Serializable]
	public class IdentifierSource : IFieldValueSource
	{
		public IdentifierSource()
			: this("IAPI-test")
		{
		}

		public IdentifierSource(string identifierPrefix)
		{
			this.IdentifierPrefix = identifierPrefix;
		}

		public string IdentifierPrefix { get; }

		public IEnumerable CreateValuesEnumerator()
		{
			for (int i = 0; ; i++)
			{
				yield return $"{this.IdentifierPrefix} - {i}";
			}
		}
	}
}
