namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Globalization;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Resources;
	using Relativity.Transfer;
	using Relativity.Transfer.Http;

	public class FieldFileExportRequest : ExportRequest
	{
		/// <summary>
		///     For Web mode
		/// </summary>
		public int FileId { get; }

		/// <summary>
		///     For Web mode
		/// </summary>
		public int FileFieldArtifactId { get; }

		public FieldFileExportRequest(ObjectExportInfo artifact, int fileFieldArtifactId, string destinationLocation)
			: base(artifact.ArtifactID, artifact.NativeSourceLocation, destinationLocation)
		{
			FileId = artifact.FileID;
			FileFieldArtifactId = fileFieldArtifactId;
		}

		protected override TransferPath CreateTransferPath()
		{
			var httpTransferPathData = new HttpTransferPathData
			{
				ArtifactId = ArtifactId,
				FileId = FileId,
				FileFieldArtifactId = FileFieldArtifactId,
				ExportType = ExportType.FileFieldArtifact
			};

			if (string.IsNullOrWhiteSpace(this.DestinationLocation))
			{
				string errorMessage = string.Format(
					CultureInfo.CurrentCulture,
					ExportStrings.FieldFileExportRequestDestinationLocationExceptionMessage,
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