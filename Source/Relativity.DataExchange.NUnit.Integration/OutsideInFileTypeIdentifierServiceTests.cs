// -----------------------------------------------------------------------------------------------------
// <copyright file="OutsideInFileTypeIdentifierServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="OutsideInFileTypeIdentifierService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration
{
	using System;
	using System.Data;
	using System.Globalization;
	using System.IO;
	using System.IO.Compression;

	using FileHelpers;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="OutsideInFileTypeIdentifierService"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class OutsideInFileTypeIdentifierServiceTests
	{
		private static int timeoutSeconds = 30;
		private IFileTypeIdentifier service;
		private string rootDatasetDocumentsDirectory;

		[SetUp]
		public void Setup()
		{
			OutsideInFileTypeIdentifierService.Shutdown();
			this.service = new OutsideInFileTypeIdentifierService(timeoutSeconds);
			string rootDirectory = Path.Combine(Path.GetTempPath(), "Relativity-Test-Datasets");
#if RELEASE
			if (System.IO.Directory.Exists(rootDirectory))
			{
				const bool Recursive = true;
				System.IO.Directory.Delete(rootDirectory, Recursive);
			}
#endif

			if (!System.IO.Directory.Exists(rootDirectory))
			{
				System.IO.Directory.CreateDirectory(rootDirectory);
			}

			this.rootDatasetDocumentsDirectory = Path.Combine(rootDirectory, "Test-FileTypeId-List");
			if (!System.IO.Directory.Exists(this.rootDatasetDocumentsDirectory))
			{
				System.IO.Directory.CreateDirectory(this.rootDatasetDocumentsDirectory);
				string sourceZipFile = ResourceFileHelper.GetResourceFilePath("OutsideIn", "Test-FileTypeId-List.zip");
				ZipFile.ExtractToDirectory(sourceZipFile, this.rootDatasetDocumentsDirectory);
			}
		}

		[TearDown]
		public void Teardown()
		{
			if (this.service != null)
			{
				this.service.Dispose();
				this.service = null;
			}
		}

		[IdentifiedTest("12b6eb4c-37f6-4a8e-b2e5-f4e0735b1c02")]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.OutsideIn)]
		public void ShouldVerifyTheOutsideInGoldenDataset()
		{
			// arrange
			int rowIndex = 0;
			int skipped = 0;
			string testCsvPath = ResourceFileHelper.GetResourceFilePath("OutsideIn", "Test-FileTypeId-List-2018-9.csv");
			string goldenCsvPath = ResourceFileHelper.GetResourceFilePath("OutsideIn", "Test-FileTypeId-List-Golden-2018-9.csv");
			using (DataTable goldenCsvImport = CsvEngine.CsvToDataTable(goldenCsvPath, ','))
			using (DataTable testResults = new DataTable())
			{
				goldenCsvImport.Locale = CultureInfo.CurrentCulture;
				testResults.Locale = CultureInfo.CurrentCulture;
				testResults.Columns.Add("Id", typeof(int));
				testResults.Columns.Add("Description", typeof(string));
				testResults.Columns.Add("FileName", typeof(string));
				DataTable csvImport = CsvEngine.CsvToDataTable(testCsvPath, ',');

				// act
				foreach (DataRow row in csvImport.Rows)
				{
					string sourceFileName = row[0].ToString();
					string sourcePath = System.IO.Path.Combine(this.rootDatasetDocumentsDirectory, sourceFileName.TrimStart('.', '\\'));
					if (!System.IO.File.Exists(sourcePath))
					{
						testResults.Rows.Add(int.MinValue, string.Empty, string.Empty);
						skipped++;
						Console.WriteLine($">>> Skipping {sourcePath}");
					}
					else
					{
						IFileTypeInfo info = this.service.Identify(sourcePath);
						testResults.Rows.Add(info.Id, info.Description, sourceFileName);
					}
				}

				// assert
				Assert.That(
					goldenCsvImport.Rows.Count,
					Is.EqualTo(testResults.Rows.Count),
					"The number of rows being compared did not match.");
				foreach (DataRow goldenDataRow in goldenCsvImport.Rows)
				{
					int expectedId = int.Parse(goldenDataRow["Id"].ToString());
					var testId = int.Parse(testResults.Rows[rowIndex]["Id"].ToString());
					if (testId == int.MinValue)
					{
						continue;
					}

					var expectedFileType = goldenDataRow["Description"].ToString();
					var testFileType = testResults.Rows[rowIndex]["Description"].ToString();
					var expectedFileName = goldenDataRow["FileName"].ToString();
					var testFileName = testResults.Rows[rowIndex]["FileName"].ToString();
					Assert.That(testId, Is.EqualTo(expectedId));
					Assert.That(testFileType, Is.EqualTo(expectedFileType));
					Assert.That(testFileName, Is.EqualTo(expectedFileName));
					rowIndex++;
				}

				Assert.That(rowIndex, Is.Not.Zero, "No were were verified and suggests the CSV files aren't aligned with the ZIP dataset.");
			}
		}
	}
}