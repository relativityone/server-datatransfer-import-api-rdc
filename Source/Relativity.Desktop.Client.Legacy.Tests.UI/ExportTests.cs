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
		private readonly string exportRootPath = PathsProvider.GetTestOutputPath(@"Export");

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
				ExportPath = CreateExportPath(),
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

			RunExportTest(exportParameters, x => x.ExportFolderAndSubfolders(), 31, 429283);
		}

		[Test]
		public void ExportImagesFromFolderAndSubfoldersToOpticonFile()
		{
			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = "Documents - All Metadata",
				ExportPath = CreateExportPath(),
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

			RunExportTest(exportParameters, x => x.ExportFolderAndSubfolders(), 12, 7515998);
		}

		[Test]
		public void ExportProductionSetAsNativesAndImagesAndTextFiles()
		{
			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = ProductionName,
				ExportPath = CreateExportPath(),
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

			RunExportTest(exportParameters, x => x.ExportProductionSet(), 52, 7535217);
		}

		[Test]
		public void ExportSavedSearchAsNativesAndImagesAndTextFiles()
		{
			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = "All Documents",
				ExportPath = CreateExportPath(),
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

			RunExportTest(exportParameters, x => x.ExportSavedSearch(), 42, 7969054);
		}

		[Test]
		public void ExportImagingProfiles()
		{
			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = "All Imaging Profiles",
				ExportPath = CreateExportPath(),
				VolumeInformationDigitPadding = 3,
				FilesNamedAfter = "Identifier",
				MetadataFileFormat = "Concordance (.dat)",
				MetadataFileEncoding = "Unicode (UTF-8)"
			};

			RunExportTest(exportParameters, x => x.ExportImagingProfileObjects(), 1, 159);
		}

		private void RunExportTest(ExportWindowSetupParameters exportParameters,
			Func<RelativityDesktopClientWindow, ExportWindow> getExportWindow,
			int expectedFilesCount,
			int expectedFilesSize)
		{
			var workspaceSelectWindow = Login();

			var rdcWindow = workspaceSelectWindow.ChooseWorkspace(TestParameters.WorkspaceName);
			rdcWindow.SelectRootFolder();

			var exportWindow = getExportWindow(rdcWindow);
			exportWindow.SetupExport(exportParameters);
			var progressWindow = exportWindow.RunExport();

			var allRecordsProcessed = progressWindow.WaitForAllRecordsToBeProcessed(TimeSpan.FromMinutes(5));
			var progressStatus = progressWindow.StatusText;
			var errors = progressWindow.GetErrorsText();
			var exportDir = new DirectoryInfo(exportParameters.ExportPath);
			var files = exportDir.GetFiles("*.*", SearchOption.AllDirectories);
			var filesSize = files.Sum(x => x.Length);

			if (string.IsNullOrEmpty(errors) && allRecordsProcessed)
			{
				CloseSession();
				Directory.Delete(exportParameters.ExportPath, true);
			}

			// Assert
			Assert.IsTrue(string.IsNullOrEmpty(errors), $"Export failed with errors: {errors}");
			Assert.IsTrue(allRecordsProcessed, $"Failed to process all records. Status: {progressStatus}");
			Assert.AreEqual(expectedFilesCount, files.Length);
			Assert.AreEqual(expectedFilesSize, filesSize);
		}

		private string CreateExportPath()
		{
			var exportPath =  Path.Combine(exportRootPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
			Directory.CreateDirectory(exportPath);
			return exportPath;
		}

		private void ImportDataForExport()
		{
			var documents = ImportHelper.ImportDocuments(TestParameters).ToList();
			ImportHelper.ImportImagesForDocuments(TestParameters, documents);
			ImportHelper.ImportProduction(TestParameters, ProductionName, documents);
		}
	}
}