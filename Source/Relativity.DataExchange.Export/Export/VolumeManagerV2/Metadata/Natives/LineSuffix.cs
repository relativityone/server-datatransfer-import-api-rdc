namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives
{
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Relativity.Logging;

	public class LineSuffix
	{
		private readonly ILoadFileCellFormatter _loadFileCellFormatter;
		private readonly ILog _logger;

		public LineSuffix(ILoadFileCellFormatter loadFileCellFormatter, ILog logger)
		{
			_loadFileCellFormatter = loadFileCellFormatter;
			_logger = logger;
		}

		public void AddSuffix(DeferredEntry loadFileEntry)
		{
			string rowSuffix = _loadFileCellFormatter.RowSuffix;
			if (!string.IsNullOrEmpty(rowSuffix))
			{
				_logger.LogVerbose("Adding suffix to load file entries.");
				loadFileEntry.AddStringEntry(rowSuffix);
			}
		}
	}
}