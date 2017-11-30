using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class ExportRequest
	{
		/// <summary>
		///     Needed for Web mode
		/// </summary>
		public int ArtifactId { get; }

		/// <summary>
		///     Needed for Web mode
		/// </summary>
		public string RemoteFileGuid { get; protected set; }

		public string SourceLocation { get; protected set; }
		public string DestinationLocation { get; }

		public ExportRequest(ImageExportInfo image, string destinationLocation)
		{
			SourceLocation = image.SourceLocation;
			ArtifactId = image.ArtifactID;
			RemoteFileGuid = image.FileGuid;
			DestinationLocation = destinationLocation;
		}

		public ExportRequest(ObjectExportInfo artifact, string destinationLocation)
		{
			SourceLocation = artifact.NativeSourceLocation;
			ArtifactId = artifact.ArtifactID;
			RemoteFileGuid = artifact.NativeFileGuid;
			DestinationLocation = destinationLocation;
		}
	}
}