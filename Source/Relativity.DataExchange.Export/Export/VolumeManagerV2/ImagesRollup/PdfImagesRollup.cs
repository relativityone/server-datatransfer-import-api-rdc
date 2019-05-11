namespace Relativity.DataExchange.Export.VolumeManagerV2.ImagesRollup
{
	using System.Collections.Generic;
	using System.Linq;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Io;
	using Relativity.Logging;

	public class PdfImagesRollup : MultiPageImagesRollup
	{
		public PdfImagesRollup(ExportFile exportSettings, IFile fileWrapper, IStatus status, ILog logger, IImage imageConverter) : base(exportSettings, fileWrapper, status, logger,
			imageConverter)
		{
		}

		protected override void ConvertImage(IList<string> imageList, string tempLocation)
		{
			ImageConverter.ConvertImagesToMultiPagePdf(imageList.ToArray(), tempLocation);
		}

		protected override string GetExtension()
		{
			return ".pdf";
		}
	}
}