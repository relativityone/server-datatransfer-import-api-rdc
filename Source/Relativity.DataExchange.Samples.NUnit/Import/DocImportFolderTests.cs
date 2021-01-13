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
		[IdentifiedTestCase("70d5c71e-3785-43d3-8c1e-3c66f8e3b828", "00-te/st")]
		[IdentifiedTestCase("eefa35e6-1fa6-4274-95d1-55fe9e842d92", "01-te:st")]
		[IdentifiedTestCase("181c003e-ac1b-47d7-a610-54c354270f63", "02-te?st")]
		[IdentifiedTestCase("4490bab0-7fcd-4ef4-b66a-53d55d686de6", "03-te<st")]
		[IdentifiedTestCase("4f8df583-44d6-4d61-a76e-d178d5518704", "04-te>st")]
		[IdentifiedTestCase("28e498c8-b482-4b67-8d19-92e65b3f5de2", "05-te\"st")]
		[IdentifiedTestCase("85fd5ebd-34ce-441a-9428-ec54371c2127", "06-te|st")]
		[IdentifiedTestCase("9af91d7f-c301-4df9-8500-e970a36d9e08", "07-te*st")]
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
		[IdentifiedTestCase("0dc14fcc-5e93-4fa0-8641-e1b63e88c637", "\\case-root1")]
		[IdentifiedTestCase("4e1659d0-42cd-4976-9daf-0266622fbe00", "\\case-root1\\")]
		[IdentifiedTestCase("ac0ee7bb-155d-435d-88bb-4a5e6cae423f", "\\case-root1\\case-root2")]
		[IdentifiedTestCase("312ee185-8cbd-4ac0-8a2b-ba9f9d6bc447", "\\case-root1\\case-Root2")]
		[IdentifiedTestCase("714d52ea-1c95-4538-a7e8-095fd73516d6", "\\case-ROOT1\\case-root2")]
		[IdentifiedTestCase("3357efe2-cd52-47eb-a977-a75a5db86b29", "\\case-ROOT1\\case-Root2")]
		[IdentifiedTestCase("8cec9c61-a795-437f-9ece-0b30ded9f0bb", "\\case-ROOT1\\case-ROOT2")]
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

		[IdentifiedTestCase("927a0d4c-acf1-49c5-a5dc-421363737458", 10)]
		[IdentifiedTestCase("f4c0f311-952f-416a-b09e-990d2022b60e", 25)]
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