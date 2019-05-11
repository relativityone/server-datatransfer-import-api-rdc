// -----------------------------------------------------------------------------------------------------
// <copyright file="SinglePageImagesRollupTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System.Collections;

    using global::NUnit.Framework;

    using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.ImagesRollup;

	[TestFixture]
	public class SinglePageImagesRollupTests
	{
		[Test]
		public void ItShouldAlwaysReturnFalseForRollupResult()
		{
			var instance = new SinglePageImagesRollup();

			var artifact = new ObjectExportInfo
			{
				Images = new ArrayList()
			};

			var image = new ImageExportInfo();
			artifact.Images.Add(image);

			// ACT
			instance.RollupImages(artifact);

			// ASSERT
			Assert.That(image.SuccessfulRollup, Is.False);
		}

		[Test]
		public void ItShouldHandleEmptyImagesListWithoutException()
		{
			var instance = new SinglePageImagesRollup();

			var artifact = new ObjectExportInfo
			{
				Images = new ArrayList()
			};

			// ACT
			Assert.DoesNotThrow(() => instance.RollupImages(artifact));
		}
	}
}