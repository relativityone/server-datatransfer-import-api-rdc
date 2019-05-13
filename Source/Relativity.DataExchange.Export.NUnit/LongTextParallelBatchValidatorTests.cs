// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextParallelBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.Logging;

	[TestFixture]
	public class LongTextParallelBatchValidatorTests : LongTextBatchValidatorTests
	{
		protected override IBatchValidator CreateValidator()
		{
			return new LongTextParallelBatchValidator(this.LongTextRepository.Object, this.FileHelper.Object, this.Status.Object, new NullLogger());
		}
	}
}