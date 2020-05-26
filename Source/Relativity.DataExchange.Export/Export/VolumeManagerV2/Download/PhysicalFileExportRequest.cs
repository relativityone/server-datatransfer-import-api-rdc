namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using kCura.WinEDDS.Exporters;

	using Relativity.Transfer;
	using Relativity.Transfer.Http;

	public class PhysicalFileExportRequest : ExportRequest
	{
		private PhysicalFileExportRequest(int artifactId, string sourceLocation, string destinationLocation, string remoteFileGuid)
			: base(artifactId, sourceLocation, destinationLocation)
		{
			this.RemoteFileGuid = remoteFileGuid;
		}

		public static PhysicalFileExportRequest CreateRequestForImage(
			ImageExportInfo image,
			string destinationLocation)
		{
			return new PhysicalFileExportRequest(image.ArtifactID, image.SourceLocation, destinationLocation, image.FileGuid);
		}

		public static PhysicalFileExportRequest CreateRequestForNative(
			ObjectExportInfo artifact,
			string destinationLocation)
		{
			return new PhysicalFileExportRequest(artifact.ArtifactID, artifact.NativeSourceLocation, destinationLocation, artifact.NativeFileGuid);
		}

		public static PhysicalFileExportRequest CreateRequestForPdf(
			ObjectExportInfo artifact,
			string destinationLocation)
		{
			return new PhysicalFileExportRequest(artifact.ArtifactID, artifact.PdfSourceLocation, destinationLocation, artifact.PdfFileGuid);
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