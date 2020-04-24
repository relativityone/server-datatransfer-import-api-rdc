// <copyright file="ObjectNameValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System;
	using System.Collections;
	using System.Linq;

	[Serializable]
	public class ObjectNameValueSource : IFieldValueSource
	{
		private readonly IdentifierValueSource identifierValueSource;
		private readonly int numberOfObjects;
		private readonly int maxNumberOfMultiValues;
		private readonly string multiValuesDelimiter;

		private readonly Random random = new Random(Seed: 42);

		private ObjectNameValueSource(IdentifierValueSource identifierValueSource, int numberOfObjects, int maxNumberOfMultiValues, string multiValuesDelimiter)
		{
			this.identifierValueSource = identifierValueSource;
			this.numberOfObjects = numberOfObjects;
			this.maxNumberOfMultiValues = maxNumberOfMultiValues;
			this.multiValuesDelimiter = multiValuesDelimiter;
		}

		public static ObjectNameValueSource CreateForMultiObjects(
			IdentifierValueSource identifierValueSource,
			int numberOfObjects,
			int maxNumberOfMultiValues)
		{
			return CreateForMultiObjects(
				identifierValueSource,
				numberOfObjects,
				maxNumberOfMultiValues,
				SettingsConstants.DefaultMultiValueDelimiter.ToString());
		}

		public static ObjectNameValueSource CreateForMultiObjects(
			IdentifierValueSource identifierValueSource,
			int numberOfObjects,
			int maxNumberOfMultiValues,
			string multiValuesDelimiter)
		{
			return new ObjectNameValueSource(identifierValueSource, numberOfObjects, maxNumberOfMultiValues, multiValuesDelimiter);
		}

		public static ObjectNameValueSource CreateForSingleObject(IdentifierValueSource identifierValueSource, int numberOfObjects)
		{
			return new ObjectNameValueSource(identifierValueSource, numberOfObjects, 1, string.Empty);
		}

		public IEnumerable CreateValuesEnumerator()
		{
			while (true)
			{
				int numberOfMultiValues = (int)Math.Ceiling(this.random.NextDouble() * this.random.NextDouble() * this.maxNumberOfMultiValues);

				var ids = new string[numberOfMultiValues];
				for (int i = 0; i < numberOfMultiValues; i++)
				{
					int id = this.random.Next(this.numberOfObjects);
					ids[i] = this.identifierValueSource.GetName(id);
				}

				yield return string.Join(this.multiValuesDelimiter, ids.Distinct());
			}
		}
	}
}
