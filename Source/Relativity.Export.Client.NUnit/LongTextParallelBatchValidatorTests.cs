// ----------------------------------------------------------------------------
// <copyright file="LongTextParallelBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;

    using global::NUnit.Framework;

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