// <copyright file="MissingFileExportTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[Explicit]
	public class MissingFileExportTests : ExportTestBase
	{
		private IntegrationTestParameters testParameters;

		protected override IntegrationTestParameters TestParameters => testParameters;

		[OneTimeSetUp]
		public async Task OneTimeSetUp()
		{
			testParameters = IntegrationTestHelper.Create();

			ImportDocumentsAndImages();

			await RemoveFileFromFileShare().ConfigureAwait(false);
		}

		[IdentifiedTest("2013DF1B-46DE-4755-B96B-AF454CA5D2CE")]
		[TestCase(TapiClient.Direct)]
		[TestCase(TapiClient.Web)]
		public void ExportShouldReportErrorOnMissingFile(TapiClient client)
		{
			// ARRANGE
			GivenTheTapiForceClientAppSettings(client);

			ExtendedExportFileSetup.SetupDocumentExport(ExtendedExportFile);
			ExtendedExportFileSetup.SetupImageExport(ExtendedExportFile);
			ExtendedExportFileSetup.SetupPaddings(ExtendedExportFile);
			ExtendedExportFileSetup.SetupDelimiters(ExtendedExportFile);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			int expectedDocumentsCount = TestData.SampleDocFiles.Count();
			int imagesPerDocumentCount = TestData.SampleImageFiles.Count();
			int metadataFilesCount = 2;
			int expectedMissingFilesCount = 1;
			int expectedExportedFilesCount = metadataFilesCount + expectedDocumentsCount + (expectedDocumentsCount * imagesPerDocumentCount) - expectedMissingFilesCount;

			// ASSERT
			this.ThenTheExportJobIsNotSuccessful(expectedDocsProcessed: expectedDocumentsCount, expectedErrorsCount: expectedMissingFilesCount);
			ThenTheFilesAreExported(expectedExportedFilesCount);
		}

		private async Task RemoveFileFromFileShare()
		{
			var queryHelper = new SqlQueryHelper(TestParameters);
			var fileNameToRemove = Path.GetFileName(TestData.SampleDocFiles.First());

			string fileLocationQuery =
				$@"SELECT [Location] FROM [EDDS{TestParameters.WorkspaceId}].[EDDSDBO].[File] WHERE [Filename] = '{fileNameToRemove}'";

			DataTable result = await queryHelper.ExecuteQueryAsync(fileLocationQuery).ConfigureAwait(false);
			string path = (string)result.Rows[0][0];
			File.Delete(path);
		}

		private void ImportDocumentsAndImages()
		{
			var documents = ImportHelper.ImportDocuments(TestParameters);
			ImportHelper.ImportImagesForDocuments(TestParameters, documents);
		}
	}
}