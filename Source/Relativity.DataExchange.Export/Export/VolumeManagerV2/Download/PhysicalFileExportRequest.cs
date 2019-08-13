namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Globalization;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Resources;
	using Relativity.Transfer;
	using Relativity.Transfer.Http;

	public class PhysicalFileExportRequest : ExportRequest
	{
		/// <summary>
		///     For Web mode
		/// </summary>
		public string RemoteFileGuid { get; }

		public PhysicalFileExportRequest(ImageExportInfo image, string destinationLocation)
			: base(image.ArtifactID, image.SourceLocation, destinationLocation)
		{
			RemoteFileGuid = image.FileGuid;
		} 

		public PhysicalFileExportRequest(ObjectExportInfo artifact, string destinationLocation)
			: base(artifact.ArtifactID, artifact.NativeSourceLocation, destinationLocation)
		{
			RemoteFileGuid = artifact.NativeFileGuid;
		}

		protected override TransferPath CreateTransferPath()
		{
			var httpTransferPathData = new HttpTransferPathData
			{
				ArtifactId = ArtifactId,
				ExportType = ExportType.NativeFile,
				RemoteGuid = RemoteFileGuid
			};

			if (string.IsNullOrWhiteSpace(this.DestinationLocation))
			{
				string errorMessage = string.Format(
					CultureInfo.CurrentCulture,
					ExportStrings.NativeExportRequestDestinationLocationExceptionMessage,
					this.ArtifactId);
				throw new ArgumentException(errorMessage, nameof(this.DestinationLocation));
			}

			var fileInfo = new System.IO.FileInfo(DestinationLocation);
			var transferPath = new TransferPath
			{
				Order = Order,
				SourcePath = SourceLocation,
				TargetPath = fileInfo.Directory?.FullName,
				TargetFileName = fileInfo.Name
			};

			transferPath.AddData(HttpTransferPathData.HttpTransferPathDataKey, httpTransferPathData);
			return transferPath;
		}
	}
}