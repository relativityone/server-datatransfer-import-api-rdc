namespace Relativity.Export.VolumeManagerV2.Repository
{
	using kCura.WinEDDS.Exporters;

	using Relativity.Export.VolumeManagerV2.Download;

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