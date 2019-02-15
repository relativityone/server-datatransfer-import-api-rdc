// ----------------------------------------------------------------------------
// <copyright file="DocImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Samples.NUnit.Tests
{
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using global::NUnit.Framework;

    using Relativity.ImportExport.UnitTestFramework;

    /// <summary>
    /// Represents a test that imports native documents and validates the results.
    /// </summary>
    [TestFixture]
	public class DocImportTests : DocImportTestsBase
	{
		/// <summary>
		/// Gets the test case data.
		/// </summary>
		/// <value>
		/// The <see cref="TestCaseData"/> instances.
		/// </value>
		/// <remarks>
		/// Ensure that duplicate folders never cause failures.
		/// </remarks>
		private static IEnumerable<TestCaseData> TestCases =>
			new List<TestCaseData>
				{
					new TestCaseData(SampleDocPdfFileName, null),
					new TestCaseData(SampleDocWordFileName, string.Empty),
					new TestCaseData(SampleDocExcelFileName, "\\doc-import-root1"),
					new TestCaseData(SampleDocMsgFileName, "\\doc-import-root1"),
					new TestCaseData(SampleDocHtmFileName, "\\doc-import-root1\\doc-import-root2"),
					new TestCaseData(SampleDocEmfFileName, "\\doc-import-root1\\doc-import-root2"),
					new TestCaseData(SampleDocPptFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3"),
					new TestCaseData(SampleDocPngFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3"),
					new TestCaseData(SampleDocTxtFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3\\doc-import-root4"),
					new TestCaseData(SampleDocWmfFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3\\doc-import-root4")
				};

		[Test]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheDoc(string fileName, string folderPath)
		{
			// Arrange
			int initialDocumentCount = this.QueryRelativityObjectCount((int)kCura.Relativity.Client.ArtifactType.Document);
			string controlNumber = GenerateControlNumber();
			kCura.Relativity.ImportAPI.ImportAPI importApi = this.CreateImportApiObject();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			this.ConfigureJobSettings(
				job,
				this.ArtifactTypeId,
				this.IdentifierFieldId,
				WellKnownFields.FilePath,
				WellKnownFields.ControlNumber,
				WellKnownFields.FolderName);
			this.ConfigureJobEvents(job);

			// Setup the data source.
			this.DataSource.Columns.AddRange(new[]
			{
				new DataColumn(WellKnownFields.ControlNumber, typeof(string)),
				new DataColumn(WellKnownFields.FilePath, typeof(string)),
				new DataColumn(WellKnownFields.FolderName, typeof(string))
			});

			// Add the file to the data source.
			string file = ResourceFileHelper.GetDocsResourceFilePath(fileName);
			this.DataSource.Rows.Add(controlNumber, file, folderPath);
			job.SourceData.SourceData = this.DataSource.CreateDataReader();

			// Act
			job.Execute();

			// Assert - the import job is successful.
			this.AssertImportSuccess();

			// Assert - the object count is incremented by 1.
			int expectedDocCount = initialDocumentCount + this.DataSource.Rows.Count;
			int actualDocCount = this.QueryRelativityObjectCount((int)kCura.Relativity.Client.ArtifactType.Document);
			Assert.That(actualDocCount, Is.EqualTo(expectedDocCount));

			// Assert - the imported document exists.
			IList<Relativity.Services.Objects.DataContracts.RelativityObject> docs =
				this.QueryRelativityObjects(this.ArtifactTypeId, new[] { WellKnownFields.ControlNumber });
			Assert.That(docs, Is.Not.Null);
			Assert.That(docs.Count, Is.EqualTo(expectedDocCount));
			Relativity.Services.Objects.DataContracts.RelativityObject importedObj
				= FindRelativityObject(docs, WellKnownFields.ControlNumber, controlNumber);
			Assert.That(importedObj, Is.Not.Null);

			// Assert - the workspace doesn't include duplicate folders.
			if (!string.IsNullOrEmpty(folderPath))
			{
				IEnumerable<string> folders = SplitFolderPath(folderPath);
				this.AssertDistinctFolders(folders.ToArray());
			}
		}
	}
}