// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeFileParallelBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.Logging;

	[TestFixture]
	public class NativeFileParallelBatchValidatorTests : NativeFileBatchValidatorTests
	{
		protected override IBatchValidator CreateValidator()
		{
			return new NativeFileParallelBatchValidator(this.ErrorFileWriter.Object, this.FileHelper.Object, this.Status.Object, new NullLogger());
		}
	}
}