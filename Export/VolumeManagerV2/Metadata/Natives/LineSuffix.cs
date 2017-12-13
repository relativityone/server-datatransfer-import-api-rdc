using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LineSuffix
	{
		private readonly ILoadFileCellFormatter _loadFileCellFormatter;

		public LineSuffix(ILoadFileCellFormatter loadFileCellFormatter)
		{
			_loadFileCellFormatter = loadFileCellFormatter;
		}

		public void AddSuffix(DeferredEntry loadFileEntry)
		{
			string rowSuffix = _loadFileCellFormatter.RowSuffix;
			if (!string.IsNullOrEmpty(rowSuffix))
			{
				loadFileEntry.AddStringEntry(rowSuffix);
			}
		}
	}
}