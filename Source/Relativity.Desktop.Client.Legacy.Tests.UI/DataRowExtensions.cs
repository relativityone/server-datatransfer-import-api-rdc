using System;
using System.Data;
using Relativity.DataExchange.TestFramework;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	internal static class DataRowExtensions
	{
		public static DataRow SetControlNumber(this DataRow dataRow, string controlNumber)
		{
			dataRow[WellKnownFields.ControlNumber] = controlNumber;
			return dataRow;
		}

		public static DataRow GenerateControlNumber(this DataRow dataRow)
		{
			dataRow[WellKnownFields.ControlNumber] = "REL-" + Guid.NewGuid();
			return dataRow;
		}

		public static string GetControlNumber(this DataRow dataRow)
		{
			return (string) dataRow[WellKnownFields.ControlNumber];
		}

		public static DataRow SetFilePath(this DataRow dataRow, string filePath)
		{
			dataRow[WellKnownFields.FilePath] = filePath;
			return dataRow;
		}

		public static DataRow SetFolderName(this DataRow dataRow, string folderName)
		{
			dataRow[WellKnownFields.FolderName] = folderName;
			return dataRow;
		}

		public static DataRow SetBatesNumber(this DataRow dataRow, string folderName)
		{
			dataRow[WellKnownFields.BatesNumber] = folderName;
			return dataRow;
		}

		public static DataRow SetFileLocation(this DataRow dataRow, string folderName)
		{
			dataRow[WellKnownFields.BatesNumber] = folderName;
			return dataRow;
		}

		public static DataRow SetExtractedText(this DataRow dataRow, string extractedText)
		{
			dataRow["Extracted Text"] = extractedText;
			return dataRow;
		}
	}
}