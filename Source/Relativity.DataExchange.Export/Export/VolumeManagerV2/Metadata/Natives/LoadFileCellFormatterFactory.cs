namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives
{
	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using HtmlCellFormatter = Relativity.DataExchange.Export.HtmlCellFormatter;

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