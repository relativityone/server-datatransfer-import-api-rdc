using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class TextExportRequest : ExportRequest
	{
		/// <summary>
		///     For Web mode
		/// </summary>
		public bool FullText { get; private set; }

		/// <summary>
		///     For Web mode
		/// </summary>
		public int FieldArtifactId { get; private set; }

		private TextExportRequest(ObjectExportInfo artifact, string destinationLocation) : base(artifact, destinationLocation)
		{
			SourceLocation = string.Empty;
			RemoteFileGuid = string.Empty;
		}

		public static TextExportRequest CreateRequestForFullText(ObjectExportInfo artifact, int fieldArtifactId, string destinationLocation)
		{
			var request = new TextExportRequest(artifact, destinationLocation)
			{
				FullText = true,
				FieldArtifactId = fieldArtifactId
			};
			return request;
		}

		public static TextExportRequest CreateRequestForLongText(ObjectExportInfo artifact, int fieldArtifactId, string destinationLocation)
		{
			var request = new TextExportRequest(artifact, destinationLocation)
			{
				FullText = false,
				FieldArtifactId = fieldArtifactId
			};
			return request;
		}
	}
}