using kCura.WinEDDS.Exporters;

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
	}
}