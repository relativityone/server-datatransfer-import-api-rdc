// ----------------------------------------------------------------------------
// <copyright file="SearchablePdfExportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading.Tasks;
	using global::NUnit.Framework;
	using kCura.WinEDDS;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
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
			if (!IntegrationTestHelper.IsRegressionEnvironment())
			{
				SearchablePdfTestHelper.SetupTestData(this.TestParameters);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA1801: Review unused parameters",
			Justification = "We are using TestExecutionContext.CurrentContext.CurrentTest.Arguments to retrieve value of client parameter.")]
		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IgnoreIfRegressionEnvironment("Ignored because 'SetupTestData' requires access to SQL to prepare pdfs to export.")]
		[IdentifiedTest("c1ea287a-44af-43fb-8a38-4956a5072de6")]
		public async Task ShouldExportSearchablePdfAsync(
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client,
			[Values(true, false)] bool exportPdf,
			[Values(true, false)] bool copyPdfFromRepository)
		{
			// ARRANGE
			SetupDefaultExportFile(this.ExtendedExportFile);
			this.ExtendedExportFile.ExportPdf = exportPdf;
			this.ExtendedExportFile.VolumeInfo.CopyPdfFilesFromRepository = copyPdfFromRepository;

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			ExportedFilesValidator.ValidateSearchablePdfsCount(this.ExtendedExportFile, TestData.SampleSearchablePdfTestFiles.Count());
			await ExportedFilesValidator.ValidateSearchablePdfFilesAsync(this.ExtendedExportFile).ConfigureAwait(false);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA1801: Review unused parameters",
			Justification = "We are using TestExecutionContext.CurrentContext.CurrentTest.Arguments to retrieve value of client parameter.")]
		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IgnoreIfRegressionEnvironment("Ignored because 'SetupTestData' requires access to SQL to prepare pdfs to export.")]
		[IdentifiedTest("e5976b64-107b-4d23-bd37-b748526768d9")]
		public async Task ShouldAddCorrectSubDirectoryPrefixWhenExportSearchablePdfAsync(
			[Values(null, "test_prefix")] string subDirectoryPrefix,
			[Values(TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			SetupDefaultExportFile(this.ExtendedExportFile);
			this.ExtendedExportFile.VolumeInfo.set_SubdirectoryPdfPrefix(false, subDirectoryPrefix);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			ExportedFilesValidator.ValidateSearchablePdfsCount(this.ExtendedExportFile, TestData.SampleSearchablePdfTestFiles.Count());
			ExportedFilesValidator.ValidateSearchablePdfSubDirectoryPrefix(this.ExtendedExportFile);
			await ExportedFilesValidator.ValidateSearchablePdfFilesAsync(this.ExtendedExportFile).ConfigureAwait(false);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA1801: Review unused parameters",
			Justification = "We are using TestExecutionContext.CurrentContext.CurrentTest.Arguments to retrieve value of client parameter.")]
		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IgnoreIfRegressionEnvironment("Ignored because 'SetupTestData' requires access to SQL to prepare pdfs to export.")]
		[IdentifiedTest("baa7ad81-c688-43f3-bb1a-9f953fe5b6d6")]
		public async Task ShouldDisplayWarningWhenInsufficientSubDirectoryPaddingAsync([Values(TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			const string InsufficientPaddingWarningMessage =
				"The selected subdirectory padding of 1 is less than the recommended subdirectory padding 2 for this export";
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

		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IgnoreIfRegressionEnvironment("Ignored because 'SetupTestData' requires access to SQL to prepare pdfs to export.")]
		[IdentifiedTest("48d687a6-0206-40f0-850b-681818192760")]
		public async Task ShouldAuditExportOfSearchablePdfAsync()
		{
			// ARRANGE
			int userId = await this.SetupNewUserAsync().ConfigureAwait(false);
			SetupDefaultExportFile(this.ExtendedExportFile);
			this.ExtendedExportFile.VolumeInfo.set_SubdirectoryPdfPrefix(false, RandomHelper.NextString(10, 15));

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			ExportedFilesValidator.ValidateSearchablePdfsCount(this.ExtendedExportFile, TestData.SampleSearchablePdfTestFiles.Count());
			await ExportedFilesValidator.ValidateSearchablePdfFilesAsync(this.ExtendedExportFile).ConfigureAwait(false);
			await this.ThenTheAuditIsCorrectAsync(userId).ConfigureAwait(false);
			await UsersHelper.RemoveUserAsync(this.TestParameters, userId).ConfigureAwait(false);
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

		private async Task<int> SetupNewUserAsync()
		{
			string newUsername = $"ImportAPI.{Guid.NewGuid()}@relativity.com";
			this.ExtendedExportFile.Credential = new NetworkCredential(newUsername, this.TestParameters.RelativityPassword);

			const int SystemAdministratorsGroupId = 20;
			return await UsersHelper.CreateNewUserAsync(
							  this.TestParameters,
							  newUsername,
							  this.TestParameters.RelativityPassword,
							  new[] { SystemAdministratorsGroupId }).ConfigureAwait(false);
		}
	}
}