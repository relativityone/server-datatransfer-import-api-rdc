// <copyright file="ChoicesSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.ImportDataSource.FieldValueSources
{
	using System;
	using System.Collections;

	[Serializable]
	public class ChoicesSource : IFieldValueSource
	{
		public ChoicesSource(
			int numberOfDifferentPaths,
			int maximumPathDepth,
			int numberOfDifferentElements,
			int maximumElementLength,
			char multiValueDelimiter,
			char nestedValueDelimiter)
		{
			this.NumberOfDifferentPaths = numberOfDifferentPaths;
			this.MaximumPathDepth = maximumPathDepth;
			this.NumberOfDifferentElements = numberOfDifferentElements;
			this.MaximumElementLength = maximumElementLength;
			this.MultiValueDelimiter = multiValueDelimiter;
			this.NestedValueDelimiter = nestedValueDelimiter;
		}

		public int NumberOfDifferentPaths { get; }

		public int MaximumPathDepth { get; }

		public int NumberOfDifferentElements { get; }

		public int MaximumElementLength { get; }

		public char MultiValueDelimiter { get; }

		public char NestedValueDelimiter { get; }

		public IEnumerable CreateValuesEnumerator()
		{
			RandomPathGenerator generator = RandomPathGenerator.GetChoiceGenerator(
				maxPathDepth: this.MaximumPathDepth,
				numOfDifferentElements: this.NumberOfDifferentElements,
				numOfDifferentPaths: this.NumberOfDifferentPaths,
				maxElementLength: this.MaximumElementLength,
				multiValueDelimiter: this.MultiValueDelimiter,
				nestedValueDelimiter: this.NestedValueDelimiter);

			return generator.ToEnumerable();
		}
	}
}
