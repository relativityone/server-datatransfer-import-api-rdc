
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export
{
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
			if (columns.Any())
			{
				StringBuilder retString = GetHeaderLine(columns);
				retString.Append(Environment.NewLine);
				return retString.ToString();
			}
			return string.Empty;
		}

		protected abstract StringBuilder GetHeaderLine(List<ViewFieldInfo> columns);

		protected virtual string GetHeaderColName(ViewFieldInfo fieldInfo)
		{
			return _fieldNameProvider.GetDisplayName(fieldInfo);
		}
	}
}
