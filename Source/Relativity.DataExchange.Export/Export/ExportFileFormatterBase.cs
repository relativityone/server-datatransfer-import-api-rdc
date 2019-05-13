namespace Relativity.DataExchange.Export
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	public abstract class ExportFileFormatterBase : ILoadFileHeaderFormatter
	{
		private readonly IFieldNameProvider _fieldNameProvider;
		protected ExportFile ExportSettings { get; private set; }

		protected ExportFileFormatterBase(ExportFile exportSettings, IFieldNameProvider fieldNameProvider)
		{
			_fieldNameProvider = fieldNameProvider;
			ExportSettings = exportSettings;
		}

		public string GetHeader(List<ViewFieldInfo> columns)
		{
			if (columns != null && columns.Any())
			{
				string retString = GetHeaderLine(columns);
				return $"{retString}{Environment.NewLine}";
			}
			return string.Empty;
		}

		protected abstract string GetHeaderLine(List<ViewFieldInfo> columns);

		protected virtual string GetHeaderColName(ViewFieldInfo fieldInfo)
		{
			return _fieldNameProvider.GetDisplayName(fieldInfo);
		}
	}
}
