
using System.Collections.Generic;
using System.Text;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export
{
	public class ExportFileFormatter : ExportFileFormatterBase
	{
		public ExportFileFormatter(ExportFile exportSettings, IFieldNameProvider fieldNameProvider) : base(exportSettings, fieldNameProvider)
		{
		}

		protected override StringBuilder GetHeaderLine(List<ViewFieldInfo> columns)
		{
			StringBuilder retString = new StringBuilder();
			foreach (ViewFieldInfo field in columns)
			{
				retString.AppendFormat("{0}{1}{0}{2}", ExportSettings.QuoteDelimiter, GetHeaderColName(field),
					ExportSettings.RecordDelimiter);
			}
			// Remove last RecordDelimiter from the header line
			retString.Length--;

			if (ExportSettings.ExportNative)
			{
				retString.AppendFormat("{2}{0}{1}{0}", ExportSettings.QuoteDelimiter, "FILE_PATH", ExportSettings.RecordDelimiter);
			}
			return retString;
		}
	}

	
}
