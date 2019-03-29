namespace Relativity.Export.VolumeManagerV2.ImagesRollup
{
	using Relativity.Import.Export;

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