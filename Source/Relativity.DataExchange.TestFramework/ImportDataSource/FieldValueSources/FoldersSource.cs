// <copyright file="FoldersSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.ImportDataSource.FieldValueSources
{
	using System;
	using System.Collections;

	[Serializable]
	public class FoldersSource : IFieldValueSource
	{
		public FoldersSource(
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
				maxPathDepth: 5,
				numOfDifferentElements: 100,
				numOfDifferentPaths: 1000,
				maxElementLength: 50);
			return randomFolderGenerator.ToFolders();
		}
	}
}
