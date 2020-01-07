// <copyright file="MultiResourcePoolExportTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

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
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(TestParameters, client);

			// ARRANGE
			GivenTheTapiForceClientAppSettings(client);

			SetupDocumentExport();
			SetupImageExport();
			SetupPaddings();
			SetupDelimiters();

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(TestData.SampleDocFiles.Count() * 2);
		}

		private void SetupDocumentExport()
		{
			var encoding = Encoding.GetEncoding("utf-8");
			this.ExtendedExportFile.TextFileEncoding = encoding;
			this.ExtendedExportFile.LoadFileEncoding = encoding;
			this.ExtendedExportFile.TypeOfExport = ExportFile.ExportType.AncestorSearch;
			this.ExtendedExportFile.ExportNative = true;
			this.ExtendedExportFile.TypeOfExportedFilePath = ExportFile.ExportedFilePathType.Absolute;
		}

		private void SetupImageExport()
		{
			this.ExtendedExportFile.ExportImages = true;
			this.ExtendedExportFile.LogFileFormat = LoadFileType.FileFormat.Opticon;
			this.ExtendedExportFile.TypeOfImage = ExportFile.ImageType.SinglePage;
		}

		private void SetupDelimiters()
		{
			this.ExtendedExportFile.MultiRecordDelimiter = ':';
			this.ExtendedExportFile.NestedValueDelimiter = '/';
			this.ExtendedExportFile.NewlineDelimiter = '\n';
			this.ExtendedExportFile.QuoteDelimiter = '"';
			this.ExtendedExportFile.RecordDelimiter = '\r';
		}

		private void SetupPaddings()
		{
			this.ExtendedExportFile.SubdirectoryDigitPadding = 3;
			this.ExtendedExportFile.VolumeDigitPadding = 2;
		}

		private void ImportDocumentsAndImages()
		{
			var documents = ImportHelper.ImportDocuments(TestParameters);
			ImportHelper.ImportImagesForDocuments(TestParameters, documents);
		}
	}
}