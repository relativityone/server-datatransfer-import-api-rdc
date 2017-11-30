using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class TextExportRequest : ExportRequest
	{
		public bool FullText { get; private set; }
		public int FieldArtifactId { get; private set; }

		private TextExportRequest(ObjectExportInfo artifact, string destinationLocation) : base(artifact, destinationLocation)
		{
			SourceLocation = string.Empty;
			RemoteFileGuid = string.Empty;
		}

		public static TextExportRequest CreateRequestForFullText(ObjectExportInfo artifact, ViewFieldInfo field, string destinationLocation)
		{
			var request = new TextExportRequest(artifact, destinationLocation)
			{
				FullText = true,
				FieldArtifactId = field.FieldArtifactId
			};
			return request;
		}

		public static TextExportRequest CreateRequestForLongText(ObjectExportInfo artifact, ViewFieldInfo field, string destinationLocation)
		{
			var request = new TextExportRequest(artifact, destinationLocation)
			{
				FullText = false,
				FieldArtifactId = field.FieldArtifactId
			};
			return request;
		}
	}
}