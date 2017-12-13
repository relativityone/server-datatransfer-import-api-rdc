using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LineImageField
	{
		private readonly ILoadFileCellFormatter _loadFileCellFormatter;
		private readonly ILog _logger;

		public LineImageField(ILoadFileCellFormatter loadFileCellFormatter, ILog logger)
		{
			_loadFileCellFormatter = loadFileCellFormatter;
			_logger = logger;
		}

		public void AddImageField(DeferredEntry loadFileEntry, ObjectExportInfo artifact)
		{
			string imagesCell = _loadFileCellFormatter.CreateImageCell(artifact);
			if (!string.IsNullOrEmpty(imagesCell))
			{
				_logger.LogVerbose("Adding entry for image cell.");
				loadFileEntry.AddStringEntry(imagesCell);
			}
		}
	}
}