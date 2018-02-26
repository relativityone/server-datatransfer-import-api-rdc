using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public abstract class ExportRequest
	{
		public string SourceLocation { get; }

		/// <summary>
		///     For Web mode
		/// </summary>
		public int ArtifactId { get; }

		public string DestinationLocation { get; }

		public string FileName { get; set; }

		public int Order { get; set; }

		protected ExportRequest(int artifactId, string sourceLocation, string destinationLocation)
		{
			ArtifactId = artifactId;
			SourceLocation = sourceLocation;
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