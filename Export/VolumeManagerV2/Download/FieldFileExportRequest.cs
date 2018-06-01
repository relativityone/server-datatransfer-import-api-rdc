using kCura.WinEDDS.Exporters;
using Relativity.Transfer;
using Relativity.Transfer.Http;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FieldFileExportRequest : FileExportRequest
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

		public override TransferPath CreateTransferPath(int order)
		{
			var httpTransferPathData = new HttpTransferPathData
			{
				ArtifactId = ArtifactId,
				FileId = FileId,
				FileFieldArtifactId = FileFieldArtifactId,
				ExportType = ExportType.FileFieldArtifact
			};

			var fileInfo = new System.IO.FileInfo(DestinationLocation);
			var transferPath = new TransferPath
			{
				Order = order,
				SourcePath = SourceLocation,
				TargetPath = fileInfo.Directory?.FullName,
				TargetFileName = fileInfo.Name
			};

			transferPath.AddData(HttpTransferPathData.HttpTransferPathDataKey, httpTransferPathData);
			return transferPath;
		}
	}
}