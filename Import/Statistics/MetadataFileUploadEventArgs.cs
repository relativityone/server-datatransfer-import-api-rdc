namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class MetadataFileUploadEventArgs
	{
		public MetadataFileUploadEventArgs(int metadataFileChunks, int currentChunk)
		{
			MetadataFileChunks = metadataFileChunks;
			CurrentChunk = currentChunk;
		}

		public int MetadataFileChunks { get; }
		public int CurrentChunk { get; }
	}
}