// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextParallelBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
    using Relativity.Logging;

    [TestFixture]
	public class LongTextParallelBatchValidatorTests : LongTextBatchValidatorTests
	{
		protected override IBatchValidator CreateValidator()
		{
			return new LongTextParallelBatchValidator(LongTextRepository.Object, FileHelper.Object, Status.Object, new NullLogger());
		}
	}
}