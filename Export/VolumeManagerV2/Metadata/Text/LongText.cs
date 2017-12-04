using System.IO;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public abstract class LongText
	{
		protected string LongTextValue { get; private set; }

		public TextExportRequest ExportRequest { get; private set; }
		public string Location { get; private set; }
		public bool RequireDeletion { get; private set; }
		public int ArtifactId { get; private set; }
		public int FieldArtifactId { get; private set; }

		public abstract TextReader GetLongText();

		public static LongText CreateFromMissingFile(int artifactId, int fieldArtifactId, TextExportRequest exportRequest)
		{
			return new LongTextInFile
			{
				ArtifactId = artifactId,
				FieldArtifactId = fieldArtifactId,
				Location = exportRequest.DestinationLocation,
				ExportRequest = exportRequest,
				RequireDeletion = false
			};
		}

		public static LongText CreateFromMissingValue(int artifactId, int fieldArtifactId, TextExportRequest exportRequest)
		{
			return new LongTextInFile
			{
				ArtifactId = artifactId,
				FieldArtifactId = fieldArtifactId,
				Location = exportRequest.DestinationLocation,
				ExportRequest = exportRequest,
				//It will be stored in temporary file
				RequireDeletion = true
			};
		}

		public static LongText CreateFromExistingFile(int artifactId, int fieldArtifactId, string location)
		{
			return new LongTextInFile
			{
				ArtifactId = artifactId,
				FieldArtifactId = fieldArtifactId,
				Location = location,
				RequireDeletion = false
			};
		}

		public static LongText CreateFromExistingValue(int artifactId, int fieldArtifactId, string text)
		{
			return new LongTextInMemory
			{
				ArtifactId = artifactId,
				FieldArtifactId = fieldArtifactId,
				LongTextValue = text,
				RequireDeletion = false
			};
		}
	}
}