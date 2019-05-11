﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives
{
	using System.Collections.Generic;
	using System.Threading;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Relativity.Logging;

	public class LoadFileMetadataBuilder
	{
		private readonly LoadFileHeader _loadFileHeader;
		private readonly ILoadFileLine _loadFileLine;
		private readonly ILog _logger;

		public LoadFileMetadataBuilder(LoadFileHeader loadFileHeader, ILoadFileLine loadFileLine, ILog logger)
		{
			_loadFileHeader = loadFileHeader;
			_loadFileLine = loadFileLine;
			_logger = logger;
		}

		public IDictionary<int, ILoadFileEntry> AddLines(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Creating metadata for load file for current batch.");

			IDictionary<int, ILoadFileEntry> loadFileEntries = new Dictionary<int, ILoadFileEntry>();

			_loadFileHeader.AddHeader(loadFileEntries);

			foreach (var artifact in artifacts)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return new Dictionary<int, ILoadFileEntry>();
				}

				_logger.LogVerbose("Adding line for artifact {artifactId}.", artifact.ArtifactID);
				ILoadFileEntry line = _loadFileLine.CreateLine(artifact);
				loadFileEntries.Add(artifact.ArtifactID, line);
			}

			_logger.LogVerbose("Metadata for load file for current batch created.");
			return loadFileEntries;
		}
	}
}