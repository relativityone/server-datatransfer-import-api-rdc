using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FileExportRequest : ExportRequest
	{
		/// <summary>
		///     For Web mode
		/// </summary>
		public string RemoteFileGuid { get; protected set; }

		public string SourceLocation { get; protected set; }

		public FileExportRequest(ImageExportInfo image, string destinationLocation) : base(image.ArtifactID, destinationLocation)
		{
			SourceLocation = image.SourceLocation;
			RemoteFileGuid = image.FileGuid;
		}

		public FileExportRequest(ObjectExportInfo artifact, string destinationLocation) : base(artifact.ArtifactID, destinationLocation)
		{
			SourceLocation = artifact.NativeSourceLocation;
			RemoteFileGuid = artifact.NativeFileGuid;
		}
	}
}