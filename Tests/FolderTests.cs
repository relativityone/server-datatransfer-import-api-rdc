using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using kCura.Data.RowDataGateway;
using Relativity.Data;
using Enumerable = System.Linq.Enumerable;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Tests
{
	[TestFixture]
	public class FolderTests
	{
		private string _tableName;
		private BaseContext _context = new Context(_connectionString);

		private const int _UNKNOWN_PARENT_FOLDER_ID = -9;
		private const int _ROOT_FOLDER_ID = -7;
		private static readonly string _connectionString = ConfigurationManager.AppSettings["connectionString"];


		private int CreateTable() =>
			_context.ExecuteNonQuerySQLStatement(String.Format(@"CREATE TABLE [Resource].[{0}] (
				[kCura_Import_ParentFolderID] [nvarchar](200) NOT NULL,
				[kCura_Import_ParentFolderPath] [nvarchar](200) NOT NULL,
				)", _tableName));

		private int DropTable() =>
			_context.ExecuteNonQuerySQLStatement(String.Format(@"DROP TABLE [Resource].[{0}]", _tableName));

		private int InsertData(Dictionary<string, int> rows)
		{
			string insertSql = rows
				.Select(r => $"INSERT INTO [Resource].[{_tableName}] VALUES ('{r.Value}', '{r.Key}');")
				.Aggregate("", (sql, rowSql) => sql + rowSql);
			return _context.ExecuteNonQuerySQLStatement(insertSql);
		}

		private void VerifyRows(Dictionary<string, int> folderPathToId)
		{
			DataTable rows = _context.ExecuteSqlStatementAsDataTable($"SELECT * FROM [Resource].[{_tableName}]");
			foreach (DataRow row in rows.Rows)
			{
				int expected = folderPathToId[row["kCura_Import_ParentFolderPath"].ToString()];
				int actual = Convert.ToInt32(row["kCura_Import_ParentFolderID"]);
				Assert.AreEqual(expected, actual, "Wrong ParentFolderID was found in DB");
			}
		}


		[SetUp]
		public void SetUp()
		{
			_tableName = $"Table_{System.Guid.NewGuid().ToString()}";
			CreateTable();
		}

		[TearDown]
		public void TearDown()
		{
			DropTable();
			_tableName = null;
		}

		[Test]
		public void UpdateFolderIdsForFolderPathsTest_HighVolume_AllRowsKnown()
		{
			// Assign
			const int folderPathToIdWithUnknownIdNum = 5000;
			const int folderPathToIdWithDefinedIdNum = 100;
			const int folderPathToIdNotInDbNum = 100;

			Func<int, string> getFolderPath =
				(x) => $"Folder/Path{x}";

			#pragma warning disable RG2007 // Explicit Type Declaration
			var folderPathToIdWithUnknownId =
				Enumerable.Range(0, folderPathToIdWithUnknownIdNum)
					.Select(x => new { Key = getFolderPath(x), Val = _UNKNOWN_PARENT_FOLDER_ID });

			var folderPathToIdWithDefinedId =
				Enumerable.Range(folderPathToIdWithUnknownIdNum, folderPathToIdWithDefinedIdNum)
					.Select(x =>
				{
					int val;
					const int variantsNum = 3;
					const int arbitraryVal = 10;
					switch (x % variantsNum)
					{
						case 0:
							val = x;
							break;
						case 1:
							val = _ROOT_FOLDER_ID;
							break;
						default:
							val = x * arbitraryVal;
							break;
					}
					return new { Key = getFolderPath(x), Val = val };
				});

			var folderPathToIdNotInDb =
				Enumerable.Range(folderPathToIdWithUnknownIdNum + folderPathToIdWithDefinedIdNum, folderPathToIdNotInDbNum)
					.Select(x => new { Key = getFolderPath(x), Val = x });
			#pragma warning restore RG2007 // Explicit Type Declaration

			Dictionary<string, int> initialData =
				folderPathToIdWithUnknownId
					.Concat(folderPathToIdWithDefinedId)
					.ToDictionary(x => x.Key, y => y.Val);

			InsertData(initialData);

			Dictionary<string, int> expected =
				folderPathToIdWithUnknownId
					.Select((x, i) => new { Key = x.Key, Val = i })
					.Concat(folderPathToIdWithDefinedId)
					.ToDictionary(x => x.Key, y => y.Val);

			Dictionary<string, int> inputDict =
				folderPathToIdWithUnknownId
					.Select((x, i) => new { Key = x.Key, Val = i })
					.Concat(folderPathToIdWithDefinedId)
					.Concat(folderPathToIdNotInDb)
					.ToDictionary(x => x.Key, y => y.Val);

			// Act
			Folder.UpdateFolderIdsForFolderPaths(_context, _tableName, inputDict, _UNKNOWN_PARENT_FOLDER_ID,
				_ROOT_FOLDER_ID);

			// Assert
			VerifyRows(expected);
		}

	}
}
