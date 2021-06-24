﻿// ----------------------------------------------------------------------------
// <copyright file="SearchablePdfFileHashValidator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Validation
{
	using System.Collections.Generic;

	public class SearchablePdfFileHashValidator : BaseFileHashValidator
	{
		protected override IEnumerable<string> Files => TestData.SampleSearchablePdfTestFiles;
	}
}