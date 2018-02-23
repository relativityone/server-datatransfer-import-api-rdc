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

		public int Order { get; set; }

		protected ExportRequest(int artifactId, string destinationLocation)
		{
			ArtifactId = artifactId;
			DestinationLocation = destinationLocation;
		}

		public TransferPath CreateTransferPath(int order)
		{
			Order = order;
			return CreateTransferPath();
		}


		protected abstract TransferPath CreateTransferPath();
	}
}