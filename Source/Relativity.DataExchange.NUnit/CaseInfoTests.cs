// -----------------------------------------------------------------------------------------------------
// <copyright file="CaseInfoTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="CaseInfo"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System.Data;
	using System.Globalization;

	using global::NUnit.Framework;

	using Relativity.Import.Export.Service;

	[TestFixture]
	public class CaseInfoTests : SerializationTestsBase
	{
		[TestCase(null, null)]
		[TestCase("", "")]
		[TestCase(@"\\files2\T005\files\", @"\\files2\T005\files\")]
		[TestCase(@"\\files2\T005\files", @"\\files2\T005\files\")]
		[TestCase(@"C:\Windows\System32\", @"C:\Windows\System32\")]
		[TestCase(@"C:\Windows\System32", @"C:\Windows\System32\")]
		public static void ShouldAppendTheTrailingSlashOnTheDocumentPath(string value, string expected)
		{
			CaseInfo caseInfo = new CaseInfo { DocumentPath = value };
			Assert.That(caseInfo.DocumentPath, Is.EqualTo(expected));
		}

		[TestCase(null, null)]
		[TestCase("", "")]
		[TestCase("https://relativity.one", "https://relativity.one/")]
		[TestCase("https://relativity.one/", "https://relativity.one/")]
		public static void ShouldAppendTheTrailingSlashOnAllPaths(string value, string expected)
		{
			CaseInfo caseInfo = new CaseInfo { DownloadHandlerURL = value };
			Assert.That(caseInfo.DownloadHandlerURL, Is.EqualTo(expected));
		}

		[Test]
		public static void ShouldSerializeAndDeserializeTheObject()
		{
			CaseInfo expected = new CaseInfo
			{
				ArtifactID = 10,
				DocumentPath = @"\\files",
				DownloadHandlerURL = "https://relativity.one",
				EnableDataGrid = true,
				MatterArtifactID = 99,
				Name = "Test",
				RootArtifactID = 199,
				RootFolderID = 299,
				StatusCodeArtifactID = 399,
			};

			CaseInfo actual = BinarySerialize(expected) as CaseInfo;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual);
			actual = SoapSerialize(expected) as CaseInfo;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual);
		}

		[Test]
		public static void ShouldMapTheDataRow()
		{
			// Note: AsImportAllowed/ExportAllowed are NOT mapped.
			using (DataTable table = new DataTable())
			{
				table.Locale = CultureInfo.CurrentCulture;
				table.Columns.Add("ArtifactID", typeof(int));
				table.Columns.Add("DefaultFileLocationName", typeof(string));
				table.Columns.Add("DownloadHandlerApplicationPath", typeof(string));
				table.Columns.Add("EnableDataGrid", typeof(bool));
				table.Columns.Add("MatterArtifactID", typeof(int));
				table.Columns.Add("Name", typeof(string));
				table.Columns.Add("RootArtifactID", typeof(int));
				table.Columns.Add("RootFolderID", typeof(int));
				table.Columns.Add("StatusCodeArtifactID", typeof(int));
				DataRow row = table.NewRow();
				row["ArtifactID"] = 10;
				row["DefaultFileLocationName"] = @"\\files";
				row["DownloadHandlerApplicationPath"] = "https://relativity.one";
				row["EnableDataGrid"] = true;
				row["MatterArtifactID"] = 99;
				row["Name"] = "Test";
				row["RootArtifactID"] = 199;
				row["RootFolderID"] = 299;
				row["StatusCodeArtifactID"] = 399;
				table.Rows.Add(row);
				CaseInfo expected = new CaseInfo(row);
				ValidatePropertyValues(expected);
			}
		}

		private static void ValidatePropertyValues(CaseInfo actual)
		{
			// Note: AsImportAllowed/ExportAllowed are NOT mapped.
			Assert.That(actual.ArtifactID, Is.EqualTo(10));
			Assert.That(actual.DocumentPath, Is.EqualTo(@"\\files\"));
			Assert.That(actual.DownloadHandlerURL, Is.EqualTo("https://relativity.one/"));
			Assert.That(actual.EnableDataGrid, Is.True);
			Assert.That(actual.MatterArtifactID, Is.EqualTo(99));
			Assert.That(actual.Name, Is.EqualTo("Test"));
			Assert.That(actual.RootArtifactID, Is.EqualTo(199));
			Assert.That(actual.RootFolderID, Is.EqualTo(299));
			Assert.That(actual.StatusCodeArtifactID, Is.EqualTo(399));
		}
	}
}