using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class MultiPageNotOpticonImageLoadFile : ImageLoadFile
	{
		public MultiPageNotOpticonImageLoadFile(ExportFile exportSettings, IFilePathProvider filePathProvider, ILoadFileEntry loadFileEntry) : base(exportSettings, filePathProvider,
			loadFileEntry)
		{
		}

		protected override List<ImageExportInfo> GetImagesToProcess(ObjectExportInfo artifact)
		{
			return artifact.Images.Cast<ImageExportInfo>().ToList();
		}

		protected override int GetBaseImageIndex(int i)
		{
			return 0;
		}
	}
}