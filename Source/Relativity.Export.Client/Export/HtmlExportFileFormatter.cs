using System;
using System.Collections.Generic;
using System.Text;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export
{
	public class HtmlExportFileFormatter  : ExportFileFormatterBase
	{
		public HtmlExportFileFormatter(ExportFile exportSettings, IFieldNameProvider fieldNameProvider) : base(exportSettings, fieldNameProvider)
		{
		}

		protected override string GetHeaderLine(List<ViewFieldInfo> columns)
		{
			StringBuilder retString = new StringBuilder();
			
			retString.Append("<html><head><title>" + System.Web.HttpUtility.HtmlEncode(ExportSettings.CaseInfo.Name) + "</title>");
			retString.Append("<style type='text/css'>" + Environment.NewLine);
			retString.Append("td {vertical-align: top;background-color:#EEEEEE;}" + Environment.NewLine);
			retString.Append("th {color:#DDDDDD;text-align:left;}" + Environment.NewLine);
			retString.Append("table {background-color:#000000;}" + Environment.NewLine);
			retString.Append("</style>" + Environment.NewLine);
			retString.Append("</head><body>" + Environment.NewLine);
			retString.Append("<table width='100%'><tr>" + Environment.NewLine);
			
			foreach (ViewFieldInfo field in columns)
			{
				string headerColName = GetHeaderColName(field);
				retString.AppendFormat("{0}{1}{2}", "<th>", System.Web.HttpUtility.HtmlEncode(headerColName), "</th>");
			}
			if (ExportSettings.ExportImages && ExportSettings.ArtifactTypeID == (int)Relativity.ArtifactType.Document)
			{
				retString.Append("<th>Image Files</th>");
			}
			if (ExportSettings.ExportNative)
			{
				retString.Append("<th>Native Files</th>");
			}
			retString.Append(Environment.NewLine + "</tr>" + Environment.NewLine);
			return retString.ToString();
		}
	}
}
