using kCura.WinEDDS.Exporters;
using Relativity.Transfer;
using Relativity.Transfer.Http;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class LongTextExportRequest : ExportRequest
	{
		/// <summary>
		///     For Web mode
		/// </summary>
		public bool FullText { get; private set; }

		/// <summary>
		///     For Web mode
		/// </summary>
		public int FieldArtifactId { get; private set; }

		private LongTextExportRequest(ObjectExportInfo artifact, string destinationLocation) : base(artifact.ArtifactID, destinationLocation)
		{
		}

		public static LongTextExportRequest CreateRequestForFullText(ObjectExportInfo artifact, int fieldArtifactId, string destinationLocation)
		{
			var request = new LongTextExportRequest(artifact, destinationLocation)
			{
				FullText = true,
				FieldArtifactId = fieldArtifactId
			};
			return request;
		}

		public static LongTextExportRequest CreateRequestForLongText(ObjectExportInfo artifact, int fieldArtifactId, string destinationLocation)
		{
			var request = new LongTextExportRequest(artifact, destinationLocation)
			{
				FullText = false,
				FieldArtifactId = fieldArtifactId
			};
			return request;
		}

		public override TransferPath CreateTransferPath()
		{
			var httpTransferPathData = new HttpTransferPathData
			{
				ArtifactId = ArtifactId,
				ExportType = FullText ? ExportType.FullText : ExportType.LongTextFieldArtifact,
				LongTextFieldArtifactId = FieldArtifactId
			};

			var fileInfo = new System.IO.FileInfo(DestinationLocation);
			var transferPath = new TransferPath
			{
				SourcePath = "LongText",
				TargetPath = fileInfo.Directory?.FullName,
				TargetFileName = fileInfo.Name
			};

			transferPath.AddData(HttpTransferPathData.HttpTransferPathDataKey, httpTransferPathData);
			return transferPath;
		}
	}
}