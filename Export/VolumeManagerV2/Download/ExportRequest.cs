using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public abstract class ExportRequest
	{
		/// <summary>
		///     For Web mode
		/// </summary>
		public int ArtifactId { get; }

		public string DestinationLocation { get; }

		public string UniqueId { get; set; }

		protected ExportRequest(int artifactId, string destinationLocation)
		{
			ArtifactId = artifactId;
			DestinationLocation = destinationLocation;
		}

		public abstract TransferPath CreateTransferPath();
	}
}