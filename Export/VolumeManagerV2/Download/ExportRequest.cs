using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public abstract class ExportRequest
	{
		private static int _exportRequestUniqueOrder;

		/// <summary>
		///     For Web mode
		/// </summary>
		public int ArtifactId { get; }

		public string DestinationLocation { get; }

		public int Order { get; }

		protected ExportRequest(int artifactId, string destinationLocation)
		{
			ArtifactId = artifactId;
			DestinationLocation = destinationLocation;
			Order = _exportRequestUniqueOrder++;
		}

		public abstract TransferPath CreateTransferPath();
	}
}