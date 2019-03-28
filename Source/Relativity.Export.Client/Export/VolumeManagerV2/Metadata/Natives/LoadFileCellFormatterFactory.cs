using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LoadFileCellFormatterFactory
	{
		public ILoadFileCellFormatter Create(ExportFile exportSettings, IFilePathTransformer filePathTransformer)
		{
			if (exportSettings.LoadFileIsHtml)
			{
				return new HtmlCellFormatter(exportSettings, filePathTransformer);
			}

			return new DelimitedCellFormatter(exportSettings);
		}
	}
}