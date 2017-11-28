using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.ImagesRollup
{
	[TestFixture]
	public class SinglePageImagesRollupTests
	{
		[Test]
		public void ItShouldReturnFalse()
		{
			var instance = new SinglePageImagesRollup();

			//ACT
			bool imagesRollupResult = instance.RollupImages(null, null, 1, 1);

			//ASSERT
			Assert.That(imagesRollupResult, Is.False);
		}
	}
}