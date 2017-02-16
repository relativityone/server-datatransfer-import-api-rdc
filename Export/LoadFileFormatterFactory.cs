using System;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export
{
	public class LoadFileFormatterFactory : ILoadFileHeaderFormatterFactory
	{
		private readonly IFieldNameProvider _fieldNameProvider;

		public LoadFileFormatterFactory(IFieldNameProvider fieldNameProvider)
		{
			_fieldNameProvider = fieldNameProvider;
		}

		public LoadFileFormatterFactory() : this(new FieldNameProvider())
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
