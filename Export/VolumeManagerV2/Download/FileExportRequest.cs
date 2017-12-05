using kCura.WinEDDS.Exporters;
using Relativity.Transfer;
using Relativity.Transfer.Http;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FileExportRequest : ExportRequest
	{
		/// <summary>
		///     For Web mode
		/// </summary>
		public string RemoteFileGuid { get; protected set; }

		public string SourceLocation { get; protected set; }

		public FileExportRequest(ImageExportInfo image, string destinationLocation) : base(image.ArtifactID, destinationLocation)
		{
			SourceLocation = image.SourceLocation;
			RemoteFileGuid = image.FileGuid;
		}

		public FileExportRequest(ObjectExportInfo artifact, string destinationLocation) : base(artifact.ArtifactID, destinationLocation)
		{
			SourceLocation = artifact.NativeSourceLocation;
			RemoteFileGuid = artifact.NativeFileGuid;
		}

		public override TransferPath CreateTransferPath(int order)
		{
			var httpTransferPathData = new HttpTransferPathData
			{
				ArtifactId = ArtifactId,
				ExportType = ExportType.NativeFile
			};

			var fileInfo = new System.IO.FileInfo(DestinationLocation);
			var transferPath = new TransferPath
			{
				SourcePath = SourceLocation,
				TargetPath = fileInfo.Directory.FullName,
				TargetFileName = fileInfo.Name,
				Order = order
			};

			transferPath.AddData(HttpTransferPathData.HttpTransferPathDataKey, httpTransferPathData);
			return transferPath;
		}
	}
}