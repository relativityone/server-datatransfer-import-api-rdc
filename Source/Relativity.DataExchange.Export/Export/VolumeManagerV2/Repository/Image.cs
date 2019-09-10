namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;

	public class Image
	{
		public ImageExportInfo Artifact { get; }

		public ExportRequest ExportRequest { get; set; }

		public bool TransferCompleted { get; set; }

		public Image(ImageExportInfo artifact)
		{
			Artifact = artifact;
		}
	}
}