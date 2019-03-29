using Relativity.Import.Export;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public class ImageWrapper : IImage
	{
		private readonly IImageConversionService _imageConverter;

		public ImageWrapper(IImageConversionService imageConverter)
		{
			_imageConverter = imageConverter;
		}


		public void ConvertImagesToMultiPagePdf(string[] inputFiles, string outputFile)
		{
			_imageConverter.ConvertImagesToMultiPagePdf(inputFiles, outputFile);
		}

		public void ConvertTIFFsToMultiPage(string[] inputFiles, string outputFile)
		{
			_imageConverter.ConvertTiffsToMultiPageTiff(inputFiles, outputFile);
		}
	}
}