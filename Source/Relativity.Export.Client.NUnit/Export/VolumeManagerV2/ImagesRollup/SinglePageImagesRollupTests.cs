using System.Collections;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Exporters;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.ImagesRollup
{
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

			//ACT
			instance.RollupImages(artifact);

			//ASSERT
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

			//ACT
			Assert.DoesNotThrow(() => instance.RollupImages(artifact));
		}
	}
}