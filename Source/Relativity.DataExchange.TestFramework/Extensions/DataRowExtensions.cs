// ----------------------------------------------------------------------------
// <copyright file="DataRowExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Extensions
{
	using System;
	using System.Data;

	internal static class DataRowExtensions
	{
		public static DataRow SetControlNumber(this DataRow dataRow, string controlNumber)
		{
			dataRow[WellKnownFields.ControlNumber] = controlNumber;
			return dataRow;
		}

		public static DataRow GenerateControlNumber(this DataRow dataRow, string fileName = null)
		{
			string fileNameSection = string.IsNullOrEmpty(fileName) ? string.Empty : fileName + "-";
			dataRow[WellKnownFields.ControlNumber] = "REL-" + fileNameSection + Guid.NewGuid();
			return dataRow;
		}

		public static string GetControlNumber(this DataRow dataRow)
		{
			return dataRow[WellKnownFields.ControlNumber] as string;
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
			dataRow[WellKnownFields.FileLocation] = folderName;
			return dataRow;
		}

		public static DataRow SetExtractedText(this DataRow dataRow, string extractedText)
		{
			dataRow[WellKnownFields.ExtractedText] = extractedText;
			return dataRow;
		}
	}
}