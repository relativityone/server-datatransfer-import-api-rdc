using System.Collections.Generic;
using System.Linq;
using kCura.Utility;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public class PdfImagesRollup : MultiPageImagesRollup
	{
		public PdfImagesRollup(ExportFile exportSettings, IFileHelper fileHelper, IStatus status, ILog logger, Image imageConverter) : base(exportSettings, fileHelper, status, logger,
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