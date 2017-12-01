using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Exceptions;
using kCura.WinEDDS.Exporters;
using Polly;
using Polly.Retry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class ImageLoadFileWriter : MetadataFileWriter
	{
		public ImageLoadFileWriter(ILog logger, StatisticsWrapper statistics, IFileHelper fileHelper, RetryPolicy retryPolicy, ImageLoadFileDestinationPath destinationPath,
			StreamFactory streamFactory) : base(logger, statistics, fileHelper, retryPolicy, destinationPath, streamFactory)
		{
		}

		public void Write(IList<KeyValuePair<string, string>> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			if (linesToWrite == null || linesToWrite.Count == 0)
			{
				Logger.LogVerbose("No lines to write to image load file - skipping.");
				return;
			}

			Logger.LogVerbose("Writing to image load file with retry policy.");
			ExecuteWithRetry((context, token) =>
			{
				Write(linesToWrite, artifacts, context);
			}, cancellationToken);
		}

		private void Write(IList<KeyValuePair<string, string>> linesToWrite, ObjectExportInfo[] artifacts, Context context)
		{
			ReinitializeStream();

			WriteArtifacts(linesToWrite, artifacts, context);
			
			SaveStreamPositionAndUpdateStatistics();
		}

		private void WriteArtifacts(IList<KeyValuePair<string, string>> linesToWrite, ObjectExportInfo[] artifacts, Context context)
		{
			//TODO this "sorting" was introduced after changing ConcurrentDictionary to ConcurrentBag - is it needed?
			foreach (var artifact in artifacts)
			{
				//TODO I don't like this :(
				context[WritersRetryPolicy.CONTEXT_LAST_ARTIFACT_ID_KEY] = artifact.ArtifactID;

				IEnumerable<ImageExportInfo> imagesList = artifact.Images.Cast<ImageExportInfo>();
				IEnumerable<string> bates = imagesList.Select(x => x.BatesNumber).Distinct();

				foreach (var bate in bates)
				{
					string key = bate;

					foreach (var line in linesToWrite.Where(x => x.Key == $"FT{key}").OrderBy(x => x.Key).ThenBy(x => x.Value))
					{
						_fileWriter.Write(line.Value);
					}

					foreach (var line in linesToWrite.Where(x => x.Key == key).OrderBy(x => x.Key).ThenBy(x => x.Value))
					{
						_fileWriter.Write(line.Value);
					}
				}
			}
		}


		protected override FileWriteException.DestinationFile GetLoadFileContext()
		{
			return FileWriteException.DestinationFile.Image;
		}
	}
}