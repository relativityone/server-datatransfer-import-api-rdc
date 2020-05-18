// <copyright file="MultiResourcePoolExportTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[Explicit]
	public class MultiResourcePoolExportTests : ExportTestBase
	{
		private ResourcePoolHelper resourcePoolHelper;

		private IntegrationTestParameters testParameters;

		protected override IntegrationTestParameters TestParameters => testParameters;

		[OneTimeSetUp]
		public async Task OneTimeSetUp()
		{
			testParameters = IntegrationTestHelper.Create();

			resourcePoolHelper = new ResourcePoolHelper(TestParameters);

			ImportDocumentsAndImages();

			await resourcePoolHelper.CreateResourcePoolWithFileShareAsync().ConfigureAwait(false);

			ImportDocumentsAndImages();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			IntegrationTestHelper.Destroy(testParameters);
		}

		[IdentifiedTest("2EAFF4D8-ED3B-412C-B506-D35574130C04")]
		[TestCase(TapiClient.Aspera)]
		[TestCase(TapiClient.Direct)]
		[TestCase(TapiClient.Web)]
		public void ShouldExportAllSampleDocAndImagesFromTwoFileShares(TapiClient client)
		{
			// ARRANGE
			GivenTheTapiForceClientAppSettings(client);

			ExtendedExportFileSetup.SetupDocumentExport(ExtendedExportFile);
			ExtendedExportFileSetup.SetupImageExport(ExtendedExportFile);
			ExtendedExportFileSetup.SetupPaddings(ExtendedExportFile);
			ExtendedExportFileSetup.SetupDelimiters(ExtendedExportFile);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			int expectedDocumentsCountInBothShares = 2 * TestData.SampleDocFiles.Count();
			int imagesPerDocumentCount = TestData.SampleImageFiles.Count();
			int metadataFilesCount = 2;
			int expectedExportedFilesCount = metadataFilesCount + expectedDocumentsCountInBothShares + (expectedDocumentsCountInBothShares * imagesPerDocumentCount);

			// ASSERT
			this.ThenTheExportJobIsSuccessful(expectedDocumentsCountInBothShares);
			ThenTheFilesAreExported(expectedExportedFilesCount);
		}

		private void ImportDocumentsAndImages()
		{
			var documents = ImportHelper.ImportDocuments(TestParameters);
			ImportHelper.ImportImagesForDocuments(TestParameters, documents);
		}
	}
}