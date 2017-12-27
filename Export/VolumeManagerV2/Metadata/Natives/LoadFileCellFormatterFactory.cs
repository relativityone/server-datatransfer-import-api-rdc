using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LoadFileCellFormatterFactory
	{
		public ILoadFileCellFormatter Create(ExportFile exportSettings)
		{
			if (exportSettings.LoadFileIsHtml)
			{
				return new HtmlCellFormatter(exportSettings);
			}

			return new DelimitedCellFormatter(exportSettings);
		}
	}
}