namespace Relativity.Export.VolumeManagerV2.Repository
{
	using kCura.WinEDDS.Exporters;

	using Relativity.Export.VolumeManagerV2.Download;

	public class Native
	{
		public ObjectExportInfo Artifact { get; }

		public ExportRequest ExportRequest { get; set; }

		public bool HasBeenDownloaded { get; set; }

		public Native(ObjectExportInfo artifact)
		{
			Artifact = artifact;
		}
	}
}