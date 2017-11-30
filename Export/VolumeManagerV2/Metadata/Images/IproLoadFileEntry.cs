using System.Collections.Concurrent;
using System.Collections.Generic;
using kCura.WinEDDS.Exporters.LineFactory;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class IproLoadFileEntry : ILoadFileEntry
	{
		private readonly ExportFile _exportSettings;

		public IproLoadFileEntry(ExportFile exportSettings)
		{
			_exportSettings = exportSettings;
		}

		public KeyValuePair<string, string> Create(string batesNumber, string filePath, int pageNumber, long pageOffset, int numberOfImages)
		{
			//TODO
			var lineFactory = new SimpleIproImageLineFactory(batesNumber, pageNumber, filePath, "TODO", _exportSettings.TypeOfImage.Value);
			var linesToWriteOpt = new ConcurrentBag<KeyValuePair<string, string>>();
			lineFactory.WriteLine(null, linesToWriteOpt);
			return linesToWriteOpt.ToArray()[0];
		}
	}
}