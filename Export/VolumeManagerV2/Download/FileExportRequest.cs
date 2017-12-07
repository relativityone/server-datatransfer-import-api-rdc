namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public abstract class FileExportRequest : ExportRequest
	{
		public string SourceLocation { get; }

		protected FileExportRequest(int artifactId, string sourceLocation, string destinationLocation) : base(artifactId, destinationLocation)
		{
			SourceLocation = sourceLocation;
		}
	}
}