﻿namespace Relativity.Export.VolumeManagerV2.ImagesRollup
{
	using Relativity.Import.Export;
	using Relativity.Import.Export.Media;

	public class ImageWrapper : IImage
	{
		private readonly IImageConverter _imageConverter;

		public ImageWrapper(IImageConverter imageConverter)
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