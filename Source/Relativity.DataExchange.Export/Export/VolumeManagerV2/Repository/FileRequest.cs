namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;

	public class FileRequest<TInfo>
	{
		public TInfo Artifact { get; }

		public ExportRequest ExportRequest { get; set; }

		public bool TransferCompleted { get; set; }

		public FileRequest(TInfo artifact)
		{
			Artifact = artifact;
		}
	}
}
