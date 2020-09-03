// ----------------------------------------------------------------------------
// <copyright file="SearchablePdfExportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using global::NUnit.Framework;
	using kCura.WinEDDS;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	public class SearchablePdfExportTests : ExportTestBase
	{
		private const RelativityVersion MinSupportedVersion = RelativityVersion.MayappleExportPDFs;

		protected override IntegrationTestParameters TestParameters => AssemblySetup.TestParameters;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			SearchablePdfTestHelper.SetupTestData(this.TestParameters);
		}

		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("c1ea287a-44af-43fb-8a38-4956a5072de6")]
		public async Task ShouldExportSearchablePdfAsync(
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client,
			[Values(true, false)] bool exportPdf,
			[Values(true, false)] bool copyPdfFromRepository)
		{
			// ARRANGE
			GivenTheTapiForceClientAppSettings(client);
			SetupDefaultExportFile(this.ExtendedExportFile);
			this.ExtendedExportFile.ExportPdf = exportPdf;
			this.ExtendedExportFile.VolumeInfo.CopyPdfFilesFromRepository = copyPdfFromRepository;

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			ExportedFilesValidator.ValidateSearchablePdfsCount(this.ExtendedExportFile, TestData.SampleSearchablePdfTestFiles.Count());
			await ExportedFilesValidator.ValidateSearchablePdfFilesAsync(this.ExtendedExportFile).ConfigureAwait(false);
			await this.ThenTheAuditIsCorrectAsync().ConfigureAwait(false);
		}

		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("e5976b64-107b-4d23-bd37-b748526768d9")]
		public async Task ShouldAddCorrectSubDirectoryPrefixWhenExportSearchablePdfAsync(
			[Values(null, "test_prefix")] string subDirectoryPrefix)
		{
			// ARRANGE
			GivenTheTapiForceClientAppSettings(TapiClient.Web);
			SetupDefaultExportFile(this.ExtendedExportFile);
			this.ExtendedExportFile.VolumeInfo.set_SubdirectoryPdfPrefix(false, subDirectoryPrefix);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			ExportedFilesValidator.ValidateSearchablePdfsCount(this.ExtendedExportFile, TestData.SampleSearchablePdfTestFiles.Count());
			ExportedFilesValidator.ValidateSearchablePdfSubDirectoryPrefix(this.ExtendedExportFile);
			await ExportedFilesValidator.ValidateSearchablePdfFilesAsync(this.ExtendedExportFile).ConfigureAwait(false);
		}

		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("baa7ad81-c688-43f3-bb1a-9f953fe5b6d6")]
		public async Task ShouldDisplayWarningWhenInsufficientSubDirectoryPaddingAsync()
		{
			// ARRANGE
			const string InsufficientPaddingWarningMessage =
				"The selected subdirectory padding of 1 is less than the recommended subdirectory padding 2 for this export";
			GivenTheTapiForceClientAppSettings(TapiClient.Web);
			SetupDefaultExportFile(this.ExtendedExportFile);
			this.ExtendedExportFile.SubdirectoryDigitPadding = 1;
			this.ExtendedExportFile.VolumeInfo.SubdirectoryStartNumber =
				TestData.SampleSearchablePdfTestFiles.Count() >= 10
					? 1
					: 11 - TestData.SampleSearchablePdfTestFiles.Count();
			this.ExtendedExportFile.VolumeInfo.SubdirectoryMaxSize = 1;

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			ExportedFilesValidator.ValidateSearchablePdfsCount(this.ExtendedExportFile, TestData.SampleSearchablePdfTestFiles.Count());
			ExportedFilesValidator.ValidateSearchablePdfSubDirectoryPrefix(this.ExtendedExportFile);
			ThenTheStatusMessageWasAdded(InsufficientPaddingWarningMessage);
			await ExportedFilesValidator.ValidateSearchablePdfFilesAsync(this.ExtendedExportFile).ConfigureAwait(false);
		}

		private static void SetupDefaultExportFile(ExportFile exportFile)
		{
			exportFile.TypeOfExport = ExportFile.ExportType.ParentSearch;

			exportFile.TextFileEncoding = Encoding.GetEncoding("utf-8");
			exportFile.LoadFileEncoding = Encoding.GetEncoding("utf-8");
			exportFile.LoadFileExtension = "dat";

			exportFile.TypeOfExportedFilePath = ExportFile.ExportedFilePathType.Absolute;
			exportFile.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier;
			exportFile.ExportNative = false;
			exportFile.ExportImages = false;
			exportFile.ExportPdf = true;
			exportFile.VolumeInfo.CopyPdfFilesFromRepository = true;

			exportFile.SubdirectoryDigitPadding = 3;
			exportFile.VolumeDigitPadding = 2;

			exportFile.MultiRecordDelimiter = ';';
			exportFile.NestedValueDelimiter = '\\';
			exportFile.NewlineDelimiter = '@';
			exportFile.QuoteDelimiter = 'þ';
			exportFile.RecordDelimiter = '¶';
		}
	}
}