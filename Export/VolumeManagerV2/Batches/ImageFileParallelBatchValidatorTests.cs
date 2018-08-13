using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Batches
{
	[TestFixture]
	public class ImageFileParallelBatchValidatorTests : ImageFileBatchValidatorTests
	{
		protected override IBatchValidator CreateSut()
		{
			return new ImageFileParallelBatchValidator(_errorFileWriter.Object, _fileHelper.Object, new NullLogger());
		}
	}
}