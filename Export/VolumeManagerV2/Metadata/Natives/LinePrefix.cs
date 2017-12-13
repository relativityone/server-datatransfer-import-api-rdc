using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LinePrefix
	{
		private readonly ILoadFileCellFormatter _loadFileCellFormatter;

		public LinePrefix(ILoadFileCellFormatter loadFileCellFormatter)
		{
			_loadFileCellFormatter = loadFileCellFormatter;
		}

		public void AddPrefix(DeferredEntry loadFileEntry)
		{
			string rowPrefix = _loadFileCellFormatter.RowPrefix;

			if (!string.IsNullOrEmpty(rowPrefix))
			{
				loadFileEntry.AddStringEntry(rowPrefix);
			}
		}
	}
}