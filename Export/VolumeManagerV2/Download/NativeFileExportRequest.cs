using kCura.WinEDDS.Exporters;
using Relativity.Transfer;
using Relativity.Transfer.Http;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class NativeFileExportRequest : FileExportRequest
	{
		/// <summary>
		///     For Web mode
		/// </summary>
		public string RemoteFileGuid { get; }

		public NativeFileExportRequest(ImageExportInfo image, string destinationLocation)
			: base(image.ArtifactID, image.SourceLocation, destinationLocation)
		{
			RemoteFileGuid = image.FileGuid;
		}

		public NativeFileExportRequest(ObjectExportInfo artifact, string destinationLocation)
			: base(artifact.ArtifactID, artifact.NativeSourceLocation, destinationLocation)
		{
			RemoteFileGuid = artifact.NativeFileGuid;
		}

		public override TransferPath CreateTransferPath()
		{
			var httpTransferPathData = new HttpTransferPathData
			{
				ArtifactId = ArtifactId,
				ExportType = ExportType.NativeFile,
				RemoteGuid = RemoteFileGuid
			};

			var fileInfo = new System.IO.FileInfo(DestinationLocation);
			var transferPath = new TransferPath
			{
				SourcePath = SourceLocation,
				TargetPath = fileInfo.Directory?.FullName,
				TargetFileName = fileInfo.Name
			};

			transferPath.AddData(HttpTransferPathData.HttpTransferPathDataKey, httpTransferPathData);
			return transferPath;
		}
	}
}