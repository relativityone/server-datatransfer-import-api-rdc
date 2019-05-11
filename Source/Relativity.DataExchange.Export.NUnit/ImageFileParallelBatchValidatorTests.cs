// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageFileParallelBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
    using Relativity.Logging;

    [TestFixture]
	public class ImageFileParallelBatchValidatorTests : ImageFileBatchValidatorTests
	{
		protected override IBatchValidator CreateSut()
		{
			return new ImageFileParallelBatchValidator(ErrorFileWriter.Object, FileHelper.Object, new NullLogger());
		}
	}
}