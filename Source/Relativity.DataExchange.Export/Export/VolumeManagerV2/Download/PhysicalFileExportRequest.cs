namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using kCura.WinEDDS.Exporters;

	using Relativity.Transfer;
	using Relativity.Transfer.Http;

	public class PhysicalFileExportRequest : ExportRequest
	{
		public PhysicalFileExportRequest(ImageExportInfo image, string destinationLocation)
			: base(image.ArtifactID, image.SourceLocation, destinationLocation)
		{
			this.RemoteFileGuid = image.FileGuid;
		}

		public PhysicalFileExportRequest(ObjectExportInfo artifact, string destinationLocation)
			: base(artifact.ArtifactID, artifact.NativeSourceLocation, destinationLocation)
		{
			this.RemoteFileGuid = artifact.NativeFileGuid;
		}

		/// <summary>
		///     For Web mode
		/// </summary>
		public string RemoteFileGuid { get; }

		protected override TransferPath CreateTransferPath()
		{
			HttpTransferPathData httpTransferPathData = new HttpTransferPathData
				                                            {
					                                            ArtifactId = this.ArtifactId,
					                                            ExportType = ExportType.NativeFile,
					                                            RemoteGuid = this.RemoteFileGuid
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