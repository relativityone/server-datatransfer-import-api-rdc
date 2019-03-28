using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LinePrefix
	{
		private readonly ILoadFileCellFormatter _loadFileCellFormatter;
		private readonly ILog _logger;

		public LinePrefix(ILoadFileCellFormatter loadFileCellFormatter, ILog logger)
		{
			_loadFileCellFormatter = loadFileCellFormatter;
			_logger = logger;
		}

		public void AddPrefix(DeferredEntry loadFileEntry)
		{
			string rowPrefix = _loadFileCellFormatter.RowPrefix;

			if (!string.IsNullOrEmpty(rowPrefix))
			{
				_logger.LogVerbose("Adding prefix to load file entries.");
				loadFileEntry.AddStringEntry(rowPrefix);
			}
		}
	}
}