using System.Collections.Generic;
using System.Text;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export
{
	public class ExportFileFormatter : ExportFileFormatterBase
	{
		private const string _FILE_PATH_COL_NAME = "FILE_PATH";

		public ExportFileFormatter(ExportFile exportSettings, IFieldNameProvider fieldNameProvider) : base(exportSettings, fieldNameProvider)
		{
		}

		protected override string GetHeaderLine(List<ViewFieldInfo> columns)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ViewFieldInfo field in columns)
			{
				string headerColName = GetHeaderColName(field);
				stringBuilder.AppendFormat($"{ExportSettings.QuoteDelimiter}{headerColName}{ExportSettings.QuoteDelimiter}{ExportSettings.RecordDelimiter}");
			}
			if (ExportSettings.ExportNative)
			{
				stringBuilder.AppendFormat($"{ExportSettings.QuoteDelimiter}{_FILE_PATH_COL_NAME}{ExportSettings.QuoteDelimiter}");
			}
			return stringBuilder.ToString().TrimEnd(ExportSettings.RecordDelimiter);
		}
	}


}
