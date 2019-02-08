using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Batches
{
	[TestFixture]
	public class NativeFileParallelBatchValidatorTests: NativeFileBatchValidatorTests
	{
		protected override IBatchValidator CreateValidator()
		{
			return new NativeFileParallelBatchValidator(ErrorFileWriter.Object, FileHelper.Object, Status.Object, new NullLogger());
		}
	}
}