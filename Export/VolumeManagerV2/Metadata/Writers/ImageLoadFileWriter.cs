using System.Collections.Generic;
using System.IO;
using System.Linq;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class ImageLoadFileWriter
	{
		private readonly ILog _logger;

		public ImageLoadFileWriter(ILog logger)
		{
			_logger = logger;
		}

		public void Write(StreamWriter streamWriter, IList<KeyValuePair<string, string>> linesToWrite, IEnumerator<ObjectExportInfo> artifacts)
		{
			if (linesToWrite == null || linesToWrite.Count == 0)
			{
				_logger.LogVerbose("No lines to write to image load file - skipping.");
				return;
			}

			WriteArtifacts(streamWriter, linesToWrite, artifacts);
		}

		private void WriteArtifacts(StreamWriter streamWriter, IList<KeyValuePair<string, string>> linesToWrite, IEnumerator<ObjectExportInfo> artifacts)
		{
			//TODO this "sorting" was introduced after changing ConcurrentDictionary to ConcurrentBag - is it needed?
			while (artifacts.MoveNext())
			{
				_logger.LogVerbose("Writing entries to image load file for artifact {artifactId}.", artifacts.Current.ArtifactID);

				IEnumerable<ImageExportInfo> imagesList = artifacts.Current.Images.Cast<ImageExportInfo>();
				IEnumerable<string> bates = imagesList.Select(x => x.BatesNumber).Distinct();

				foreach (var bate in bates)
				{
					_logger.LogVerbose("Writing entry to image load file for image {bateNumber}.", bate);
					string key = bate;

					foreach (var line in linesToWrite.Where(x => x.Key == $"FT{key}").OrderBy(x => x.Key).ThenBy(x => x.Value))
					{
						_logger.LogVerbose("Writing Full text entry to image load file for image {bateNumber}.", bate);
						streamWriter.Write(line.Value);
					}

					foreach (var line in linesToWrite.Where(x => x.Key == key).OrderBy(x => x.Key).ThenBy(x => x.Value))
					{
						_logger.LogVerbose("Writing metadata entry to image load file for image {bateNumber}.", bate);
						streamWriter.Write(line.Value);
					}
				}
			}
		}
	}
}