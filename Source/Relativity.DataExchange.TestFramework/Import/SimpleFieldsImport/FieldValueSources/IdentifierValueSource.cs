// <copyright file="IdentifierValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System;
	using System.Collections;

	[Serializable]
	public class IdentifierValueSource : IFieldValueSourceWithPrefix
	{
		public IdentifierValueSource()
			: this("IAPI-test", 1)
		{
		}

		public IdentifierValueSource(string identifierPrefix)
			: this(identifierPrefix, 1)
		{
		}

		public IdentifierValueSource(int numberOfRepeats)
			: this("IAPI-test", numberOfRepeats)
		{
		}

		public IdentifierValueSource(string identifierPrefix, int numberOfRepeats)
		{
			this.IdentifierPrefix = identifierPrefix;
			this.NumberOfRepeats = numberOfRepeats;
		}

		public string IdentifierPrefix { get; }

		public int NumberOfRepeats { get; }

		public IEnumerable CreateValuesEnumerator()
		{
			for (int i = 0; ; i++)
			{
				for (int repeatIndex = 0; repeatIndex < this.NumberOfRepeats; repeatIndex++)
				{
					yield return this.GetName(i);
				}
			}
		}

		public IFieldValueSourceWithPrefix CreateFieldValueSourceWithPrefix(string prefix)
		{
			return new IdentifierValueSource(prefix, this.NumberOfRepeats);
		}

		public string GetName(int index) => $"{this.IdentifierPrefix} - {index}";
	}
}
