using System.Collections.Generic;
using System.Linq;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public class MultiPageTiffImagesRollup : MultiPageImagesRollup
	{
		public MultiPageTiffImagesRollup(ExportFile exportSettings, IFileHelper fileHelper, IStatus status, ILog logger, IImage imageConverter) : base(exportSettings, fileHelper, status,
			logger, imageConverter)
		{
		}

		protected override void ConvertImage(IList<string> imageList, string tempLocation)
		{
			ImageConverter.ConvertTIFFsToMultiPage(imageList.ToArray(), tempLocation);
		}

		protected override string GetExtension()
		{
			return ".tif";
		}
	}
}