// <copyright file="ChoicesValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	[Serializable]
	public class ChoicesValueSource : IFieldValueSource
	{
		private readonly Lazy<List<string>> uniqueValues;

		public ChoicesValueSource(
			int numberOfDifferentPaths,
			int maximumPathDepth,
			int numberOfDifferentElements,
			int maximumElementLength)
		: this(
			numberOfDifferentPaths,
			maximumPathDepth,
			numberOfDifferentElements,
			maximumElementLength,
			SettingsConstants.DefaultMultiValueDelimiter,
			SettingsConstants.DefaultNestedValueDelimiter)
		{
		}

		public ChoicesValueSource(
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

			this.uniqueValues = new Lazy<List<string>>(this.CreateAllUniqueValues);
		}

		public int NumberOfDifferentPaths { get; }

		public int MaximumPathDepth { get; }

		public int NumberOfDifferentElements { get; }

		public int MaximumElementLength { get; }

		public char MultiValueDelimiter { get; }

		public char NestedValueDelimiter { get; }

		public int MaxNumberOfMultiValues { get; private set; }

		public List<string> UniqueValues => this.uniqueValues.Value;

		public static ChoicesValueSource CreateSingleChoiceSource(int numberOfDifferentChoices) =>
			CreateSingleChoiceSource(numberOfDifferentChoices, SettingsConstants.DefaultMultiValueDelimiter, SettingsConstants.DefaultNestedValueDelimiter);

		public static ChoicesValueSource CreateSingleChoiceSource(int numberOfDifferentChoices, char multiValueDelimiter, char nestedValueDelimiter)
		{
			var result = new ChoicesValueSource(
				numberOfDifferentPaths: numberOfDifferentChoices,
				maximumPathDepth: 1,
				numberOfDifferentElements: numberOfDifferentChoices,
				maximumElementLength: 50,
				multiValueDelimiter,
				nestedValueDelimiter);
			result.MaxNumberOfMultiValues = 1;
			return result;
		}

		public static ChoicesValueSource CreateMultiChoiceSource(int numberOfDifferentChoices, int maxNumberOfMultiValues) =>
			CreateMultiChoiceSource(numberOfDifferentChoices, SettingsConstants.DefaultMultiValueDelimiter, SettingsConstants.DefaultNestedValueDelimiter, maxNumberOfMultiValues);

		public static ChoicesValueSource CreateMultiChoiceSource(int numberOfDifferentChoices, char multiValueDelimiter, char nestedValueDelimiter, int maxNumberOfMultiValues)
		{
			var result = new ChoicesValueSource(
				numberOfDifferentPaths: numberOfDifferentChoices,
				maximumPathDepth: 5,
				numberOfDifferentElements: numberOfDifferentChoices,
				maximumElementLength: 20,
				multiValueDelimiter,
				nestedValueDelimiter);
			result.MaxNumberOfMultiValues = maxNumberOfMultiValues;
			return result;
		}

		public IEnumerable CreateValuesEnumerator()
		{
			var randomGenerator = new Random(42);

			while (true)
			{
				int numberOfMultiValues = randomGenerator.Next(1, this.MaxNumberOfMultiValues + 1);

				var paths = new string[numberOfMultiValues];
				for (int i = 0; i < numberOfMultiValues; i++)
				{
					int index = randomGenerator.Next(this.uniqueValues.Value.Count);
					paths[i] = this.uniqueValues.Value[index];
				}

				yield return string.Join(this.MultiValueDelimiter.ToString(), paths.Distinct());
			}
		}

		private List<string> CreateAllUniqueValues()
		{
			RandomPathGenerator generator = RandomPathGenerator.GetChoiceGenerator(
				maxPathDepth: this.MaximumPathDepth,
				numOfDifferentElements: this.NumberOfDifferentElements,
				numOfDifferentPaths: this.NumberOfDifferentPaths,
				maxElementLength: this.MaximumElementLength,
				multiValueDelimiter: this.MultiValueDelimiter,
				nestedValueDelimiter: this.NestedValueDelimiter);

			return generator.ToEnumerable(this.NestedValueDelimiter, 1, this.MultiValueDelimiter)
				.Select(x => x.Trim())
				.Distinct()
				.Take(this.NumberOfDifferentElements)
				.ToList();
		}
	}
}
