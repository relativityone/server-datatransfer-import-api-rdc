using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LineImageField
	{
		private readonly ILoadFileCellFormatter _loadFileCellFormatter;

		public LineImageField(ILoadFileCellFormatter loadFileCellFormatter)
		{
			_loadFileCellFormatter = loadFileCellFormatter;
		}

		public void AddImageField(DeferredEntry loadFileEntry, ObjectExportInfo artifact)
		{
			string imagesCell = _loadFileCellFormatter.CreateImageCell(artifact);
			if (!string.IsNullOrEmpty(imagesCell))
			{
				loadFileEntry.AddStringEntry(imagesCell);
			}
		}
	}
}