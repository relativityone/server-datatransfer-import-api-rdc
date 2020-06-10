using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using kCura.Relativity.Client;
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
		private readonly string allDocumentsViewName = ConfigurationManager.AppSettings["AllDocumentsViewName"];

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

		public void ResetContext()
		{
			OneTimeTearDown();
			OneTimeSetUp();
		}

		[Test]
		public void ExportRenderedPdfs()
		{
			_ = RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int) ArtifactType.Document);
			ImportHelper.ImportDefaultTestData(TestParameters);
			SetPdfTypeForDocumentsInWorkspace();

			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = allDocumentsViewName,
				ExportPath = CreateExportPath(),
				VolumeInformationDigitPadding = 3,
				FilesNamedAfter = "Identifier",
				ExportImages = false,
				ExportNativeFiles = true,
				ExportFullTextAsFile = true,
				MetadataFileFormat = "Concordance (.dat)",
				MetadataFileEncoding = "Unicode (UTF-8)",
				TextFileEncoding = "Unicode (UTF-8)",
				TextFieldPrecedence = "Extracted Text",
				ExportRenderedPDFs = true,
				PDFPrefix = "PDF_TEST"
			};

			RunExportTest(exportParameters, x => x.ExportFolderAndSubfolders(), 21);

			ResetContext();
		}

		[Test]
		public void ExportNativesWithFullTextAsFilesFromFolderAndSubfoldersToDatFile()
		{
			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = allDocumentsViewName,
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

			RunExportTest(exportParameters, x => x.ExportFolderAndSubfolders(), 21);
		}

		[Test]
		public void ExportImagesFromFolderAndSubfoldersToOpticonFile()
		{
			var exportParameters = new ExportWindowSetupParameters
			{
				FieldSourceName = allDocumentsViewName,
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

			RunExportTest(exportParameters, x => x.ExportFolderAndSubfolders(), 12);
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

			RunExportTest(exportParameters, x => x.ExportProductionSet(), 52);
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

			RunExportTest(exportParameters, x => x.ExportSavedSearch(), 32);
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

			RunExportTest(exportParameters, x => x.ExportImagingProfileObjects(), 1);
		}

		private void RunExportTest(ExportWindowSetupParameters exportParameters,
			Func<RelativityDesktopClientWindow, ExportWindow> getExportWindow,
			int expectedFilesCount)
		{
			var workspaceSelectWindow = Login();

			var rdcWindow = workspaceSelectWindow.ChooseWorkspace(TestParameters.WorkspaceName);
			rdcWindow.SelectRootFolder();
			rdcWindow.WaitForTransferModeDetection();
			rdcWindow.CaptureWindowScreenshot();

			var exportWindow = getExportWindow(rdcWindow);
			exportWindow.SetupExport(exportParameters);

			var progressWindow = exportWindow.RunExport();

			var allRecordsProcessed = progressWindow.WaitForAllRecordsToBeProcessed(TimeSpan.FromMinutes(5));

			if (RdcWindowsManager.TryGetRdcConfirmationDialog(out DialogWindow confirmationDialog))
			{
				confirmationDialog.CaptureWindowScreenshot();
				confirmationDialog.ClickButton("Cancel");
				progressWindow.SwitchToWindow();
			}

			var progressStatus = progressWindow.StatusText;
			var errors = progressWindow.GetErrorsText();
			var files = Directory.GetFiles(exportParameters.ExportPath, "*.*", SearchOption.AllDirectories);

			if (string.IsNullOrEmpty(errors) && allRecordsProcessed)
			{
				CloseSession();
				Directory.Delete(exportParameters.ExportPath, true);
			}

			// Assert
			Assert.IsTrue(string.IsNullOrEmpty(errors), $"Export failed with errors: {errors}");
			Assert.IsTrue(allRecordsProcessed, $"Failed to process all records. Status: {progressStatus}");
			Assert.AreEqual(expectedFilesCount, files.Length);
		}

		private string CreateExportPath()
		{
			var exportPath = Path.Combine(exportRootPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
			Directory.CreateDirectory(exportPath);
			return exportPath;
		}

		private void ImportDataForExport()
		{
			var documents = ImportHelper.ImportDocuments(TestParameters).ToList();
			ImportHelper.ImportImagesForDocuments(TestParameters, documents);
			ImportHelper.ImportProduction(TestParameters, ProductionName, documents);
		}

		private void SetPdfTypeForDocumentsInWorkspace()
		{
			// In future we can potentially replace this code with Production service that would produce searchable pdf
			int pdfType = (int) Relativity.DataExchange.Service.FileType.Pdf;
			int nativeType = (int) Relativity.DataExchange.Service.FileType.Native;

			SqlConnectionStringBuilder builder =
				IntegrationTestHelper.GetSqlConnectionStringBuilder(this.TestParameters);

			using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText =
						$@"Update [EDDS{this.TestParameters.WorkspaceId}].[EDDSDBO].[File] Set Type = '{pdfType}' WHERE Type = '{nativeType}'";
					command.ExecuteScalar();
				}
			}

		}
	}
}