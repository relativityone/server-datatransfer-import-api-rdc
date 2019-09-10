namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using kCura.WinEDDS.Exporters;

	using Relativity.Transfer;
	using Relativity.Transfer.Http;

	public class FieldFileExportRequest : ExportRequest
	{
		public FieldFileExportRequest(ObjectExportInfo artifact, int fileFieldArtifactId, string destinationLocation)
			: base(artifact.ArtifactID, artifact.NativeSourceLocation, destinationLocation)
		{
			this.FileId = artifact.FileID;
			this.FileFieldArtifactId = fileFieldArtifactId;
		}

		/// <summary>
		///     For Web mode
		/// </summary>
		public int FileId { get; }

		/// <summary>
		///     For Web mode
		/// </summary>
		public int FileFieldArtifactId { get; }

		protected override TransferPath CreateTransferPath()
		{
			HttpTransferPathData httpTransferPathData = new HttpTransferPathData
				                                            {
					                                            ArtifactId = this.ArtifactId,
					                                            FileId = this.FileId,
					                                            FileFieldArtifactId = this.FileFieldArtifactId,
					                                            ExportType = ExportType.FileFieldArtifact
				                                            };
			TransferPath transferPath = CreateTransferPath(
				this.ArtifactId,
				this.Order,
				this.SourceLocation,
				this.DestinationLocation,
				httpTransferPathData);
			return transferPath;
		}
	}
}