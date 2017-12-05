using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class LoadFileWriterRetryable : WriterRetryable, ILoadFileWriter
	{
		private readonly LoadFileWriter _loadFileWriter;

		public LoadFileWriterRetryable(WritersRetryPolicy writersRetryPolicy, StreamFactory streamFactory, ILog logger, IStatus status, LoadFileDestinationPath destinationPath,
			LoadFileWriter loadFileWriter) : base(writersRetryPolicy, streamFactory, logger, status, destinationPath)
		{
			_loadFileWriter = loadFileWriter;
		}

		public void Write(IDictionary<int, ILoadFileEntry> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			Execute((enumerator, streamWriter) =>
			{
				_loadFileWriter.Write(streamWriter, linesToWrite, enumerator);
			}, artifacts, cancellationToken);
		}
	}
}