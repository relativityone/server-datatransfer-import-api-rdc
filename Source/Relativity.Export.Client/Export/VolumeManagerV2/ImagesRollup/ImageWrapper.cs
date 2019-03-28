using kCura.Utility;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public class ImageWrapper : IImage
	{
		private readonly Image _imageConverter;

		public ImageWrapper(Image imageConverter)
		{
			_imageConverter = imageConverter;
		}


		public void ConvertImagesToMultiPagePdf(string[] inputFiles, string outputFile)
		{
			_imageConverter.ConvertImagesToMultiPagePdf(inputFiles, outputFile);
		}

		public void ConvertTIFFsToMultiPage(string[] inputFiles, string outputFile)
		{
			_imageConverter.ConvertTIFFsToMultiPage(inputFiles, outputFile);
		}
	}
}