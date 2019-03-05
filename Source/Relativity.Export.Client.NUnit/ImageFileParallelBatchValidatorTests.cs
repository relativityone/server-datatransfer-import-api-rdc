// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageFileParallelBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;

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