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
	[TestFixture]
    [Feature.DataTransfer.DocumentExportApi.Operations.ExportFolderAndSubfolders]
	[TestType.EdgeCase]
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA1801: Review unused parameters",
			Justification = "We are using TestExecutionContext.CurrentContext.CurrentTest.Arguments to retrieve value of client parameter.")]
		[IdentifiedTestCase("2EAFF4D8-ED3B-412C-B506-D35574130C04", TapiClient.Aspera)]
		[IdentifiedTestCase("7CF052B9-836D-4E1A-A1EF-35D0C15DD22B", TapiClient.Direct)]
		[IdentifiedTestCase("C3D2530A-0E1E-4498-8D57-58D7BE755FF0", TapiClient.Web)]
		public void ShouldExportAllSampleDocAndImagesFromTwoFileShares(TapiClient client)
		{
			// ARRANGE
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

			this.ThenTheCorrelationIdWasRetrieved();
		}

		private void ImportDocumentsAndImages()
		{
			var documents = ImportHelper.ImportDocuments(TestParameters);
			ImportHelper.ImportImagesForDocuments(TestParameters, documents);
		}
	}
}