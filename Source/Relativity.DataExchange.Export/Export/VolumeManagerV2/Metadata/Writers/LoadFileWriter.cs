namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Threading;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives;
	using Relativity.Logging;

	public class LoadFileWriter
	{
		private readonly ILog _logger;

		public LoadFileWriter(ILog logger)
		{
			_logger = logger;
		}

		public void Write(StreamWriter streamWriter, IDictionary<int, ILoadFileEntry> linesToWrite, IEnumerator<ObjectExportInfo> artifacts, CancellationToken cancellationToken)
		{
			if (linesToWrite == null || linesToWrite.Count == 0)
			{
				_logger.LogVerbose("No lines to write to load file - skipping.");
				return;
			}

			_logger.LogVerbose("Writing to load file with retry policy.");

			WriteHeaderIfNeeded(streamWriter, linesToWrite);

			WriteArtifacts(streamWriter, linesToWrite, artifacts, cancellationToken);
		}

		private void WriteHeaderIfNeeded(StreamWriter streamWriter, IDictionary<int, ILoadFileEntry> linesToWrite)
		{
			if (linesToWrite.TryGetValue(LoadFileHeader.HEADER_KEY, out var loadFileEntry))
			{
				_logger.LogVerbose("Writing header to load file.");
				loadFileEntry?.Write(ref streamWriter);
			}
		}

		private void WriteArtifacts(StreamWriter streamWriter, IDictionary<int, ILoadFileEntry> linesToWrite, IEnumerator<ObjectExportInfo> artifacts,
			CancellationToken cancellationToken)
		{
			while (artifacts.MoveNext())
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				if (linesToWrite.TryGetValue(artifacts.Current.ArtifactID, out var loadFileEntry))
				{
					_logger.LogVerbose("Writing entry to load file for artifact {artifactId}.", artifacts.Current.ArtifactID);
					loadFileEntry?.Write(ref streamWriter);
				}
			}
		}
	}
}