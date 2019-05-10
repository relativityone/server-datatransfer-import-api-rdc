namespace Relativity.Export.VolumeManagerV2.Metadata.Writers
{
	using System.Collections.Generic;
	using System.Threading;

	using kCura.WinEDDS;

	using Relativity.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.Export.VolumeManagerV2.Statistics;
	using Relativity.Logging;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	public class LoadFileWriterRetryable : WriterRetryable, ILoadFileWriter
	{
		private readonly LoadFileWriter _loadFileWriter;

		public LoadFileWriterRetryable(WritersRetryPolicy writersRetryPolicy, StreamFactory streamFactory, ILog logger, IStatus status, LoadFileDestinationPath destinationPath,
			LoadFileWriter loadFileWriter, IMetadataProcessingStatistics metadataProcessingStatistics) : base(writersRetryPolicy, streamFactory, logger, status, destinationPath,
			metadataProcessingStatistics)
		{
			_loadFileWriter = loadFileWriter;
		}

		public void Write(IDictionary<int, ILoadFileEntry> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			Execute((enumerator, streamWriter) => { _loadFileWriter.Write(streamWriter, linesToWrite, enumerator, cancellationToken); }, artifacts, cancellationToken);
		}
	}
}