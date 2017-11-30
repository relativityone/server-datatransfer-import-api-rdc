using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class SinglePageImageLoadFile : ImageLoadFile
	{
		public SinglePageImageLoadFile(ExportFile exportSettings, IFilePathProvider filePathProvider, ILoadFileEntry loadFileEntry, IFullTextLoadFileEntry fullTextLoadFileEntry) : base(
			exportSettings, filePathProvider, loadFileEntry, fullTextLoadFileEntry)
		{
		}

		protected override List<ImageExportInfo> GetImagesToProcess(ObjectExportInfo artifact)
		{
			return artifact.Images.Cast<ImageExportInfo>().ToList();
		}

		protected override int GetBaseImageIndex(int i)
		{
			return i;
		}
	}
}