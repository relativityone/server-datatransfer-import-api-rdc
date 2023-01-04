namespace Relativity.DataExchange.Export
{
	using System.Collections.Generic;
	using System.Text;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	public class ExportFileFormatter : ExportFileFormatterBase
	{
		private const string _FILE_PATH_COL_NAME = "FILE_PATH";
		private const string _PDF_PATH_COL_NAME = "PDF_PATH";

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
				stringBuilder.AppendFormat($"{ExportSettings.QuoteDelimiter}{_FILE_PATH_COL_NAME}{ExportSettings.QuoteDelimiter}{ExportSettings.RecordDelimiter}");
			}
			if (ExportSettings.ExportPdf)
			{
				stringBuilder.AppendFormat($"{ExportSettings.QuoteDelimiter}{_PDF_PATH_COL_NAME}{ExportSettings.QuoteDelimiter}{ExportSettings.RecordDelimiter}");
			}
			return stringBuilder.ToString().TrimEnd(ExportSettings.RecordDelimiter);
		}
	}


}
