using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export
{
	public class ExportFileFormatterFactory : ILoadFileHeaderFormatterFactory
	{
		private readonly IFieldNameProvider _fieldNameProvider;

		public ExportFileFormatterFactory(IFieldNameProvider fieldNameProvider)
		{
			_fieldNameProvider = fieldNameProvider;
		}

		public ExportFileFormatterFactory() : this(new FieldNameProvider())
		{
		}

		public ILoadFileHeaderFormatter Create(ExportFile exportFile)
		{
			if (exportFile.LoadFileIsHtml)
			{
				return new HtmlExportFileFormatter(exportFile, _fieldNameProvider);
			}
			return new ExportFileFormatter(exportFile, _fieldNameProvider);
		}
	}
}
