namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text
{
	using System.IO;
	using System.Text;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;

	public abstract class LongText
	{
		protected string LongTextValue { get; private set; }

		public LongTextExportRequest ExportRequest { get; private set; }
		public bool TransferCompleted { get; set; }
		public string Location { get; private set; }
		public bool RequireDeletion { get; private set; }
		public int ArtifactId { get; private set; }
		public int FieldArtifactId { get; private set; }
		public Encoding SourceEncoding { get; set; }
		public Encoding DestinationEncoding { get; private set; }
		public long Length { get; private set; }

		public abstract TextReader GetLongText();

		public static LongText CreateFromMissingFile(
			int artifactId,
			int fieldArtifactId,
			LongTextExportRequest exportRequest,
			Encoding sourceEncoding,
			Encoding destinationEncoding,
			long length)
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
				TransferCompleted = false,
				Length = length
			};
		}

		public static LongText CreateFromMissingValue(
			int artifactId,
			int fieldArtifactId,
			LongTextExportRequest exportRequest,
			Encoding encoding,
			long length)
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
				TransferCompleted = false,
				Length = length
			};
		}

		public static LongText CreateFromExistingFile(
			int artifactId,
			int fieldArtifactId,
			string location,
			Encoding encoding,
			long length)
		{
			return new LongTextInFile
			{
				ArtifactId = artifactId,
				FieldArtifactId = fieldArtifactId,
				Location = location,
				RequireDeletion = false,
				SourceEncoding = encoding,
				DestinationEncoding = encoding,
				TransferCompleted = true,
				Length = length
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
				TransferCompleted = true,
				Length = text?.Length ?? 0
			};
		}
	}
}