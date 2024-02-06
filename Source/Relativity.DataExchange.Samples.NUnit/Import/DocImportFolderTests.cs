// ----------------------------------------------------------------------------
// <copyright file="DocImportFolderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Samples.NUnit.Import
{
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents a test that imports native documents with folders and validates the results.
	/// </summary>
	/// <remarks>
	/// Due to poor performance, disabling client-side implementation by default.
	/// </remarks>
	[TestFixture(false)]
	[TestFixture(true)]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[TestType.MainFlow]
	public class DocImportFolderTests : DocImportTestsBase
	{
		/// <summary>
		/// The flag that indicates whether to create folders server-side or client-side.
		/// </summary>
		private readonly bool serverSideFolders;

		/// <summary>
		/// Initializes a new instance of the <see cref="DocImportFolderTests"/> class.
		/// </summary>
		/// <param name="serverSideFolders">
		/// <see langword="true" /> to create all folders via server-side WebPI; otherwise, <see langword="false" /> to create all folders via client-side API.
		/// </param>
		public DocImportFolderTests(bool serverSideFolders)
		{
			this.serverSideFolders = serverSideFolders;
		}

		[Category(TestCategories.Folder)]
		[IdentifiedTestCase("70d5c71e-3785-43d3-8c1e-3c66f8e3b828", "00-t/e:s?t<s")]
		[IdentifiedTestCase("70d5c71e-3785-43d3-8c1e-3c66f8e3b828", "01-t>e\"s|t*s")]
		public void ShouldImportTheDocWhenTheFolderContainsInvalidChars(string invalidFolder)
		{
			// Arrange
			IList<string> initialFolders = this.QueryWorkspaceFolders();
			string controlNumber = GenerateControlNumber();
			string folder = $"\\{invalidFolder}-{this.serverSideFolders}";
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job =
				this.ArrangeImportJob(controlNumber, folder, SampleDocPdfFileName);

			// Act
			job.Execute();

			// Assert - the invalid folders were scrubbed and the import job is successful.
			this.AssertImportSuccess();

			// Assert - a new folder is added to the workspace.
			int expectedDocCount = initialFolders.Count + 1;
			IList<string> actualFolders = this.QueryWorkspaceFolders();
			Assert.That(actualFolders.Count, Is.EqualTo(expectedDocCount));
		}

		[Category(TestCategories.Folder)]
		[IdentifiedTestCase("4e1659d0-42cd-4976-9daf-0266622fbe00", "\\case-root1\\")]
		[IdentifiedTestCase("ac0ee7bb-155d-435d-88bb-4a5e6cae423f", "\\case-root1\\case-ROOT2")]
		[IdentifiedTestCase("312ee185-8cbd-4ac0-8a2b-ba9f9d6bc447", "\\case-ROOT1\\case-Root2")]
		public void ShouldNotDuplicateFoldersDueToCaseSensitivity(string folder)
		{
			// Arrange
			string controlNumber = GenerateControlNumber();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job =
				this.ArrangeImportJob(controlNumber, folder, SampleDocPdfFileName);

			// Act
			job.Execute();

			// Assert - the invalid folders were scrubbed and the import job is successful.
			this.AssertImportSuccess();

			// Assert - SQL collation is case-insensitive.
			int separators = folder.TrimEnd('\\').Count(x => x == '\\');
			if (separators == 1)
			{
				this.AssertDistinctFolders("case-root1");
			}
			else
			{
				this.AssertDistinctFolders("case-root1", "case-root2");
			}
		}

		[IdentifiedTestCase("95c100db-21ef-4ec2-ae4c-a1615cdf4329", 50)]
		public void ShouldSupportTheMaxFolderDepth(int maxDepth)
		{
			// Arrange
			string folderPath = GenerateFolderPath(maxDepth);
			List<DocImportRecord> records = AllSampleDocFileNames.Select(fileName => new DocImportRecord
			{
				ControlNumber = GenerateControlNumber(),
				File = ResourceFileHelper.GetDocsResourceFilePath(fileName),
				Folder = folderPath,
			}).ToList();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = this.ArrangeImportJob(records);

			// Act
			job.Execute();

			// Assert - the import job is successful.
			this.AssertImportSuccess();

			// Assert - all max depth folders were created.
			IEnumerable<string> folders = SplitFolderPath(folderPath);
			this.AssertDistinctFolders(folders.ToArray());
		}

		protected override void OnSetup()
		{
			base.OnSetup();
			AppSettings.Instance.CreateFoldersInWebApi = this.serverSideFolders;
		}
	}
}