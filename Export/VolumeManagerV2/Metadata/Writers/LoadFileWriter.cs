using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Exceptions;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Polly;
using Polly.Retry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class LoadFileWriter : MetadataFileWriter
	{
		public LoadFileWriter(ILog logger, StatisticsWrapper statistics, IFileHelper fileHelper, RetryPolicy retryPolicy, IDestinationPath destinationPath, StreamFactory streamFactory) :
			base(logger, statistics, fileHelper, retryPolicy, destinationPath, streamFactory)
		{
		}

		public void Write(IDictionary<int, ILoadFileEntry> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			if (linesToWrite == null || linesToWrite.Count == 0)
			{
				Logger.LogVerbose("No lines to write to load file - skipping.");
				return;
			}

			Logger.LogVerbose("Writing to load file with retry policy.");

			ExecuteWithRetry((context, token) =>
			{
				Write(linesToWrite, artifacts, context);
			}, cancellationToken);
		}

		private void Write(IDictionary<int, ILoadFileEntry> linesToWrite, ObjectExportInfo[] artifacts, Context context)
		{
			ReinitializeStream();

			WriteHeaderIfNeeded(linesToWrite);

			WriteArtifacts(linesToWrite, artifacts, context);

			SaveStreamPositionAndUpdateStatistics();
		}

		public void WriteHeaderIfNeeded(IDictionary<int, ILoadFileEntry> linesToWrite)
		{
			const int headerArtifactID = -1;

			ILoadFileEntry loadFileEntry;
			if (linesToWrite.TryGetValue(headerArtifactID, out loadFileEntry))
			{
				loadFileEntry?.Write(ref FileWriter);
			}
		}

		private void WriteArtifacts(IDictionary<int, ILoadFileEntry> linesToWrite, ObjectExportInfo[] artifacts, Context context)
		{
			foreach (var artifact in artifacts)
			{
				//TODO I don't like this :(
				context[WritersRetryPolicy.CONTEXT_LAST_ARTIFACT_ID_KEY] = artifact.ArtifactID;

				ILoadFileEntry loadFileEntry;
				if (linesToWrite.TryGetValue(artifact.ArtifactID, out loadFileEntry))
				{
					loadFileEntry?.Write(ref FileWriter);
				}
			}
		}

		protected override FileWriteException.DestinationFile GetLoadFileContext()
		{
			return FileWriteException.DestinationFile.Load;
		}
	}
}