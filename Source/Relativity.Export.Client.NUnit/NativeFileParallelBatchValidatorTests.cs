// ----------------------------------------------------------------------------
// <copyright file="NativeFileParallelBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;

    using global::NUnit.Framework;

    using Relativity.Logging;

    [TestFixture]
	public class NativeFileParallelBatchValidatorTests: NativeFileBatchValidatorTests
	{
		protected override IBatchValidator CreateValidator()
		{
			return new NativeFileParallelBatchValidator(ErrorFileWriter.Object, FileHelper.Object, Status.Object, new NullLogger());
		}
	}
}