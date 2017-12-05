using System.Collections.Concurrent;
using System.Collections.Generic;
using kCura.WinEDDS.Exporters.LineFactory;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public class IproLoadFileEntry : IImageLoadFileEntry
	{
		private readonly ExportFile _exportSettings;
		private readonly ILog _logger;

		public IproLoadFileEntry(ExportFile exportSettings, ILog logger)
		{
			_exportSettings = exportSettings;
			_logger = logger;
		}

		public KeyValuePair<string, string> Create(string batesNumber, string filePath, string volume, int pageNumber, long pageOffset, int numberOfImages)
		{
			_logger.LogVerbose("Creating Ipro load file entry for image {batesNumber} with type {type}.", batesNumber, _exportSettings.TypeOfImage.Value);
			var lineFactory = new SimpleIproImageLineFactory(batesNumber, pageNumber, filePath, volume, _exportSettings.TypeOfImage.Value);
			var linesToWriteOpt = new ConcurrentBag<KeyValuePair<string, string>>();
			lineFactory.WriteLine(null, linesToWriteOpt);
			return linesToWriteOpt.ToArray()[0];
		}
	}
}