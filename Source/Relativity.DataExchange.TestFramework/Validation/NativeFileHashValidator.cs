// ----------------------------------------------------------------------------
// <copyright file="NativeFileHashValidator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Validation
{
	using System.Collections.Generic;

	public class NativeFileHashValidator : BaseFileHashValidator
	{
		protected override IEnumerable<string> Files { get; } = TestData.SampleDocFiles;
	}
}