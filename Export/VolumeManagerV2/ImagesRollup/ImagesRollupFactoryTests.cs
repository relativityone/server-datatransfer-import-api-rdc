using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Exporters;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.ImagesRollup
{
	[TestFixture]
	public class ImagesRollupFactoryTests
	{
		[Test]
		[TestCase(false, false)]
		[TestCase(false, true)]
		[TestCase(true, false)]
		public void ItShouldReturnSinglePageRollupWhenNotExportingImages(bool exportImages, bool copyFiles)
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				ExportImages = exportImages,
				VolumeInfo = new VolumeInfo
				{
					CopyImageFilesFromRepository = copyFiles
				},
				TypeOfImage = ExportFile.ImageType.MultiPageTiff
			};

			var instance = new ImagesRollupFactory(null, null, null);

			//ACT
			IImagesRollup imagesRollup = instance.Create(exportSettings);

			//ASSERT
			Assert.That(imagesRollup, Is.InstanceOf<SinglePageImagesRollup>());
		}
	}
}