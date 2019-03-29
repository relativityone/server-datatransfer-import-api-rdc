using System.Collections.Generic;
using System.Linq;
using Relativity.Import.Export.Io;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
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