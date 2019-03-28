using System.IO;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public abstract class LongText
	{
		protected string LongTextValue { get; private set; }

		public LongTextExportRequest ExportRequest { get; private set; }
		public bool HasBeenDownloaded { get; set; }
		public string Location { get; private set; }
		public bool RequireDeletion { get; private set; }
		public int ArtifactId { get; private set; }
		public int FieldArtifactId { get; private set; }
		public Encoding SourceEncoding { get; set; }
		public Encoding DestinationEncoding { get; private set; }

		public abstract TextReader GetLongText();

		public static LongText CreateFromMissingFile(int artifactId, int fieldArtifactId, LongTextExportRequest exportRequest, Encoding sourceEncoding, Encoding destinationEncoding)
		{
			return new LongTextInFile
			{
				ArtifactId = artifactId,
				FieldArtifactId = fieldArtifactId,
				Location = exportRequest.DestinationLocation,
				ExportRequest = exportRequest,
				RequireDeletion = false,
				SourceEncoding = sourceEncoding,
				DestinationEncoding = destinationEncoding,
				HasBeenDownloaded = false
			};
		}

		public static LongText CreateFromMissingValue(int artifactId, int fieldArtifactId, LongTextExportRequest exportRequest, Encoding encoding)
		{
			return new LongTextInFile
			{
				ArtifactId = artifactId,
				FieldArtifactId = fieldArtifactId,
				Location = exportRequest.DestinationLocation,
				ExportRequest = exportRequest,
				//It will be stored in temporary file
				RequireDeletion = true,
				SourceEncoding = encoding,
				DestinationEncoding = encoding,
				HasBeenDownloaded = false
			};
		}

		public static LongText CreateFromExistingFile(int artifactId, int fieldArtifactId, string location, Encoding encoding)
		{
			return new LongTextInFile
			{
				ArtifactId = artifactId,
				FieldArtifactId = fieldArtifactId,
				Location = location,
				RequireDeletion = false,
				SourceEncoding = encoding,
				DestinationEncoding = encoding,
				HasBeenDownloaded = true
			};
		}

		public static LongText CreateFromExistingValue(int artifactId, int fieldArtifactId, string text)
		{
			return new LongTextInMemory
			{
				ArtifactId = artifactId,
				FieldArtifactId = fieldArtifactId,
				LongTextValue = text,
				RequireDeletion = false,
				SourceEncoding = Encoding.Default,
				DestinationEncoding = Encoding.Default,
				HasBeenDownloaded = true
			};
		}
	}
}