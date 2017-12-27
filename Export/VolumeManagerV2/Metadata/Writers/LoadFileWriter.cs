using System.Collections.Generic;
using System.IO;
using System.Threading;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
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
			const int headerArtifactID = -1;

			ILoadFileEntry loadFileEntry;
			if (linesToWrite.TryGetValue(headerArtifactID, out loadFileEntry))
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

				ILoadFileEntry loadFileEntry;
				if (linesToWrite.TryGetValue(artifacts.Current.ArtifactID, out loadFileEntry))
				{
					_logger.LogVerbose("Writing entry to load file for artifact {artifactId}.", artifacts.Current.ArtifactID);
					loadFileEntry?.Write(ref streamWriter);
				}
			}
		}
	}
}