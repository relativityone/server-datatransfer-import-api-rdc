using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public abstract class ExportRequest
	{
		/// <summary>
		///     For Web mode
		/// </summary>
		public int ArtifactId { get; }

		public string DestinationLocation { get; }

		protected ExportRequest(int artifactId, string destinationLocation)
		{
			ArtifactId = artifactId;
			DestinationLocation = destinationLocation;
		}
	}
}