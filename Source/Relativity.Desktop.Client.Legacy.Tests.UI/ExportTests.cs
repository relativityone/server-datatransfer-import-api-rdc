using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Relativity.DataExchange.TestFramework;
using Relativity.DataExchange.TestFramework.RelativityHelpers;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows;
using Relativity.Desktop.Client.Legacy.Tests.UI.Workflow;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	[TestFixture]
	internal class ExportTests : RdcTestBase
	{
		private const string ProductionName = "Production-For-Export";
		private readonly string exportPath = PathsProvider.GetTestOutputPath(@"Export");

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			TestParameters = IntegrationTestHelper.Create();
			ImportDataForExport();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (TestParameters != null)
			{
				IntegrationTestHelper.Destroy(TestParameters);
				TestParameters = null;
			}
		}

		[Test]
		public void ExportNativesWithFullTextAsFilesFromFolderAndSubfoldersToDatFile()
		{
			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = "Documents - All Metadata",
				ExportPath = exportPath,
				VolumeInformationDigitPadding = 3,
				FilesNamedAfter = "Identifier",
				ExportImages = false,
				ExportNativeFiles = true,
				ExportFullTextAsFile = true,
				MetadataFileFormat = "Concordance (.dat)",
				MetadataFileEncoding = "Unicode (UTF-8)",
				TextFileEncoding = "Unicode (UTF-8)",
				TextFieldPrecedence = "Extracted Text"
			};

			RunExportTest(exportParameters, x => x.ExportFolderAndSubfolders(), 31, 3627);
		}

		[Test]
		public void ExportImagesFromFolderAndSubfoldersToOpticonFile()
		{
			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = "Documents - All Metadata",
				ExportPath = exportPath,
				VolumeInformationDigitPadding = 3,
				FilesNamedAfter = "Identifier",
				ExportImages = true,
				ExportNativeFiles = false,
				ExportFullTextAsFile = false,
				MetadataFileFormat = "Concordance (.dat)",
				MetadataFileEncoding = "Unicode (UTF-8)",
				ImageFileFormat = "Opticon",
				ImageFileType = "Multi-page TIF"
			};

			RunExportTest(exportParameters, x => x.ExportFolderAndSubfolders(), 12, 1452);
		}

		[Test]
		public void ExportProductionSetAsNativesAndImagesAndTextFiles()
		{
			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = ProductionName,
				ExportPath = exportPath,
				VolumeInformationDigitPadding = 3,
				FilesNamedAfter = "Begin production number",
				ExportImages = true,
				ExportNativeFiles = true,
				ExportFullTextAsFile = true,
				MetadataFileFormat = "Comma-separated (.csv)",
				MetadataFileEncoding = "Unicode (UTF-8)",
				TextFileEncoding = "Unicode (UTF-8)",
				TextFieldPrecedence = "Extracted Text",
				ImageFileFormat = "IPRO",
				ImageFileType = "Single-page TIF/JPG"
			};

			RunExportTest(exportParameters, x => x.ExportProductionSet(), 52, 5186);
		}

		[Test]
		public void ExportSavedSearchAsNativesAndImagesAndTextFiles()
		{
			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = "All Documents",
				ExportPath = exportPath,
				VolumeInformationDigitPadding = 3,
				FilesNamedAfter = "Identifier",
				ExportImages = true,
				ExportNativeFiles = true,
				ExportFullTextAsFile = true,
				MetadataFileFormat = "HTML (.html)",
				MetadataFileEncoding = "Unicode (UTF-8)",
				TextFileEncoding = "Unicode (UTF-8)",
				TextFieldPrecedence = "Extracted Text",
				ImageFileFormat = "IPRO (FullText)",
				ImageFileType = "PDF"
			};

			RunExportTest(exportParameters, x => x.ExportSavedSearch(), 42, 4972);
		}

		[Test]
		public void ExportImagingProfiles()
		{
			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = "All Imaging Profiles",
				ExportPath = exportPath,
				VolumeInformationDigitPadding = 3,
				FilesNamedAfter = "Identifier",
				MetadataFileFormat = "Concordance (.dat)",
				MetadataFileEncoding = "Unicode (UTF-8)"
			};

			RunExportTest(exportParameters, x => x.ExportImagingProfileObjects(), 1, 92);
		}

		private void RunExportTest(ExportWindowSetupParameters exportParameters,
			Func<RelativityDesktopClientWindow, ExportWindow> getExportWindow,
			int expectedFilesCount,
			int expectedFilesSize)
		{
			EnsureExportPathExists();

			var workspaceSelectWindow = Login();

			var rdcWindow = workspaceSelectWindow.ChooseWorkspace(TestParameters.WorkspaceName);
			rdcWindow.SelectRootFolder();

			var exportWindow = getExportWindow(rdcWindow);
			exportWindow.SetupExport(exportParameters);
			var progressWindow = exportWindow.RunExport();

			var allRecordsProcessed = progressWindow.WaitForAllRecordsToBeProcessed(TimeSpan.FromMinutes(2));
			var errors = progressWindow.GetErrorsText();
			var files = Directory.GetFiles(exportPath, "*.*", SearchOption.AllDirectories);
			var filesSize = files.Sum(x => x.Length);
			Directory.Delete(exportPath, true);

			// Assert
			Assert.IsTrue(allRecordsProcessed, "Failed to process all records");
			Assert.IsTrue(string.IsNullOrEmpty(errors), $"Export failed with errors: {errors}");
			Assert.AreEqual(expectedFilesCount, files.Length);
			Assert.AreEqual(expectedFilesSize, filesSize);
		}

		private void EnsureExportPathExists()
		{
			var exportFolder = new DirectoryInfo(exportPath);
			if (exportFolder.Exists)
			{
				exportFolder.Delete(true);
				exportFolder.Create();
			}
			else
			{
				exportFolder.Create();
			}
		}

		private void ImportDataForExport()
		{
			var documents = ImportHelper.ImportDocuments(TestParameters);
			ImportHelper.ImportImagesForDocuments(TestParameters, documents);
			ImportHelper.ImportProduction(TestParameters, ProductionName, documents);
		}
	}
}