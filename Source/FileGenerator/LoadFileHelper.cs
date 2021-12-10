using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileGenerator
{
	public class LoadFileHelper
	{
		private readonly List<string> _columns;

		public LoadFileHelper()
		{
			_columns = new List<string>();
		}

		public string CreateLoadFileHeader()
		{
			BeginNewLine();
			AddColumn("Control Number");
			AddColumn("Source File");
			AddColumn("Custodian");
			AddColumn("Date Created");
			AddColumn("Date Last Modified");
			AddColumn("Date Received");
			AddColumn("Date Sent");
			AddColumn("Document Extension");
			AddColumn("Email From");
			AddColumn("Email Subject");
			AddColumn("Email To");
			AddColumn("Extracted Text");
			AddColumn("Filename");
			AddColumn("Filesize");
			AddColumn("Subject");
			AddColumn("FILE_PATH");

			return GetLine();
		}

		public string CreateLoadFileLine(int ctrlNum, int custodianNr, string srcFile, string extractedTxt, string fileName, int fileSize, string nativeFilePath)
		{
			var dateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
			BeginNewLine();
			AddColumn($"Control_Number{ctrlNum,7:D8}");
			AddColumn(srcFile);
			AddColumn($"Custodian_{custodianNr,7:D8}");
			AddColumn(dateTime);
			AddColumn(dateTime);
			AddColumn(dateTime);
			AddColumn(dateTime);
			AddColumn(Path.GetExtension(nativeFilePath).Substring(1));
			AddColumn("EmailFrom@email.com");
			AddColumn("Email Subject");
			AddColumn("EmailTo@email.com");
			AddColumn(extractedTxt);
			AddColumn(fileName);
			AddColumn(fileSize.ToString());
			AddColumn("Subject");
			AddColumn(nativeFilePath);

			return GetLine();
		}

		private void BeginNewLine()
		{
			_columns.Clear();
		}

		private void AddColumn(string name)
		{
			_columns.Add(name);
		}

		private string GetLine()
		{
			var sb = new StringBuilder();

			for(var i = 0; i < _columns.Count; i++)
			{
				sb.Append($"^{_columns[i]}^");
				if(i != _columns.Count-1)
					sb.Append("|");
			}

			return sb.ToString();
		}
	}
}
