// ----------------------------------------------------------------------------
// <copyright file="DataTableExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Extensions
{
	using System.Data;

	internal static class DataTableExtensions
	{
		public static DataTable WithControlNumber(this DataTable dataTable)
		{
			dataTable.Columns.Add(new DataColumn(WellKnownFields.ControlNumber, typeof(string)));
			return dataTable;
		}

		public static DataTable WithFileLocation(this DataTable dataTable)
		{
			dataTable.Columns.Add(new DataColumn(WellKnownFields.FileLocation, typeof(string)));
			return dataTable;
		}

		public static DataTable WithFilePath(this DataTable dataTable)
		{
			dataTable.Columns.Add(new DataColumn(WellKnownFields.FilePath, typeof(string)));
			return dataTable;
		}

		public static DataTable WithFolderName(this DataTable dataTable)
		{
			dataTable.Columns.Add(new DataColumn(WellKnownFields.FolderName, typeof(string)));
			return dataTable;
		}

		public static DataTable WithExtractedText(this DataTable dataTable)
		{
			dataTable.Columns.Add(new DataColumn(WellKnownFields.ExtractedText, typeof(string)));
			return dataTable;
		}

		public static DataTable WithBatesNumber(this DataTable dataTable)
		{
			dataTable.Columns.Add(new DataColumn(WellKnownFields.BatesNumber, typeof(string)));
			return dataTable;
		}
	}
}