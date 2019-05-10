namespace Relativity.Export.VolumeManagerV2.Metadata.Natives
{
	using Relativity.Export.VolumeManagerV2.Directories;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	public class LoadFileCellFormatterFactory
	{
		public ILoadFileCellFormatter Create(ExportFile exportSettings, IFilePathTransformer filePathTransformer)
		{
			if (exportSettings.LoadFileIsHtml)
			{
				return new Relativity.Export.VolumeManagerV2.HtmlCellFormatter(exportSettings, filePathTransformer);
			}

			return new DelimitedCellFormatter(exportSettings);
		}
	}
}