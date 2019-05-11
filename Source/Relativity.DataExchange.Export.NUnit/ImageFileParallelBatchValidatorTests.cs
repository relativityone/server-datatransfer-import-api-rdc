// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageFileParallelBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.Logging;

	[TestFixture]
	public class ImageFileParallelBatchValidatorTests : ImageFileBatchValidatorTests
	{
		protected override IBatchValidator CreateSut()
		{
			return new ImageFileParallelBatchValidator(this.ErrorFileWriter.Object, this.FileHelper.Object, new NullLogger());
		}
	}
}