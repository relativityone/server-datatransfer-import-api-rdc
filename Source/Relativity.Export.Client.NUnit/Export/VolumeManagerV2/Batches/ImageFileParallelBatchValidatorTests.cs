// ----------------------------------------------------------------------------
// <copyright file="ImageFileParallelBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit.Export.VolumeManagerV2.Batches
{
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;

    using global::NUnit.Framework;

    using Relativity.Logging;

    [TestFixture]
	public class ImageFileParallelBatchValidatorTests : ImageFileBatchValidatorTests
	{
		protected override IBatchValidator CreateSut()
		{
			return new ImageFileParallelBatchValidator(_errorFileWriter.Object, _fileHelper.Object, new NullLogger());
		}
	}
}