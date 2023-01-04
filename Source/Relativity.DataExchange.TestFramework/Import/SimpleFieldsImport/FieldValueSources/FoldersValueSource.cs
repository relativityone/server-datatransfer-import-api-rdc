// <copyright file="FoldersValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System;
	using System.Collections;

	[Serializable]
	public class FoldersValueSource : IFieldValueSource
	{
		public FoldersValueSource(
			int numberOfDifferentPaths,
			int maximumPathDepth,
			int numberOfDifferentElements,
			int maximumElementLength)
		{
			this.NumberOfDifferentPaths = numberOfDifferentPaths;
			this.MaximumPathDepth = maximumPathDepth;
			this.NumberOfDifferentElements = numberOfDifferentElements;
			this.MaximumElementLength = maximumElementLength;
		}

		public int NumberOfDifferentPaths { get; }

		public int MaximumPathDepth { get; }

		public int NumberOfDifferentElements { get; }

		public int MaximumElementLength { get; }

		public IEnumerable CreateValuesEnumerator()
		{
			RandomPathGenerator randomFolderGenerator = RandomPathGenerator.GetFolderGenerator(
				maxPathDepth: this.MaximumPathDepth,
				numOfDifferentElements: this.NumberOfDifferentElements,
				numOfDifferentPaths: this.NumberOfDifferentPaths,
				maxElementLength: this.MaximumElementLength);
			return randomFolderGenerator.ToFolders();
		}
	}
}
