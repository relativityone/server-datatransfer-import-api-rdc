using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class MultiPageOpticonImageLoadFile : ImageLoadFile
	{
		public MultiPageOpticonImageLoadFile(ExportFile exportSettings, IFilePathProvider filePathProvider, ILoadFileEntry loadFileEntry) : base(exportSettings, filePathProvider,
			loadFileEntry)
		{
		}

		protected override List<ImageExportInfo> GetImagesToProcess(ObjectExportInfo artifact)
		{
			if (artifact.Images == null || artifact.Images.Count == 0)
			{
				return new List<ImageExportInfo>();
			}
			//Opticon file should have only one entry for all pages
			return new List<ImageExportInfo> {(ImageExportInfo) artifact.Images[0]};
		}

		protected override int GetBaseImageIndex(int i)
		{
			return i;
		}
	}
}