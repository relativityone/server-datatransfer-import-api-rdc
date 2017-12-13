using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class Image
	{
		public ImageExportInfo Artifact { get; }

		public ExportRequest ExportRequest { get; set; }

		public bool HasBeenDownloaded { get; set; }

		public Image(ImageExportInfo artifact)
		{
			Artifact = artifact;
		}
	}
}